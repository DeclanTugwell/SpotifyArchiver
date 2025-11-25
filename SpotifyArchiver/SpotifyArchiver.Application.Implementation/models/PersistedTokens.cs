namespace SpotifyArchiver.Application.Implementation.models
{
    public class PersistedTokens
    {
        public string? RefreshToken { get; set; }
        public string? AccessToken { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
