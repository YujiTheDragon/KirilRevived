using Discord.Commands;
using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Victoria;
using KirilRevived.Managers;
using Victoria.Node;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace KirilRevived
{
    public class Bot
    {
        private DiscordSocketClient _client;
        private CommandService _commandService;

        public Bot()
        {
            _client = new DiscordSocketClient(new DiscordSocketConfig()
            {
                LogLevel = LogSeverity.Debug,
                GatewayIntents = GatewayIntents.All
            });

            _commandService = new CommandService(new CommandServiceConfig()
            {
                LogLevel = LogSeverity.Debug,
                CaseSensitiveCommands = true,
                DefaultRunMode = RunMode.Async,
                IgnoreExtraArgs = true


            });
            var collection = new ServiceCollection();
            collection.AddSingleton(NullLogger.Instance);
            collection.AddSingleton(_client);
            collection.AddSingleton(_commandService);
            /* collection.AddLavaNode(x =>
             {
                 x.SelfDeaf = false;
                 x.Authorization = "KUREC";
                 x.Port = 2333;
                 x.Hostname = "192.168.68.100";
             });
            */
            collection.AddSingleton(new LavaNode(_client, new NodeConfiguration()
            {

                SelfDeaf = false,
                Authorization = "KUREC",
                Port = 2333,
                Hostname = "192.168.68.100"
            }, NullLogger<LavaNode>.Instance));

                
                    
            ServiceManager.SetProvider(collection);
        }

        public async Task MainAsync()
        {
            if (string.IsNullOrWhiteSpace(ConfigManager.Config.Token)) return;


            await CommandManager.LoadCommandsAsync();

            EventManager.Init(new LavaNode(_client, new NodeConfiguration()
            {

                SelfDeaf = false,
                Authorization = "KUREC",
                Port = 2333,
                Hostname = "192.168.68.100"
            }, NullLogger<LavaNode>.Instance)
                , _client, _commandService);

            await EventManager.LoadCommands();
            await _client.LoginAsync(TokenType.Bot, ConfigManager.Config.Token);
            await _client.StartAsync();

            await Task.Delay(-1);
        }


    }
}
