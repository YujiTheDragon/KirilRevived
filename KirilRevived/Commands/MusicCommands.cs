using Discord;
using Discord.Commands;
using Discord.WebSocket;
using KirilRevived.Managers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Victoria;
using Victoria.Node;


namespace KirilRevived.Commands
{
    [Name("Music")]
    public class MusicCommands : ModuleBase<SocketCommandContext>
    {
        private static readonly LavaNode _lavaNode = ServiceManager.Provider.GetRequiredService<LavaNode>();
        [Command("join")]
        [Summary("Kara me da vlqza, pedal")]
        public async Task JoinCommand()
        {
            Console.WriteLine("GEJ");
            await Context.Channel.SendMessageAsync(await AudioManager.JoinAsync(Context.Guild, Context.User as IVoiceState, Context.Channel as ITextChannel));
        }

        [Command("play")]
        [Alias("p", "igraj", "puskaj", "pusni")]
        [Summary("6te svirq kot kaji6!")]
        public async Task PlayCommand([Remainder] string search)
        {
            Console.WriteLine("4ep");
            if (_lavaNode.HasPlayer(Context.Guild)) await Context.Channel.SendMessageAsync(await AudioManager.PlayAsync(Context.User as SocketGuildUser,
                 Context.Guild, search));
            else
            {
                await Context.Channel.SendMessageAsync(await AudioManager.JoinAsync(Context.Guild, Context.User as IVoiceState, Context.Channel as ITextChannel));
                await Context.Channel.SendMessageAsync(await AudioManager.PlayAsync(Context.User as SocketGuildUser,
                 Context.Guild, search));
            }
        }

        [Command("Shuffle")]
        [Alias("shuffle", "shuf", "me6i")]
        [Summary("Me6im!")]
        public async Task Shuffle()

        {
            await Context.Channel.SendMessageAsync(await AudioManager.ShuffleAsync(Context.Guild));
        }

        [Command("skip")]
        [Alias("s", "skok", "sashoegej","S")]
        [Summary("Skok!")]
        public async Task SkipCommand()
        {
            await Context.Channel.SendMessageAsync(await AudioManager.SkipAsync(Context.User as SocketGuildUser, Context.Guild));
            await Context.Channel.SendMessageAsync(await AudioManager.NowPlayingAsync(Context.Guild));
        }

        [Command("leave")]
        [Alias("mahaj se", "l", "begi")]
        [Summary("Trugvam si!")]
        public async Task LeaveCommand()
            => await Context.Channel.SendMessageAsync(await AudioManager.LeaveAsync(Context.Guild));

        [Command("Now Playing")]
        [Alias("np", "Np", "NP", "nP")]
        [Summary("6te svirq kot kaji6!")]

        public async Task NowPlaying()
        {

            await Context.Channel.SendMessageAsync(await AudioManager.NowPlayingAsync(Context.Guild));
        }

        [Command("Queue")]
        [Alias("q", "opa6ka", "queue", "mariogej")]
        [Summary("6te svirq kot kaji6!")]
            public async Task Queue()
        {
            await Context.Channel.SendMessageAsync("Pesni v opa64icata:\n" + await AudioManager.Queue(Context.Guild));
        }
        [Command("Clear")]
        [Alias("c", "C", "clear", "CLEAR")]
        [Summary("4istim!")]
        public async Task Clear()
        {
            await Context.Channel.SendMessageAsync( await AudioManager.Clear(Context.Guild));
        }
        [Command("Winged Hussars")]
        [Alias("winged hussars", "wingedhussars", "WingedHussars", "WH")]
        [Summary("IDVAAAAAAAAAAAAT!")]
        public async Task WingedHussars()
        {
            await Context.Channel.SendMessageAsync("IDVAAAAAAAAAAAAAAAAAAAAAAAAAAT!\n");
            await Context.Channel.SendMessageAsync("https://media.discordapp.net/attachments/438037039917826052/982206643636961311/unknown.png?width=1167&height=701");
            await PlayCommand("https://www.youtube.com/watch?v=rcYhYO02f98&ab_channel=Sabaton");
        }

    }
}

