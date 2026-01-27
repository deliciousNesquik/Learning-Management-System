using Amazon.S3;
using DotNetEnv;
using LMS.Components;
using LMS.Data;
using LMS.Data.Configuration;
using LMS.Data.Entities;
using LMS.DTOs.Storage;
using LMS.Interfaces;
using LMS.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

Env.Load();
builder.Configuration.AddEnvironmentVariables();

builder.Services.Configure<S3Options>(options =>
{
    options.AccessKey = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY") ?? "";
    options.SecretKey = Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY") ?? "";
    options.ServiceUrl = Environment.GetEnvironmentVariable("AWS_SERVICE_URL") ?? "";
    options.BucketName = Environment.GetEnvironmentVariable("AWS_BUCKET_NAME") ?? "";
    options.Region = Environment.GetEnvironmentVariable("AWS_REGION") ?? "";
});

builder.Services.AddSingleton<IAmazonS3>(sp =>
{
    var options = sp.GetRequiredService<IOptions<S3Options>>().Value;
    return new AmazonS3Client(options.AccessKey, options.SecretKey, new AmazonS3Config
    {
        ServiceURL = options.ServiceUrl,
        ForcePathStyle = true
    });
});




// Настройка драйвера для игнорирования строгой проверки временных типов данных.
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

builder.Services.AddControllers(); 
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddCascadingAuthenticationState(); 

// Добавление сервисов приложения.
builder.Services.AddScoped<IFileStorageService, S3StorageService>();

builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IModeratorService, ModeratorService>();
builder.Services.AddScoped<IOrganizationService, OrganizationService>();
builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();
builder.Services.AddScoped<IBranchService, BranchService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IAuditHistoryService, AuditHistoryService>();
builder.Services.AddScoped<IDirectorService, DirectorService>();
builder.Services.AddScoped<IStorageService, LocalStorageService>();

builder.Services.AddScoped<IUserSecurityService, UserSecurityService>();

// Добавление сервисов для авторизации в приложении.
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<UserRequestContext>();
builder.Services.AddScoped<UserPermissions, UserPermissions>();
builder.Services.AddScoped<PostgresConnectionInterceptor>();


builder.Services.AddDbContextFactory<DatabaseContext>((sp, options) =>
{
    options.UseNpgsql(Environment.GetEnvironmentVariable("CONNECTION_STRING") ?? "", 
        o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
    
    /*options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection"), 
        o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));*/
    
    var interceptor = sp.GetRequiredService<PostgresConnectionInterceptor>();
    options.AddInterceptors(interceptor);
}, ServiceLifetime.Scoped);


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.AccessDeniedPath = "/access-denied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });

builder.Services.AddAuthorization();

// Настройка инъекций моделей для приложения
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

app.MapScalarApiReference();

app.Run();