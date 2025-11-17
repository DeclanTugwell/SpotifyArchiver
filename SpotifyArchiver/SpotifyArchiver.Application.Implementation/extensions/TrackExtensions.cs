using SpotifyAPI.Web;
using SpotifyArchiver.DataAccess.Abstraction.entities;

namespace SpotifyArchiver.Application.Implementation.extensions
{
    public static class TrackExtensions
    {
        public static Track? ToTrack(this PlaylistTrack<IPlayableItem> spotifyTrack)
        {
            if (spotifyTrack.Track is FullTrack { Id: not null, Name: not null, Uri: not null } track)
            {
                return new Track
                {
                    SpotifyId = track.Id,
                    Name = track.Name,
                    ArtistName = string.Join(", ", track.Artists.Select(artist => artist.Name)),
                    SpotifyUri = track.Uri
                };
            }
            return null;
        }
    }
}
