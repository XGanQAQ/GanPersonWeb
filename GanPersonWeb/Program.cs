using GanPersonWeb.Client.Pages;
using GanPersonWeb.Client.Services;
using GanPersonWeb.Components;
using GanPersonWeb.Data;
using GanPersonWeb.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MudBlazor.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// 添加JWT认证服务
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    var jwtSettings = builder.Configuration.GetSection("JwtSettings");
    if (jwtSettings != null)
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"])),
            NameClaimType = JwtRegisteredClaimNames.Sub,
            RoleClaimType = ClaimTypes.Role
        };
    }
    else
    {
        throw new Exception("JWT settings not found in configuration.");
    }
});

builder.Services.AddAuthorization();

// Add MudBlazor services
builder.Services.AddMudServices();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

string DevelopEnvUseMemoryDb = builder.Configuration["DevelopEnvUseMemoryDb"] ?? "true";
if (builder.Environment.IsDevelopment() && DevelopEnvUseMemoryDb=="true")
{
    // Use in-memory database for testing and development
    builder.Services.AddDbContext<GanPersonDbContext>(options =>
        options.UseInMemoryDatabase("GanPersonInMemoryDb"));
}
else
{
    // 使用SQLite数据库
    //DataSource=:memory:
    //Data Source=GanPerson.db
    builder.Services.AddDbContext<GanPersonDbContext>(options =>
        options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
}


builder.Services.AddControllers(); // 添加控制器服务

builder.Services.AddScoped<DatabaseService>();
builder.Services.AddScoped<ProjectService>();
builder.Services.AddScoped<BlogService>();
builder.Services.AddScoped<BlogCommentService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<SiteVisitService>();
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<PersonalInfoService>();
builder.Services.AddScoped<PictureBedService>();


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
    var config = builder.Configuration.GetSection("InitAdminAccount");
    var userName = string.Empty;
    var password = string.Empty;
    if (config["UserName"] != null)
    {
        userName = config["UserName"];
    }
    if (config["Password"] != null)
    {
        password = config["Password"];
    }
    if(string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
    {
        throw new Exception("InitAdminAccount configuration is missing or invalid.");
    }
    await userService.CreateInitialAdminUserAsync(userName, password);
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

app.UseAuthentication(); // 必须加上这句
app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(typeof(GanPersonWeb.Client._Imports).Assembly);

app.Run();
