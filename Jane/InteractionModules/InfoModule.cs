using System;
using Discord.Interactions;
using Discord.WebSocket;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Jane.InteractionModules;

public class InfoModule : InteractionModuleBase
{
	private readonly IServiceProvider serviceProvider;
	
	public InfoModule(IServiceProvider serviceProvider)
	{
		this.serviceProvider = serviceProvider;
	}
	
	[SlashCommand("state", "Displays the current state of the bot.")]
	public async Task StateAsync()
	{
		string message;		
		StringBuilder sb = new();		
		sb.AppendLine("I'm online and ready to serve!");		
		string env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
		sb.AppendLine($"I'm currently running in {env} mode.");
		string machine = Environment.MachineName;
		sb.AppendLine($"I'm running on {machine}.");
		SocketUser jane = serviceProvider.GetRequiredService<DiscordSocketClient>().CurrentUser;
		sb.AppendLine($"I'm <@{jane.Id}>, or Just Another Nonhuman Entity, a Discord bot used by KIC to help manage the Discord environment. I'm here to help!");
		SocketUser user = serviceProvider.GetRequiredService<DiscordSocketClient>().GetUser(ulong.Parse(serviceProvider.GetRequiredService<IConfigurationRoot>()["Users:Malechus"]));
		sb.AppendLine($"I was created by <@{user.Id}> to be a helpful assistant to the community. If I am not doing so, or if there is any way I can do that better, please let him know.");
		message = sb.ToString();
		await RespondAsync(message, ephemeral: true);
	}
}
