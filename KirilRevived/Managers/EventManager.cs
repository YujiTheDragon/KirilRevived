using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Victoria.Node;

namespace KirilRevived.Managers
{
    public static class EventManager
    {

        private static LavaNode _lavaNode;
        private static DiscordSocketClient _client;
        private static CommandService _commandService;

        public static void Init(LavaNode lava, DiscordSocketClient client, CommandService comm)
        {
            _lavaNode = lava;
            _client = client;
            _commandService = comm;

        }
        // _lavaNode = ServiceManager.Provider.GetRequiredService<LavaNode>();
        // _client = ServiceManager.GetService<DiscordSocketClient>();
        // _commandService = ServiceManager.GetService<CommandService>();

        public static Task LoadCommands()
        {
            _client.Log += message =>
                {
                    Console.WriteLine($"[{DateTime.Now}]\t({message.Source})\t{message.Message}");
                    return Task.CompletedTask;
                };

            _commandService.Log += message =>
            {
                Console.WriteLine($"[{DateTime.Now}]\t({message.Source})\t{message.Message}");
                return Task.CompletedTask;
            };


            _client.Ready += OnReady;

            _client.MessageReceived += OnMessageReceived;
            return Task.CompletedTask;

        }

        private static async Task OnMessageReceived(SocketMessage arg)
        {

            var message = arg as SocketUserMessage;
            var context = new SocketCommandContext(_client, message);

            if (message.Author.IsBot || message.Channel is IDMChannel) return;

            var argPos = 0;
            bool PrefixCheck = message.HasStringPrefix(ConfigManager.Config.Prefix, ref argPos);
            bool PrefixMentionCheck = message.HasMentionPrefix(_client.CurrentUser, ref argPos);

            if (!(PrefixCheck || PrefixMentionCheck))
            {
                Console.WriteLine("GEJ");
                return;
            }

            var result = await _commandService.ExecuteAsync(context, argPos, ServiceManager.Provider);

            if (!result.IsSuccess)
            {
                if (result.Error == CommandError.UnknownCommand) return;
            }

        }
        private static async Task OnReady()
        {
            try
            {
                _lavaNode.OnTrackEnd += AudioManager.TrackEnded;
                await _lavaNode.ConnectAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            Console.WriteLine($"[{DateTime.Now}]\t(READY)\tBot is ready");
            await _client.SetStatusAsync(UserStatus.Online);
            await _client.SetGameAsync($"Prefix:{ConfigManager.Config.Prefix}", null, ActivityType.Listening);

        }
    }
}
