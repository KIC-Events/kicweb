using System;
using KiCData.Services;
using Discord;
using Discord.WebSocket;
using Discord.Commands;

namespace Jane.Services;

public class BotLogger : KiCLogger
{
	public BotLogger(DiscordSocketClient client, CommandService command)
	{
		client.Log += LogAsync;
		command.Log += LogAsync;
	}
	
	private Task LogAsync(LogMessage log)
	{
		string message = $"{log.Source}: {log.Message}";
		LogText(message);
		return Task.CompletedTask;
	}
}
