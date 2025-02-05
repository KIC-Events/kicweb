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

[Discord.Commands.Name("Admin")]
public class AdminModule : ModuleBase<SocketCommandContext>
{
	private readonly IServiceProvider serviceProvider;
	private readonly DiscordSocketClient client;
	private readonly IConfigurationRoot config;
	private readonly CommandHelper commandHelper;
	
	public AdminModule(IServiceProvider serviceProvider)
	{
		this.serviceProvider = serviceProvider;
		client = serviceProvider.GetRequiredService<DiscordSocketClient>();
		config = serviceProvider.GetRequiredService<IConfigurationRoot>();
		commandHelper = serviceProvider.GetRequiredService<CommandHelper>();
	}
	
	[Command("echo")]
	[Discord.Commands.Summary("Echoes a message in the specified channel.")]
	public async Task EchoAsync(string channelId, [Remainder] string message)
	{
		SocketUser user = base.Context.Message.Author;
		if(commandHelper.IsAdmin(user) == false)
		{
			await ReplyAsync("You do not have permission to use this command.");
			return;
		}
		
		ulong id = ulong.Parse(channelId);
		ISocketMessageChannel channel = serviceProvider.GetRequiredService<DiscordSocketClient>().GetChannel(id) as ISocketMessageChannel;
		await channel.SendMessageAsync(message);
	}
	
	[Command("count")]
	[Discord.Commands.Summary("Counts the number of users in the servers.")]
	public async Task CountAsync()
	{	
		SocketUser user = base.Context.Message.Author;
		if(commandHelper.IsAdmin(user) == false)
		{
			await ReplyAsync("You do not have permission to use this command.");
			return;
		}
		
		int privServUserCount = (client as DiscordSocketClient).Guilds.Where(g => g.Id == ulong.Parse(config["Servers:Private"])).Select(g => g.MemberCount).FirstOrDefault();
		int pubServUserCount = (client as DiscordSocketClient).Guilds.Where(g => g.Id == ulong.Parse(config["Servers:Public"])).Select(g => g.MemberCount).FirstOrDefault();
		
		await ReplyAsync($"Public Server User Count: {pubServUserCount}\nPrivate Server User Count: {privServUserCount}");
	}
	
	[Command("banlist")]
	[Discord.Commands.Summary("Lists all the banned users in the server. Use sparingly.")]
	public async Task BanListAsync()
	{	
		SocketUser user = base.Context.Message.Author;
		if(commandHelper.IsAdmin(user) == false)
		{
			await ReplyAsync("You do not have permission to use this command.");
			return;
		}
		
		StringBuilder sb = new StringBuilder();
		var BanListAsync = Context.Guild.GetBansAsync();
		var banListEnumerable = Discord.AsyncEnumerableExtensions.FlattenAsync(BanListAsync).ToAsyncEnumerable();
		await foreach(var banList in banListEnumerable)
		{
			foreach(var ban in banList)
			{
				sb.AppendLine(ban.User.Username + "#" + ban.User.Discriminator);
				sb.AppendLine(ban.User.Id.ToString());
				sb.AppendLine(ban.Reason);
				sb.AppendLine();
			}
		}
		await ReplyAsync(sb.ToString());
	}

}
