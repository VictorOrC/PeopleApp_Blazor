using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization;
using PeopleApp.Client;
using PeopleApp.Client.Services;
using PeopleApp.Client.Services.Auth;
using PeopleApp.Client.Services.Http;
using PeopleApp.Client.Auth;
using Blazored.LocalStorage;
using PeopleApp.Client.Services.Products;
using PeopleApp.Client.Services.Purchases;
using PeopleApp.Client.Services.ApiClients;

using Radzen;


var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddBlazoredLocalStorage();

// Registrar autenticación y autorización
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<ITokenStore, TokenStore>();

// Registrar JwtAuthenticationStateProvider como AuthenticationStateProvider
builder.Services.AddScoped<JwtAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<JwtAuthenticationStateProvider>());

// Registrar AuthHeaderHandler
builder.Services.AddScoped<AuthHeaderHandler>();

builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<TooltipService>();
builder.Services.AddScoped<ContextMenuService>();

builder.Services.AddScoped<ProductsApiClient>();
builder.Services.AddScoped<PurchasesApiClient>();

builder.Services.AddRadzenComponents();

builder.Services.AddScoped<ReportsApiClient>();



// Registrar HttpClient con AuthHeaderHandler correctamente
builder.Services.AddScoped(sp =>
{
    var handler = sp.GetRequiredService<AuthHeaderHandler>();
    var innerHandler = new HttpClientHandler();
    handler.InnerHandler = innerHandler;
    return new HttpClient(handler)
    {
        BaseAddress = new Uri("http://localhost:5229/")
    };
});

// Registrar servicio de alto nivel AuthApiClient
builder.Services.AddScoped<AuthApiClient>();

// Registrar servicio orquestador AuthService
builder.Services.AddScoped<AuthService>();

await builder.Build().RunAsync();


