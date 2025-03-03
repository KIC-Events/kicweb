using System;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using Discord.Interactions;
using Square.Models;

namespace Jane.Services;

public class StartupService
{
	private readonly IServiceProvider serviceProvider;
	private readonly DiscordSocketClient client;
	private readonly CommandService commands;
	private readonly IConfigurationRoot config;
	private readonly BotLogger botLogger;
	private readonly InteractionService interactionService;
	
	public StartupService(IServiceProvider serviceProvider, DiscordSocketClient client, CommandService commands, IConfigurationRoot config, BotLogger botLogger, InteractionService interactionService)
	{
		this.serviceProvider = serviceProvider;
		this.client = client;
		this.commands = commands;
		this.config = config;
		this.botLogger = botLogger;
		this.interactionService = interactionService;
	}
	
	public async Task StartConnectionAsync()
	{
		client.Ready += Announce;
		client.Ready += async () => await interactionService.RegisterCommandsGloballyAsync();
		
		string token = config["BotToken"];
		if(string.IsNullOrWhiteSpace(token))
		{
			botLogger.Log(new Exception("Bot token is missing from the configuration file."));
			return;
		}
		
		try
		{
			await client.LoginAsync(TokenType.Bot, token);
		}
		catch(Exception ex)
		{
			botLogger.Log(ex);
			return;
		}
		
		try
		{
			await client.StartAsync();
		}
		catch(Exception ex)
		{
			botLogger.Log(ex);
			return;
		}
		
		commands.AddTypeReader(typeof(bool), new BooleanTypeReader());
		
		try
		{
			await commands.AddModulesAsync(GetType().Assembly, serviceProvider);
			await interactionService.AddModulesAsync(GetType().Assembly, serviceProvider);
		}
		catch(Exception ex)
		{
			botLogger.Log(ex);
			return;
		}
	}
	
	private async Task Announce()
	{
		ISocketMessageChannel channel = client.GetChannel(ulong.Parse(config["Channels:Testing"])) as ISocketMessageChannel;
		
		string message;		
		StringBuilder sb = new();		
		sb.AppendLine("I'm online and ready to serve!");		
		string env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
		sb.AppendLine($"Environment: {env}");
		string machine = Environment.MachineName;
		sb.AppendLine($"Machine: {machine}");
		int privServUserCount = (client as DiscordSocketClient).Guilds.Where(g => g.Id == ulong.Parse(config["Servers:Private"])).Select(g => g.MemberCount).FirstOrDefault();
		int pubServUserCount = (client as DiscordSocketClient).Guilds.Where(g => g.Id == ulong.Parse(config["Servers:Public"])).Select(g => g.MemberCount).FirstOrDefault();
		sb.AppendLine("Public Server User Count: " + pubServUserCount);
		sb.AppendLine("Private Server User Count: " + privServUserCount);
		message = sb.ToString();
		
		try
		{
			await channel.SendMessageAsync(message);
		}
		catch(Exception ex)
		{
			botLogger.Log(ex);
		}
	}
}

internal class BooleanTypeReader : Discord.Commands.TypeReader
{
	   public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
		{
			bool result;
			if (bool.TryParse(input, out result))
			{
				return Task.FromResult(TypeReaderResult.FromSuccess(result));
			}

			return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "Could not parse input as a boolean."));
		}
}