using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using SpotifyArchiver.Application.Abstraction;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static SpotifyAPI.Web.Scopes;

namespace SpotifyArchiver.Application.Implementation;

public class SpotifyAuthService : ISpotifyAuthService
{
    private readonly string? _clientId = Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_ID");
    private static readonly string RedirectUri = Environment.GetEnvironmentVariable("SPOTIFY_REDIRECT_URI")!;

    private ISpotifyClient? _spotifyClient;

    public async Task<ISpotifyClient> GetAuthenticatedClient()
    {
        if (_spotifyClient is not null)
        {
            Console.WriteLine("Returning cached Spotify client.");
            return _spotifyClient;
        }

        if (string.IsNullOrEmpty(_clientId) || string.IsNullOrEmpty(RedirectUri))
        {
            throw new InvalidOperationException("Spotify client ID or redirect URI not set in environment variables.");
        }

        var (verifier, challenge) = PKCEUtil.GenerateCodes();

        // Start the listener and get the authorization code
        var authCode = await ListenForAuthCode(challenge);

        // Exchange the code for a token
        Console.WriteLine("Requesting access token...");
        var token = await new OAuthClient().RequestToken(
            new PKCETokenRequest(_clientId, authCode, new Uri(RedirectUri), verifier)
        );
        _spotifyClient = new SpotifyClient(token);
        Console.WriteLine("Access token received, client created.");

        return _spotifyClient;
    }

    private async Task<string> ListenForAuthCode(string challenge)
    {
        using var listener = new HttpListener();
        // HttpListener prefixes must end with a '/'
        var listenerPrefix = RedirectUri.EndsWith("/") ? RedirectUri : RedirectUri + "/";
        listener.Prefixes.Add(listenerPrefix);
        
        try
        {
            Console.WriteLine("Starting HttpListener...");
            listener.Start();

            var request = new LoginRequest(new Uri(RedirectUri), _clientId!, LoginRequest.ResponseType.Code)
            {
                CodeChallenge = challenge,
                CodeChallengeMethod = "S256",
                Scope = new[] { UserReadEmail, UserReadPrivate, PlaylistReadPrivate, PlaylistReadCollaborative }
            };

            Console.WriteLine("Opening browser for user authentication...");
            BrowserUtil.Open(request.ToUri());

            Console.WriteLine("Waiting for authentication callback from HttpListener...");
            var context = await listener.GetContextAsync();
            
            // Send a response to the browser
            var response = context.Response;
            var responseString = "<html><body>Authentication successful! You can close this window.</body></html>";
            var buffer = Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            var output = response.OutputStream;
            await output.WriteAsync(buffer, 0, buffer.Length);
            output.Close();

            Console.WriteLine("Authorization code received.");
            var code = context.Request.QueryString.Get("code");
            if (string.IsNullOrEmpty(code))
            {
                throw new InvalidOperationException("Authorization code was not found in the callback.");
            }
            return code;
        }
        finally
        {
            Console.WriteLine("Stopping HttpListener.");
            listener.Stop();
        }
    }
}