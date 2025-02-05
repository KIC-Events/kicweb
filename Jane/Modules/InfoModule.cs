using System;
using System.Text;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Jane.Services;
using KiCData.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Jane.Modules;

[Discord.Commands.Name("Info")]
public class InfoModule : ModuleBase<SocketCommandContext>
{
	private readonly IServiceProvider serviceProvider;
	private readonly CommandService commandService;
	private readonly CommandHelper commandHelper;
	
	public InfoModule(IServiceProvider serviceProvider)
	{
		this.serviceProvider = serviceProvider;
		commandService = serviceProvider.GetRequiredService<CommandService>();
		commandHelper = serviceProvider.GetRequiredService<CommandHelper>();
	}
	
	[Command("state")]
	[Discord.Commands.Summary("Displays the current state of the bot.")]
	[Alias("info", "jane", "whoareyou")]	
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
		await ReplyAsync(message);
	}
	
	[Command("date")]
	[Discord.Commands.Summary("Displays the current date and time.")]
	public async Task DateAsync()
	{
		await ReplyAsync($"The current date and time is {DateTime.Now}.");
	}
	
	[Command("list")]
	[Discord.Commands.Summary("Lists all available commands.")]
	[Alias("commands", "help")]
	public async Task ListAsync()
	{	
		SocketUser user = base.Context.Message.Author;
		bool isAdmin = commandHelper.IsAdmin(user);
		
		string message;
		StringBuilder sb = new();
		sb.AppendLine("Here are the available commands:");
		foreach(Discord.Commands.ModuleInfo module in commandService.Modules)
		{
			if(module.Name == "Admin" && !isAdmin)
			{
				continue;
			}
			
			sb.AppendLine($"**{module.Name}**");
			foreach(CommandInfo command in module.Commands)
			{
				sb.AppendLine($"`{command.Name}`: {command.Summary}");
			}
		}
		message = sb.ToString();
		await ReplyAsync(message);
	}
}
