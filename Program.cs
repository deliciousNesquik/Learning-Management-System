using LMS.Components;
using LMS.Data;
using LMS.Data.Models;
using LMS.Interfaces;
using LMS.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// 2. Регистрация сервисов
builder.Services.AddControllers(); 
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddCascadingAuthenticationState(); 

builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IModeratorService, ModeratorService>();
builder.Services.AddScoped<IOrganizationService, OrganizationService>();
builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();
builder.Services.AddScoped<IBranchService, BranchService>();
builder.Services.AddScoped<IAuditHistoryService, AuditHistoryService>();
builder.Services.AddScoped<IDirectorService, DirectorService>();


builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<UserSessionAccessor>();
builder.Services.AddScoped<UserPermissions, UserPermissions>();
builder.Services.AddScoped<PostgresConnectionInterceptor>();


builder.Services.AddDbContextFactory<DatabaseContext>((sp, options) =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection"), 
        o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
    
    var interceptor = sp.GetRequiredService<PostgresConnectionInterceptor>();
    options.AddInterceptors(interceptor);
}, ServiceLifetime.Scoped);


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.AccessDeniedPath = "/access-denied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.Cookie.HttpOnly = true; // Защита от JS-кражи
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });

builder.Services.AddAuthorization();

// 5. Кастомные сервисы
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<IPasswordHasher<Administrator>, PasswordHasher<Administrator>>();
builder.Services.AddScoped<IPasswordHasher<Moderator>, PasswordHasher<Moderator>>();
builder.Services.AddScoped<IPasswordHasher<Employee>, PasswordHasher<Employee>>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapControllers(); 
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();