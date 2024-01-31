using Discord.WebSocket;
using log4net;
using System.Diagnostics;
using System.Reflection;

namespace BoTflix
{
    public static class Helper
    {
        public static readonly string statusLink = "https://www.twitch.tv/vince_zder";
        public static readonly string _zderLandIconUrl = "https://cdn.discordapp.com/attachments/494958624922271745/1056847373436977162/brookByVince.gif";
        public static readonly string _JellyfinImgUrl = "https://cdn.discordapp.com/attachments/617462663374438411/1106597286630391890/bob.gif";
        public static readonly string _JellyfinGif = "https://cdn.discordapp.com/attachments/617462663374438411/1106926689763725422/loading.gif";
        public static readonly string _urlAvatarVince = "https://cdn.discordapp.com/attachments/617462663374438411/846821971114983474/luffy.gif";
        public static readonly ulong _ZderLandId = 312966999414145034;
        public static readonly ulong _idJellyfinRole = 816282726654279702;
        public static readonly ulong _idModoRole = 322489502562123778;
        public static readonly ulong _idGeneralChannel = 312966999414145034;
        public static readonly ulong _idJellyfinChannel = 816283362478129182;
        public static readonly ulong _idOnePieceChannel = 553256709439750151;
        public static readonly ulong _idSaloonVoice = 493036345686622210;
        private static readonly string _zderLandId = Environment.GetEnvironmentVariable("ZderLandId");
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly string _coinEmote = "<a:Coin:637802593413758978>";
        private static readonly string _doneEmote = "<a:check:626017543340949515>";
        private static readonly string _arrowEmote = "<a:arrow:830799574947463229>";
        private static readonly string _alarmEmote = "<a:alert:637645061764415488>";
        private static readonly string _coeurEmote = "<a:coeur:830788906793828382>";
        private static readonly string _bravoEmote = "<a:bravo:626017180731047977>";
        private static readonly string _luffyEmote = "<a:luffy:863101041498259457>";
        private static readonly string _verifiedEmote = "<a:verified:773622374926778380>";
        private static readonly string _pikachuEmote = "<a:hiPikachu:637802627345678339>";
        private static readonly string _pepeSmokeEmote = "<a:pepeSmoke:830799658354737178>";
        private static readonly string _pepeMdrEmote = "<a:pepeMDR:912738745105674292>";
        private static readonly string _heheEmote = "<a:hehe:773622227064979547>";
        private static readonly string _coeurEmoji = "\u2764";
        private static readonly string _tvEmoji = "\uD83D\uDCFA";
        private static readonly string _dlEmoji = "<:DL:894171464167747604>";

        private static readonly List<string> _greetings = new List<string>
        {
            "good day","salutations","hey","oh les bg !","petites cailles bonjour","ciao a tutti", "insérer une phrase cool",
            "konnichi wa","'sup, b?","what's poppin'?","greetings","What's in the bag?","sup","wussup?","how ya goin?",
            "what's the dizzle?","good morning","what's cracking?","quoi de neuf la cité ?","whazzup?","guten Tag",
            "EDGAAAAAAR","good afternoon","hola","hello","coucou !","what's the dilly?","très heureux d'être là",
            "wassap?","what's the rumpus?","what's crackin'?","how do?","yello","what's up?","c'est moi que revoilà !",
            "on est pas pressé, mais moi oui","what's new?","what's shaking?","howzit?","good night","hola","ahoy",
            "aloha","how's it hanging?","howsyamomanem?","how goes it?","good evening","yo","how's it going?",
            "ça dit quoi les filles ?", "Ah ! Toujours là ce bon vieux Denis","what's cooking?", "invocation !"
        };        

        
        internal static void StartProcess(string path)
        {
            using (var process = new Process())
            {
                process.StartInfo = new ProcessStartInfo
                {
                    FileName = path,
                    WindowStyle = ProcessWindowStyle.Normal,
                    UseShellExecute = true
                };

                process.Start();
            }
        }        

        internal static ISocketMessageChannel GetSocketMessageChannel(DiscordSocketClient client, ulong channelId)
        {
            var channels = GetAllChannels(client);

            ISocketMessageChannel channel = (ISocketMessageChannel)channels.FirstOrDefault(x => x.Id == channelId);

            if (channel == null) log.Error($"GetSocketMessageChannelContains : no channel {channelId}");

            return channel;
        }

        internal static bool IsJellyfinCorrectChannel(ISocketMessageChannel channel)
        {
            //return channel.Name.ToLower().Contains("jellyfin");
            return channel.Id == _idJellyfinChannel;
        }

        /// <summary>
        /// Return SocketGuild as ZderLand
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        internal static SocketGuild GetZderLand(DiscordSocketClient client)
        {
            return client.Guilds.FirstOrDefault(); // in prod the bot is strictly connected to Zderland            
        }

        internal static IEnumerable<SocketGuildChannel> GetAllChannels(DiscordSocketClient client)
        {
            SocketGuild guild = GetZderLand(client);
            var channels = guild.Channels.ToList();

            return channels;
        }
        
        internal static string GetGreeting()
        {
            Random random = new Random();
            string res = _greetings[random.Next(_greetings.Count)];

            //First letter Uper
            return res.First().ToString().ToUpper() + res.Substring(1);
        }

        internal static bool IsSundayToday() { return DateTime.Now.DayOfWeek == DayOfWeek.Sunday; }        
        

        #region Get Emoji/Emote
        public static string GetCoinEmote() { return _coinEmote; }
        public static string GetCoeurEmote() { return _coeurEmote; }
        public static string GetVerifiedEmote() { return _verifiedEmote; }
        public static string GetPikachuEmote() { return _pikachuEmote; }
        public static string GetAlarmEmote() { return _alarmEmote; }
        public static string GetBravoEmote() { return _bravoEmote; }
        public static string GetArrowEmote() { return _arrowEmote; }
        public static string GetDoneEmote() { return _doneEmote; }
        public static string GetPepeSmokeEmote() { return _pepeSmokeEmote; }
        public static string GetPepeMdrEmote() { return _pepeMdrEmote; }
        public static string GetHeheEmote() { return _heheEmote; }
        public static string GetLuffyEmote() { return _luffyEmote; }
        public static string GetCoeurEmoji() { return _coeurEmoji; }
        public static string GetTvEmoji() { return _tvEmoji; }
        public static string GetDlEmoji() { return _dlEmoji; }

        internal static string GetZderLandIconUrl()
        {
            return _zderLandIconUrl;
        }
        internal static string GetZderLandId()
        {
            return _zderLandId;
        }
        #endregion
    }
}
