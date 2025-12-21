using Microsoft.AspNetCore.Components;

namespace SpotifyArchiver.Presentation.Playlists.Components
{
    public partial class OperationButton
    {
        [Parameter]
        public Func<Task> OnClick { get; set; } = async () => { await Task.CompletedTask; };

        [Parameter]
        public string ButtonText { get; set; } = string.Empty;

        private bool _isLoading = false;

        private async Task HandleClick()
        {
            _isLoading = true;
            StateHasChanged();
            await OnClick.Invoke();
            _isLoading = false;
            StateHasChanged();
        }
    }
}
