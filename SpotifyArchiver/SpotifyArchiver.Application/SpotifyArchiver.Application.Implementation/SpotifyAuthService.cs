using SpotifyAPI.Web;
using SpotifyArchiver.Application.Implementation.models;
using System.Diagnostics;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace SpotifyArchiver.Application.Implementation
{
    public class SpotifyAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly string _clientId;
        private readonly string _redirectUri;
        private readonly string _configPath;

        private string? _refreshToken;
        private AuthorizationCodeTokenResponse? _currentTokens;

        private string _codeVerifier = "";

        public SpotifyAuthService(string clientId, string redirectUri, string configPath)
        {
            _clientId = clientId;
            _redirectUri = redirectUri;
            _configPath = configPath;

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://accounts.spotify.com")
            };

            LoadStoredTokens();
        }

        public async Task<SpotifyClient?> FetchAuthenticatedClient(CancellationToken token)
        {
            try
            {
                if (!string.IsNullOrEmpty(_refreshToken))
                {
                    _currentTokens = await RefreshTokenAsync(_refreshToken);
                    SaveTokens(_currentTokens);
                }
                else
                {
                    _currentTokens = await FullAuthorizationAsync();
                    SaveTokens(_currentTokens);
                }

                var authenticator = new AuthorizationCodeAuthenticator(_clientId, string.Empty, _currentTokens);
                authenticator.TokenRefreshed += (_, newTokens) => SaveTokens(newTokens);

                var spotifyConfig = SpotifyClientConfig.CreateDefault()
                    .WithAuthenticator(authenticator);
                var spotifyClient = new SpotifyClient(spotifyConfig);
                return spotifyClient;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Authentication failed: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Runs through the complete Auth Flow:
        /// Opens Spotify Auth Portal, upon success reroutes to <see cref="_redirectUri"/>
        /// </summary>
        private async Task<AuthorizationCodeTokenResponse> FullAuthorizationAsync()
        {
            _codeVerifier = GenerateCodeVerifier();
            var codeChallenge = GenerateCodeChallenge(_codeVerifier);

            var loginRequest = new LoginRequest(new Uri(_redirectUri), _clientId, LoginRequest.ResponseType.Code)
            {
                Scope =
                [
                    Scopes.UserReadPlaybackState,
                Scopes.UserModifyPlaybackState,
                Scopes.UserReadCurrentlyPlaying
                ],
                CodeChallenge = codeChallenge,
                CodeChallengeMethod = "S256"
            };

            var authUrl = loginRequest.ToUri();

            Process.Start(new ProcessStartInfo
            {
                FileName = authUrl.ToString(),
                UseShellExecute = true
            });

            var code = await WaitForSpotifyAuthCodeAsync(_redirectUri);

            var authResponse = await ExchangeCodeForTokenAsync(code);

            return authResponse;
        }

        private async Task<AuthorizationCodeTokenResponse> RefreshTokenAsync(string refreshToken)
        {
            var requestBody = new Dictionary<string, string>
        {
            { "grant_type", "refresh_token" },
            { "refresh_token", refreshToken },
            { "client_id", _clientId }
        };

            return await RequestTokenAsync(requestBody);
        }

        private async Task<AuthorizationCodeTokenResponse> ExchangeCodeForTokenAsync(string code)
        {
            var requestBody = new Dictionary<string, string>
        {
            { "grant_type", "authorization_code" },
            { "code", code },
            { "redirect_uri", _redirectUri },
            { "client_id", _clientId },
            { "code_verifier", _codeVerifier }
        };

            return await RequestTokenAsync(requestBody);
        }

        private async Task<AuthorizationCodeTokenResponse> RequestTokenAsync(Dictionary<string, string> body)
        {
            var resp = await _httpClient.PostAsync("/api/token", new FormUrlEncodedContent(body));

            if (!resp.IsSuccessStatusCode)
            {
                var err = await resp.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"Spotify token request failed: {resp.StatusCode} - {err}");
            }

            var content = await resp.Content.ReadAsStringAsync();
            var tokenData = JsonSerializer.Deserialize<SpotifyTokenResponse>(content)
                ?? throw new InvalidOperationException("Failed to deserialize token response");

            _refreshToken = tokenData.RefreshToken ?? _refreshToken;

            return new AuthorizationCodeTokenResponse
            {
                AccessToken = tokenData.AccessToken,
                TokenType = tokenData.TokenType,
                ExpiresIn = tokenData.ExpiresIn,
                Scope = tokenData.Scope,
                RefreshToken = _refreshToken,
                CreatedAt = DateTime.UtcNow
            };
        }

        private async Task<string> WaitForSpotifyAuthCodeAsync(string registeredRedirectUri)
        {
            var listener = new HttpListener();
            listener.Prefixes.Add(registeredRedirectUri.TrimEnd('/') + "/");
            listener.Start();

            var context = await listener.GetContextAsync();
            var code = context.Request.QueryString["code"];

            var buffer = "Authentication complete. You may close this window."u8.ToArray();
            context.Response.ContentLength64 = buffer.Length;
            await context.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
            context.Response.OutputStream.Close();

            listener.Stop();

            if (string.IsNullOrEmpty(code))
                throw new InvalidOperationException("No code returned from Spotify auth.");

            return code;
        }

        private void LoadStoredTokens()
        {
            if (!File.Exists(_configPath))
                return;

            var json = File.ReadAllText(_configPath);
            var saved = JsonSerializer.Deserialize<PersistedTokens>(json);
            _refreshToken = saved?.RefreshToken;
        }

        private void SaveTokens(AuthorizationCodeTokenResponse tokens)
        {
            var persisted = new PersistedTokens
            {
                RefreshToken = tokens.RefreshToken,
                AccessToken = tokens.AccessToken,
                CreatedAt = tokens.CreatedAt
            };

            var json = JsonSerializer.Serialize(persisted);
            File.WriteAllText(_configPath, json);
        }

        private static string GenerateCodeVerifier()
        {
            var randomBytes = new byte[64];
            RandomNumberGenerator.Fill(randomBytes);
            return Base64UrlEncode(randomBytes);
        }

        private static string GenerateCodeChallenge(string verifier)
        {
            using var sha256 = SHA256.Create();
            var challengeBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(verifier));
            return Base64UrlEncode(challengeBytes);
        }

        private static string Base64UrlEncode(byte[] input)
        {
            return Convert.ToBase64String(input)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }
    }
}
