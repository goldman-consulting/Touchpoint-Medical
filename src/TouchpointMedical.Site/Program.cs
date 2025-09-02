using Destructurama;

using Newtonsoft.Json;

using Serilog;

using TouchpointMedical.Extensions;
using TouchpointMedical.Integration.PointClickCare.Services;
using TouchpointMedical.Services;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

builder.Logging.AddFilter("Microsoft.AspNetCore.Server.Kestrel", LogLevel.Debug);
builder.Logging.AddFilter("System.Net.Security", LogLevel.Debug);

//Setup Data Directory to use App_Data
var baseDir = builder.Environment.ContentRootPath;
AppDomain.CurrentDomain.SetData("DataDirectory", Path.Combine(baseDir, "App_Data"));

//Configure Touchpoint (TouchpointMedical)
builder.Services.AddTouchpoint(builder.Configuration);

//Configure PointClickCare (TouchpointMedical.Integrations.PointClickCare)
builder.Services.AddPointClickCare(builder.Configuration);

builder.Services.Configure<HostOptions>(o =>
{
    o.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
    o.ShutdownTimeout = TimeSpan.FromSeconds(10);
});

// Configure Serilog
builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Destructure.UsingAttributes();
});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddRouting();

builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson(options => 
    {
        options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("UAT-Gateway"))
{
    app.UseRawBodyBuffering(opts => 
    {
        opts.PathPrefixes = ["/api"];
        opts.ShouldStashInHttpContextItems = true;
    });
    
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//Setup Default Http Headers
app.UseSecurityHeaders(new HeaderPolicyCollection()
    .AddFrameOptionsDeny()
    .AddCustomHeader("X-Content-Type-Options", "nosniff")
    .AddReferrerPolicyStrictOriginWhenCrossOrigin()
    .AddStrictTransportSecurityMaxAgeIncludeSubDomains()
    .AddContentSecurityPolicy(builder =>
    {
        builder.AddDefaultSrc().Self();
        builder.AddScriptSrc().Self().WithNonce();
        builder.AddStyleSrc().Self().UnsafeInline();
        builder.AddImgSrc().Self().Data();
        builder.AddFontSrc().Self();
        builder.AddConnectSrc().Self();
    })
    .AddPermissionsPolicy(builder =>
    {
        builder.AddGeolocation().None();
        builder.AddCamera().None();
        builder.AddMicrophone().None();
    })
    .AddCustomHeader("X-Permitted-Cross-Domain-Policies", "none")
    .AddCrossOriginOpenerPolicy(builder => builder.SameOrigin())
    .AddCrossOriginEmbedderPolicy(builder => builder.RequireCorp())
    .AddCrossOriginResourcePolicy(builder => builder.SameOrigin())
    );

app.Use(async (context, next) =>
{
    context.Response.OnStarting(() =>
    {
        context.Response.Headers.Remove("Server");
        context.Response.Headers.Remove("X-Powered-By");
        context.Response.Headers.Remove("ETag");
        context.Response.Headers.Remove("X-AspNet-Version");
        context.Response.Headers.Remove("X-AspNetMvc-Version");

        return Task.CompletedTask;
    });

    await next.Invoke();
});

app.UseStaticFiles();
app.UseForwardedHeaders();
app.UseHttpsRedirection();

app.UseRouting();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

//Load From Cookie
app.UseAuthentication();

app.UseAuthorization();

app.UseCors("AllowSpecificOrigin");
    
app.Run();
