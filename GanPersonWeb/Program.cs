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

// ���JWT��֤����
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

builder.Services.AddControllers(); // ��ӿ���������

builder.Services.AddScoped<DatabaseService>();
builder.Services.AddScoped<GanPersonWeb.Services.ProjectService>();
builder.Services.AddScoped<BlogService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<PersonalInfoService>();

// blazor client service
ClientServices.RegisterServices(builder.Services);

var app = builder.Build();

// ��Ӧ������ʱ��鲢����Ĭ�ϸ�����Ϣ
using (var scope = app.Services.CreateScope())
{
    var personalInfoService = scope.ServiceProvider.GetRequiredService<PersonalInfoService>();
    await personalInfoService.EnsureDefaultPersonalInfoAsync();
}
// ������ʼ����Ա�û�
using (var scope = app.Services.CreateScope())
{
    var userService = scope.ServiceProvider.GetRequiredService<UserService>();
    await userService.CreateInitialAdminUserAsync("admin", "admin123");
}
//������ʼ����
using (var scope = app.Services.CreateScope())
{
    var blogService = scope.ServiceProvider.GetRequiredService<BlogService>();
    await blogService.CreateInitialBlogsAsync();
}
//������ʼ��Ŀ
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
