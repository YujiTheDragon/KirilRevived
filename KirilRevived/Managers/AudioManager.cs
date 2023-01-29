using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Victoria;
using Victoria.Node;
using Victoria.Player;
using Victoria.Player.Decoder;
using Victoria.Resolvers;
using Victoria.WebSocket;
using Victoria.Responses;
using Victoria.Responses.Search;
using Victoria.Node.EventArgs;



namespace KirilRevived.Managers
{
    public class AudioManager
    {
        private static readonly LavaNode _lavaNode = ServiceManager.Provider.GetRequiredService<LavaNode>();



        public static async Task<string> JoinAsync(IGuild guild, IVoiceState voiceState, ITextChannel channel)
        {
            if (_lavaNode.HasPlayer(guild)) return "VE4E SUM VUTRE WE BALUK";

            if (voiceState.VoiceChannel is null)
            {
                await _lavaNode.DisconnectAsync();
                return "PURVO MA VKARAJ WE";
            }

                try
                {
                await _lavaNode.ConnectAsync();
                await _lavaNode.JoinAsync(voiceState.VoiceChannel, channel);
                return $"Vlqzoh v {voiceState.VoiceChannel.Name}";
            }
            catch (Exception ex)
            {
                return $"ERROR\n{ex.Message}";
            }


        }
        public static async Task<string> PlayAsync(SocketGuildUser user, IGuild guild, string query)
        {
            if (user.VoiceChannel is null) return "Purvo vlez v kanal purvo we";

            if (!_lavaNode.HasPlayer(guild)) return "Ne sum svurzan kum kanal";


            try
            {
                LavaPlayer<LavaTrack> player;
                _lavaNode.TryGetPlayer(guild,out player);


                LavaTrack track;
                var search = Uri.IsWellFormedUriString(query, UriKind.Absolute)
                    ? await _lavaNode.SearchAsync(SearchType.Direct, query)
                    : await _lavaNode.SearchAsync(SearchType.YouTubeMusic, query);

                if (search.Status == SearchStatus.NoMatches) return $"Ne namerih 4ep za {query}";

                track = search.Tracks.FirstOrDefault();
                if (player.Track != null && player.PlayerState is PlayerState.Playing
                    || player.PlayerState is PlayerState.Paused)
                {
                    player.Vueue.Enqueue(track);
                 
                    Console.WriteLine($"[{DateTime.Now}]\t(AUDIO)\tTrack was added to queue");
                    return $"{track.Title} has been added to queue";
                }

                await player.PlayAsync(track);
                await player.ResumeAsync();
                Console.WriteLine($"Now Playing: {track.Title}");
                return $"Now Playing: {track.Title}, with length: {track.Duration}";

            }
            catch (Exception ex)
            {
                return $"ERROR:\t{ex.Message}";
            }
        }
        public static async Task<string> SkipAsync(SocketGuildUser user, IGuild guild)
        {
            LavaPlayer<LavaTrack> player;
            _lavaNode.TryGetPlayer(guild, out player);

            if (player.PlayerState is PlayerState.Playing)

            {
                try
                {
                    if (player.Vueue.Count >= 1)
                    {
                        await player.SkipAsync();
                        Console.WriteLine($"[{DateTime.Now}]\t(AUDIO)\tTrack was Skipped");
                        return "Preska4am";
                    }
                    else
                    {
                        await player.StopAsync();
                        Console.WriteLine($"[{DateTime.Now}]\t(AUDIO)\tKraj na muzikata");
                        return "Preska4am, ama Svur6i djangura";
                    }
                }
                catch (Exception ex)
                {
                    return $"ERROR:\t{ex.Message}";
                }
            }
            else return "Nqma pesen koqto da presko4a";

        }

        public static async Task<string> NowPlayingAsync(IGuild guild)

        {
            LavaPlayer<LavaTrack> player;
            _lavaNode.TryGetPlayer(guild, out player);


            if (player.PlayerState is PlayerState.Playing)

            {
                try
                {
                    Console.WriteLine($"[{DateTime.Now}]\t(AUDIO)\tSq svirim {player.Track.Title}");
                    return $"V momenta svirq: {player.Track.Title}, i ostava: {(player.Track.Duration - player.Track.Position).ToString(@"hh\:mm\:ss")} ";
                }
                catch (Exception ex)
                {
                    return $"ERROR:\t{ex.Message}";
                }
            }
            else return "Nqma pesen det sviri";
        }

        public static async Task<string> LeaveAsync(IGuild guild)
        {
            try
            {
                LavaPlayer<LavaTrack> player;
                _lavaNode.TryGetPlayer(guild, out player);
                if (player.PlayerState is PlayerState.Playing) await player.StopAsync();
                await _lavaNode.LeaveAsync(player.VoiceChannel);
             //   await _lavaNode.DisconnectAsync();
                Console.WriteLine($"[{DateTime.Now}]\t(AUDIO)\t Bot has left a vc");
                return "TRUGVAM SI!";
            }
            catch (InvalidOperationException ex)
            {
                return $"ERROR: {ex.Message}";

            }
        }
        public static async Task<string> QueueToString(IGuild guild)
        {
            LavaPlayer<LavaTrack> player;
            _lavaNode.TryGetPlayer(guild, out player);
            var track = player.Vueue.ToArray();
            string lavalist = "";

            for (int i = 0; i < player.Vueue.Count; i++)
            {
                lavalist += "\n" + (i + 1) + ". " + track[i].Title + $" [{track[i].Duration}]";
            }
            return lavalist;

        }
        public static async Task<string> Queue(IGuild guild)
        {
            LavaPlayer<LavaTrack> player;
            _lavaNode.TryGetPlayer(guild, out player);
            if (player.Vueue.Count < 1) return "Nqma drugi pesni";

            try
            {
                Console.WriteLine($"[{DateTime.Now}]\t(AUDIO)\tOpa64ica:");
                return await QueueToString(guild);
            }
            catch (InvalidOperationException ex)
            {
                return $"ERROR: {ex.Message}";

            }

        }
        public static async Task<string> ShuffleAsync(IGuild guild)
        {
            LavaPlayer<LavaTrack> player;
            _lavaNode.TryGetPlayer(guild, out player);
            if (player.Vueue.Count < 1) return "Nqma dostatu4no pesni";
            try
            {
                Console.WriteLine($"[{DateTime.Now}]\t(AUDIO)\t ME6ENE!");
                player.Vueue.Shuffle();
                return "Me6im 6efe!";
            }
            catch (InvalidOperationException ex)
            {
                return $"ERROR: {ex.Message}";

            }
        }

        public static async Task<string> Clear(IGuild guild)
        {
            LavaPlayer<LavaTrack> player;
            _lavaNode.TryGetPlayer(guild, out player);
            if (player.Vueue.Count == 0) return "Nqmam ko da 4istq :D";
            try
            {
                Console.WriteLine($"[{DateTime.Now}]\t(AUDIO)\t 4ISTENE");
                player.Vueue.Clear();
                await player.StopAsync();
                return "4ISTIM BATE!";
            }
            catch (InvalidOperationException ex)
            {

                return $"ERROR: {ex.Message}";
            }
        }
        public static async Task TrackEnded(TrackEndEventArg<LavaPlayer<LavaTrack>,LavaTrack> args)
        {
            Console.WriteLine(args.ToString());
            if (args.Reason != TrackEndReason.Finished) return;

            if (!args.Player.Vueue.TryDequeue(out var Queueable)) return;

            if (!(Queueable is LavaTrack track))
            {
                await args.Player.TextChannel.SendMessageAsync("Next Item in the queue is not a track.");
                return;
            }
            await args.Player.PlayAsync(track);
            await args.Player.TextChannel.SendMessageAsync($"Now Playing *{track.Title} - {track.Author}*");

        }
    }
}
