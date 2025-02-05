using System;
using Discord.Commands;
using Discord.WebSocket;
using Jane.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Discord;

namespace Jane;

public class Startup
{
	public IConfigurationRoot Configuration { get; }
	
	private readonly string runtimeEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
	
	public Startup(string[] args)
	{
		IConfigurationBuilder builder = new ConfigurationBuilder()
			.SetBasePath(AppContext.BaseDirectory)
			.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
			.AddJsonFile($"appsettings.{runtimeEnvironment}.json", optional: true);
		
		Configuration = builder.Build();
	}
	
	public static async Task RunAsync(string[] args)
	{
		Startup startup = new Startup(args);
		
		await startup.RunAsync();
	}
	
	public async Task RunAsync()
	{
		var Services = new ServiceCollection();
		ConfigureServices(Services);
		
		var provider = Services.BuildServiceProvider();
		provider.GetRequiredService<BotLogger>();
		provider.GetRequiredService<CommandHandler>();
		
		try
		{
			await provider.GetRequiredService<StartupService>().StartConnectionAsync();
		}
		catch(Exception ex)
		{
			provider.GetRequiredService<BotLogger>().Log(ex);
		}
		
		try
		{
			await Task.Delay(-1);
		}
		catch(Exception ex)
		{
			provider.GetRequiredService<BotLogger>().Log(ex);
		}
	}
	
	private void ConfigureServices(IServiceCollection services)
	{
		services.AddSingleton(Configuration)
		.AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
		{
			LogLevel = LogSeverity.Verbose,
			MessageCacheSize = 1000,
			GatewayIntents = Discord.GatewayIntents.AllUnprivileged | Discord.GatewayIntents.MessageContent
		}))
		.AddSingleton(new CommandService(new CommandServiceConfig
		{
			LogLevel = LogSeverity.Verbose,
			DefaultRunMode = RunMode.Async,
			CaseSensitiveCommands = false
		}))
		.AddSingleton<BotLogger>()
		.AddSingleton<CommandHandler>()
		.AddSingleton<StartupService>()
		.AddSingleton<CommandHelper>();
	}
}
