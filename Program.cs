using KiCWeb.Configuration;
using KiCData.Services;
using KiCData.Models;
using KiCData.Models.WebModels;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

IConfigurationBuilder configBuilder = new ConfigurationBuilder()
	.AddJsonFile("appsettings.json");
if(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production")
{
	configBuilder.AddJsonFile("appsettings.Production.json");
}
else if(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
{
	configBuilder.AddJsonFile("appsettings.Development.json");
}
else { throw new Exception("Bad environment variable."); }
IConfigurationRoot config = configBuilder.Build();

// Add services to the container.
builder.Services.AddSingleton(config);
DbContextOptionsBuilder<KiCdbContext> dbOptionsBuilder = new DbContextOptionsBuilder<KiCdbContext>();
dbOptionsBuilder.UseMySql(config["Database:ConnectionString"], ServerVersion.AutoDetect(config["Database:ConnectionString"]));
DbContextOptions<KiCdbContext> options = dbOptionsBuilder.Options;
builder.Services.AddSingleton<KiCdbContext>(new KiCdbContext(options));
builder.Services.AddSingleton<IEmailService, EmailService>();
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<ICookieService, CookieService>();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<IKiCLogger, KiCLogger>();
builder.Services.AddHttpClient<IEmailService, EmailService>(client =>
{
	client.BaseAddress = new Uri(config["Base Addresses:Mail"]);
});
builder.Services.AddSingleton<PaymentService, PaymentService>();
builder.Services.AddSingleton<InternalPaymentService, InternalPaymentService>();
builder.Services.AddControllersWithViews();
var featureFlags = builder.Configuration
    .GetSection("FeatureFlags")
    .Get<FeatureFlags>() ?? new FeatureFlags();

builder.Services.AddSingleton(featureFlags);

WebApplication app = builder.Build();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
	ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor | Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseStatusCodePages();
app.UseRouting();
app.UseAuthorization();
app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
