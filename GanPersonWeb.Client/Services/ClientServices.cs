using MudBlazor.Services;
using Blazored.LocalStorage;
namespace GanPersonWeb.Client.Services
{
    /// <summary>
    /// This class is used to register all the client services in the application.
    /// shared services between server and client.
    /// </summary>
    public class ClientServices
    {
        public static void RegisterServices(IServiceCollection services)
        {
            services.AddScoped(sp =>
            {
                var navigationManager = sp.GetRequiredService<Microsoft.AspNetCore.Components.NavigationManager>();
                return new HttpClient { BaseAddress = new Uri(navigationManager.BaseUri) };
            });
            services.AddScoped<ProjectService>();
            services.AddScoped<AuthService>();
            services.AddMudServices();
            services.AddBlazoredLocalStorage();
            services.AddScoped<JwtHelperService>();
        }
    }
}
