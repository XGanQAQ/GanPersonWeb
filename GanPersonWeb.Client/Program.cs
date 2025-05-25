using GanPersonWeb.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

ClientServices.RegisterServices(builder.Services);

await builder.Build().RunAsync();
