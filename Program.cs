using KiCWeb.Configuration;
using KiCData.Services;
using KiCData.Models;
using Microsoft.EntityFrameworkCore;
using Hangfire;
using Hangfire.MySql;
using KiCWeb.Helpers;

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

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Add services to the container.
builder.Services.AddSingleton(config);
DbContextOptionsBuilder<KiCdbContext> dbOptionsBuilder = new DbContextOptionsBuilder<KiCdbContext>();
dbOptionsBuilder.UseMySql(config["Database:ConnectionString"], ServerVersion.AutoDetect(config["Database:ConnectionString"]));
DbContextOptions<KiCdbContext> options = dbOptionsBuilder.Options;
builder.Services.AddSingleton<KiCdbContext>(new KiCdbContext(options));
builder.Services.AddSingleton<IEmailService, SESEmailService>();
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<ICookieService, CookieService>();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<IKiCLogger, KiCLogger>();
//builder.Services.AddHttpClient<IEmailService, SESEmailService>(client =>
//{
//	client.BaseAddress = new Uri(config["Base Addresses:Mail"]);
//});
builder.Services.AddSingleton<PaymentService, PaymentService>();
builder.Services.AddSingleton<RegistrationSessionService, RegistrationSessionService>();
builder.Services.AddSingleton<InventoryService, InventoryService>();
builder.Services.AddControllersWithViews();
builder.Services.AddHangfire((sp, config) =>
	{
		var connectionString = sp.GetRequiredService<IConfiguration>()["Database:ConnectionString"];
		config.UseStorage(
			new MySqlStorage(
				connectionString,
				new MySqlStorageOptions
				{
					TablesPrefix = "Hangfire"
				}
			)
		);
	}
);
builder.Services.AddHangfireServer();

var featureFlags = builder.Configuration
    .GetSection("FeatureFlags")
    .Get<FeatureFlags>() ?? new FeatureFlags();

builder.Services.AddSingleton(featureFlags);

builder.Services.AddSession(options =>
{
	options.Cookie.Name = "kic_session";
	options.Cookie.IsEssential = true;
	options.IdleTimeout = TimeSpan.FromMinutes(30);
	options.Cookie.HttpOnly = true;
});

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
app.UseSession();
app.UseRouting();
app.UseAuthorization();
app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");
if (app.Environment.IsDevelopment())
{
	// Allow more relaxed dashboard access if the app is running in dev mode
	app.UseHangfireDashboard("/hangfire", new DashboardOptions
	{
		Authorization = new [] { new HangfireAuthFilter() },
		IgnoreAntiforgeryToken = true
	});
}
else
{
	// Hangfire dashboard only accessible locally in production mode
	app.UseHangfireDashboard();
}

app.Run();
