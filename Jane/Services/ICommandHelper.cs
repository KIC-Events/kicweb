using System;
using Discord.WebSocket;

namespace Jane.Services;

public interface ICommandHelper
{
	public bool IsAdmin(SocketUser user);
}
