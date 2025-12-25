using Microsoft.AspNetCore.Components;

namespace SpotifyArchiver.Presentation.Playlists.Components
{
    public partial class OperationButton
    {
        [Parameter]
        public EventCallback OnClicked { get; set; }

        [Parameter]
        public string ButtonText { get; set; } = string.Empty;

        private bool _isLoading = false;

        private async Task HandleClick()
        {
            _isLoading = true;
            await InvokeAsync(StateHasChanged);

            await OnClicked.InvokeAsync();

            _isLoading = false;
            await InvokeAsync(StateHasChanged);
        }
    }
}
