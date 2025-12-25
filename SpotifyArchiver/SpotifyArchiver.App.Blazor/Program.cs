using Microsoft.EntityFrameworkCore;
using SpotifyArchiver.App.Blazor;
using SpotifyArchiver.App.Blazor.Components;
using SpotifyArchiver.Application.Abstraction;
using SpotifyArchiver.Application.Implementation;
using SpotifyArchiver.DataAccess.Abstraction;
using SpotifyArchiver.DataAccess.Implementation;
using SQLitePCL;

var clientId = Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_ID") ?? throw new InvalidOperationException("SPOTIFY_CLIENT_ID not set in Environment Variables");
var redirectUri = Environment.GetEnvironmentVariable("SPOTIFY_REDIRECT_URI") ?? throw new InvalidOperationException("SPOTIFY_REDIRECT_URI not set in Environment Variables");
var configPath = "spotify_tokens.json";

Batteries.Init();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddDbContext<MusicDbContext>(options =>
    options.UseSqlite("Data Source=music.db"));

builder.Services.AddScoped<IPlaylistRepository, PlaylistRepository>();
builder.Services.AddScoped<ISpotifyService, SpotifyService>(provider =>
{
    var repo = provider.GetRequiredService<IPlaylistRepository>();
    return new SpotifyService(clientId, redirectUri, configPath, repo);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddAdditionalAssemblies(UiAssemblyRegistry.RegisteredAssemblies)
    .AddInteractiveServerRenderMode();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MusicDbContext>();
    db.Database.Migrate();
}

app.Run();


