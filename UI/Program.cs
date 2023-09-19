using Application;
using Microsoft.AspNetCore.HttpOverrides;
using UI.Extensions;
using UI.Extensions.Auth;
using UI.Extensions.Health;
using UI.Extensions.HttpClient;
using UI.Extensions.Localization;
using UI.Extensions.RateLimit;
using UI.Extensions.SerilogExtended;
using UI.Middlewares;

var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

var builder = WebApplication.CreateBuilder(args);

// Add Configuration from json files
builder.ReadConfigurations(env);

// ======== Register Serilog Configration ========
builder.InitSerilog(env);

// Add health app service
builder.Services.ConfigureHealthApp(builder.Configuration);

// Add health app service
builder.Services.ConfigureRateLimit(builder.Configuration);

// Add services to the container.
builder.Services.AddHttpContextAccessor();

// Add services to the container.
builder.Services.AddControllersWithViews();

// ============== Management Request Headers in Linux Host ==============
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});

// ============== allow Cors Policy ==============
builder.Services.ConfigureCorePolices();

// ============== Auth - JWT ==============
builder.Services.ConfigureAuth(builder.Configuration);

// ============== Cookie setting ==============
//builder.Services.ConfigureCookie();

// ============== Controller action Response setting ==============
//builder.Services.ConfigureResponseCaching();

// ======== Add Default Http Client Service  ========
builder.Services.ConfigureHttpClientsApp();

// ======== Add Localization Services  ========
builder.Services.ConfigureLocalization();

// ======== Add External Application Services  ========

builder.Services.AddApplicationServices(builder.Configuration);
// ============== Register Autofac  ==============
//builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory())
//    .ConfigureContainer<ContainerBuilder>(builder =>
//    {
//        builder.RegisterModule(new AutofacBusinessModule());
//    });

// ========================================= Middleware =========================================

var app = builder.Build();

// ============== Management Request Headers in Linux Host ==============
app.UseForwardedHeaders();

// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//    //app.UseExceptionHandler("/Home/Error");
//    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    //app.UseHsts();
//}

// ============== My Global Exception ==============
app.UseExceptionMiddleware();

// ============== My Global Exception ==============
app.UseMyRateLimit();

// ============== Reforce use https ============== 
//app.UseHttpsRedirection();

// ============== open dirctory path to save files ==============
app.UseStaticFiles();

// ============== enable routing for controller ==============
app.UseRouting();

// ============== allow Cors Policy ==============
app.UseCors("CorsPolicy");

// ============== Authentication & Authorization ==============
//app.UseMiddleware<JwtTokenHandlerMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

// ============== enable Localization  ==============
app.UseLocalizationMiddleware();

// ============== enable cookie setting  ==============
//app.UseCookieMiddleware();

// ============== enable Controller response caching  ==============
//app.UseResponseCaching();

// ============== Application Class Library Middleware ==============
app.UseApplicationsMiddleWares();

// ============== enable Health   ==============
app.UseHealthApp();

// ============== Request Response Handler Logging ==============
app.UseRequestResponseMiddleware();



app.MapControllerRoute(
    name: "yonetim_paneli",
    pattern: "{controller=Account}/{action=Login}/{uid:Guid?}");

await app.RunAsync();

