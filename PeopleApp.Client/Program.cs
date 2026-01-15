using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using PeopleApp.Client;
using PeopleApp.Client.Services;
using PeopleApp.Client.Services.Auth;
using PeopleApp.Client.Services.Http;
using Blazored.LocalStorage;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddBlazoredLocalStorage();

builder.Services.AddScoped<ITokenStore, TokenStore>();

// Registrar AuthHeaderHandler
builder.Services.AddScoped<AuthHeaderHandler>();

// Registrar HttpClient con BaseAddress de la API y AuthHeaderHandler
builder.Services.AddScoped(sp =>
{
    var handler = sp.GetRequiredService<AuthHeaderHandler>();
    var apiClient = new HttpClient(handler)
    {
        BaseAddress = new Uri("http://localhost:5229/")
    };
    return apiClient;
});

// Registrar servicio de alto nivel AuthApiClient
builder.Services.AddScoped<AuthApiClient>();

// Registrar servicio orquestador AuthService
builder.Services.AddScoped<AuthService>();

await builder.Build().RunAsync();


