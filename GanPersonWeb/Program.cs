using MudBlazor.Services;
using GanPersonWeb.Client.Pages;
using GanPersonWeb.Components;
using GanPersonWeb.Data;
using GanPersonWeb.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using GanPersonWeb.Client.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;


var builder = WebApplication.CreateBuilder(args);

// 添加JWT认证服务
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    var jwtSettings = builder.Configuration.GetSection("JwtSettings");
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]))
    };
});

builder.Services.AddAuthorization();

// Add MudBlazor services
builder.Services.AddMudServices();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();


// Use in-memory database for testing and development
builder.Services.AddDbContext<GanPersonDbContext>(options =>
    options.UseInMemoryDatabase("GanPersonInMemoryDb"));

builder.Services.AddControllers(); // 添加控制器服务

builder.Services.AddScoped<DatabaseService>();
builder.Services.AddScoped<GanPersonWeb.Services.ProjectService>();
builder.Services.AddScoped<BlogService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<PersonalInfoService>();

// blazor client service
ClientServices.RegisterServices(builder.Services);

var app = builder.Build();

// 在应用启动时检查并插入默认个人信息
using (var scope = app.Services.CreateScope())
{
    var personalInfoService = scope.ServiceProvider.GetRequiredService<PersonalInfoService>();
    await personalInfoService.EnsureDefaultPersonalInfoAsync();
}
// 创建初始管理员用户
using (var scope = app.Services.CreateScope())
{
    var userService = scope.ServiceProvider.GetRequiredService<UserService>();
    await userService.CreateInitialAdminUserAsync("admin", "admin123");
}
//创建初始博客
using (var scope = app.Services.CreateScope())
{
    var blogService = scope.ServiceProvider.GetRequiredService<BlogService>();
    await blogService.CreateInitialBlogsAsync();
}
//创建初始项目
using (var scope = app.Services.CreateScope())
{
    var projectService = scope.ServiceProvider.GetRequiredService<GanPersonWeb.Services.ProjectService>();
    await projectService.CreateInitialProjectsAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// 
app.MapControllers();

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(typeof(GanPersonWeb.Client._Imports).Assembly);

app.Run();
