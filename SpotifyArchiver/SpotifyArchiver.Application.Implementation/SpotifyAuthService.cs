using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using System.Net;
using System.Text;
using System.Text.Json;
using SpotifyArchiver.Application.Abstraction;
using static SpotifyAPI.Web.Scopes;

namespace SpotifyArchiver.Application.Implementation;

public class SpotifyAuthService : ISpotifyAuthService
{
    private readonly string _credentialsPath = "spotify_tokens.json";
    private static readonly string? ClientId = Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_ID");
    private static readonly string? RedirectUriString = Environment.GetEnvironmentVariable("SPOTIFY_REDIRECT_URI");

    private ISpotifyClient? _spotifyClient;

    public async Task<ISpotifyClient> GetAuthenticatedClient()
    {
        if (_spotifyClient != null)
        {
            return _spotifyClient;
        }

        if (string.IsNullOrEmpty(ClientId) || string.IsNullOrEmpty(RedirectUriString))
        {
            throw new InvalidOperationException("Spotify ClientId and RedirectUri must be set in environment variables.");
        }

        if (File.Exists(_credentialsPath))
        {
            var json = await File.ReadAllTextAsync(_credentialsPath).ConfigureAwait(false);
            var token = JsonSerializer.Deserialize<PKCETokenResponse>(json);

            var authenticator = new PKCEAuthenticator(ClientId, token!);
            authenticator.TokenRefreshed += (sender, refreshedToken) => File.WriteAllTextAsync(_credentialsPath, JsonSerializer.Serialize(refreshedToken));

            var config = SpotifyClientConfig.CreateDefault().WithAuthenticator(authenticator);
            _spotifyClient = new SpotifyClient(config);
            return _spotifyClient;
        }

        return await StartAuthentication().ConfigureAwait(false);
    }

    private async Task<ISpotifyClient> StartAuthentication()
    {
        var (verifier, challenge) = PKCEUtil.GenerateCodes();
        var redirectUri = new Uri(RedirectUriString!);

        string listenerPrefix = RedirectUriString!;
        if (!listenerPrefix.EndsWith("/"))
        {
            listenerPrefix += "/";
        }

        using var listener = new HttpListener();
        listener.Prefixes.Add(listenerPrefix);
        listener.Start();

        var request = new LoginRequest(redirectUri, ClientId!, LoginRequest.ResponseType.Code)
        {
            CodeChallenge = challenge,
            CodeChallengeMethod = "S256",
            Scope = new[] { UserReadEmail, UserLibraryRead, PlaylistReadPrivate, PlaylistReadCollaborative }
        };

        try
        {
            BrowserUtil.Open(request.ToUri());
        }
        catch (Exception)
        {
            Console.WriteLine("Unable to open browser. Please manually open: " + request.ToUri());
        }

        var context = await listener.GetContextAsync().ConfigureAwait(false);
        var response = context.Response;
        string? code = context.Request.QueryString?.Get("code");

        if (string.IsNullOrEmpty(code))
        {
            var error = context.Request.QueryString?.Get("error");
            var errorResponse = Encoding.UTF8.GetBytes($"<html><body><h1>Authentication failed</h1><p>{error}</p></body></html>");
            response.ContentLength64 = errorResponse.Length;
            response.OutputStream.Write(errorResponse, 0, errorResponse.Length);
            response.Close();
            throw new Exception($"Authentication failed: {error}");
        }
        else
        {
            var successResponse = Encoding.UTF8.GetBytes("<html><body><h1>Authentication successful!</h1><p>You can now close this window.</p></body></html>");
            response.ContentLength64 = successResponse.Length;
            response.OutputStream.Write(successResponse, 0, successResponse.Length);
            response.Close();
        }
        
        listener.Stop();

        var token = await new OAuthClient().RequestToken(
            new PKCETokenRequest(ClientId!, code, redirectUri, verifier)
        ).ConfigureAwait(false);

        await File.WriteAllTextAsync(_credentialsPath, JsonSerializer.Serialize(token)).ConfigureAwait(false);

        var authenticator = new PKCEAuthenticator(ClientId!, token);
        authenticator.TokenRefreshed += (sender, refreshedToken) => File.WriteAllTextAsync(_credentialsPath, JsonSerializer.Serialize(refreshedToken));

        var config = SpotifyClientConfig.CreateDefault().WithAuthenticator(authenticator);
        _spotifyClient = new SpotifyClient(config);
        return _spotifyClient;
    }
}