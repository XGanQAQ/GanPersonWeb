using MudBlazor.Services;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
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
            //写法一：提前配置好HttpClient
            services.AddScoped(sp =>
            {
                var navigationManager = sp.GetRequiredService<Microsoft.AspNetCore.Components.NavigationManager>();
                return new HttpClient { BaseAddress = new Uri(navigationManager.BaseUri)};
            });

            services.AddScoped<ClientProjectService>();
            services.AddScoped<ClientBlogService>();
            services.AddScoped<ClientPersonInfoService>();
            services.AddScoped<ClientSiteVisitService>();
            services.AddScoped<ClientUserService>();
            services.AddMudServices();
            services.AddBlazoredLocalStorage();
            services.AddScoped<JwtHelperService>();

        }
    }
}
