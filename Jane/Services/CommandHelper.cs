using System;
using Discord;
using Discord.WebSocket;

namespace Jane.Services;

public class CommandHelper : ICommandHelper
{
	public bool IsAdmin(SocketUser user)
	{
		if(user is not SocketGuildUser guildUser || !guildUser.GuildPermissions.Administrator)
		{
			return false;
		}
		return true;
	}
}
