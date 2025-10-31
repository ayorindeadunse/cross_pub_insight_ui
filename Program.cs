using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using CrossPubInsightUI;
using CrossPubInsightUI.Services;
using CrossPubInsightUI.Models;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configure HTTP client for the Blazor app itself
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Configure API settings from appsettings.json
builder.Services.Configure<ApiSettings>(
    builder.Configuration.GetSection("ApiSettings"));

// Configure HTTP client for CPIA API communication
builder.Services.AddHttpClient<CpiaApiService>((serviceProvider, client) =>
{
    var configuration = serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<ApiSettings>>();
    var apiSettings = configuration.Value;
    
    // Configure the base address from configuration
    client.BaseAddress = new Uri(apiSettings.CpiaApiBaseUrl);
    client.Timeout = TimeSpan.FromMinutes(5); // Allow for longer analysis operations
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

await builder.Build().RunAsync();
