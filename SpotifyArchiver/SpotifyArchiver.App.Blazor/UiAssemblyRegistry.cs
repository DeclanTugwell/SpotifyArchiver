using System.Reflection;

namespace SpotifyArchiver.App.Blazor
{
    public static class UiAssemblyRegistry
    {
        public static Assembly[] RegisteredAssemblies { get; } =
        [
            typeof(SpotifyArchiver.Presentation.Shared._Imports).Assembly,
            typeof(SpotifyArchiver.Presentation.Playlists._Imports).Assembly
        ];
    }
}
