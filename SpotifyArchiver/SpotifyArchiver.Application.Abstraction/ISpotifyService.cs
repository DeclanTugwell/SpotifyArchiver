namespace SpotifyArchiver.Application.Abstraction
{
    public interface ISpotifyService
    {
        public Task<bool> EnsureAuthenticatedAsync(CancellationToken token);
    }
}
