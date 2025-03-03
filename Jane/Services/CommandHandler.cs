using System;
using Discord.WebSocket;
using Discord.Commands;
using Microsoft.Extensions.Configuration;
using System.Net.Sockets;
using Discord.Interactions;

namespace Jane.Services;

public class CommandHandler
{
	private readonly DiscordSocketClient _client;
	private readonly CommandService _commands;
	private readonly IServiceProvider _serviceProvider;
	private readonly IConfigurationRoot _config;
	private readonly InteractionService _interactionService;
	
	public CommandHandler(
		DiscordSocketClient client,
		CommandService commands,
		IServiceProvider serviceProvider,
		IConfigurationRoot configurationRoot,
		InteractionService interactionService)
	{
		_client = client;
		_commands = commands;
		_serviceProvider = serviceProvider;
		_config = configurationRoot;
		_interactionService = interactionService;
		
		_client.MessageReceived += HandleCommandAsync;
		_client.InteractionCreated += async (x) => 
		{
			var ctx = new SocketInteractionContext(_client, x);
			await _interactionService.ExecuteCommandAsync(ctx, _serviceProvider);
		};
	}
	
	private async Task HandleCommandAsync(SocketMessage messageParam)
	{
		if (!(messageParam is SocketUserMessage message)) return;
		if (message.Author.IsBot) return;
		
		int argPos = 0;
		if (!message.HasStringPrefix("!", ref argPos)) return;
		
		SocketCommandContext context = new(_client, message);
		var result = await _commands.ExecuteAsync(context, argPos, _serviceProvider);
		
		if (!result.IsSuccess)
			await context.Channel.SendMessageAsync(_config["Responses:Error"]);
	}
}
