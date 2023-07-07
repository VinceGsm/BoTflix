using Discord;
using Discord.Rest;
using Discord.WebSocket;
using log4net;
using System.Reflection;

namespace BoTflix.Service
{
    public class MessageService
    {
        public DiscordSocketClient _client;
        private const long _vinceId = 312317884389130241;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public MessageService(DiscordSocketClient client)
        {
            _client = client;
            _client.MessageReceived += MessageReceived;
        }

        #region Client
        private Task MessageReceived(SocketMessage arg)
        {
            //DM from User
            if (arg.Source == MessageSource.User && arg.Channel.Name.StartsWith('@'))
            {
                string message = $"<@{arg.Author.Id}> *says* : " + arg.Content;
                SendToLeader(message);
                AddReactionRobot((SocketUserMessage)arg);
            }

            return Task.CompletedTask;
        }
        #endregion

        #region Reaction
        public async Task AddReactionVu(SocketUserMessage message)
        {
            // --> 👀
            Emoji vu = new Emoji("\uD83D\uDC40");
            await message.AddReactionAsync(vu);
        }

        public async Task AddReactionRefused(SocketUserMessage message)
        {
            // --> ❌
            Emoji cross = new Emoji("\u274C");
            await message.AddReactionAsync(cross);
        }

        public async Task AddReactionRobot(SocketUserMessage message)
        {
            // --> 🤖
            Emoji robot = new Emoji("\uD83E\uDD16");
            await message.AddReactionAsync(robot);
        }

        public async Task AddReactionAlarm(SocketUserMessage message)
        {
            var alarm = Emote.Parse(Helper.GetAlarmEmote());
            await message.AddReactionAsync(alarm);
        }

        public async Task AddReactionBirthDay(IMessage message)
        {
            var bravo = Emote.Parse(Helper.GetBravoEmote());
            // --> 🎂
            Emoji cake = new Emoji("\uD83C\uDF82");

            await message.AddReactionAsync(cake);
            await message.AddReactionAsync(bravo);
        }

        public async Task AddDoneReaction(SocketUserMessage message)
        {
            await message.RemoveAllReactionsAsync();

            var check = Emote.Parse(Helper.GetDoneEmote());
            await message.AddReactionAsync(check);
        }
        #endregion

        #region Message
        internal void SendToLeader(string message)
        {
            var leader = _client.GetUser(_vinceId);
            leader.SendMessageAsync(message);
        }

        #region Control Message
        internal async Task CommandNotAuthorizeHere(ISocketMessageChannel channel, MessageReference reference, ulong idChannelWhereLegit)
        {
            await channel.SendMessageAsync($"L'utilisation de cette commande est limitée au channel <#{idChannelWhereLegit}>", messageReference: reference);
        }

        internal async Task CommandForbidden(ISocketMessageChannel channel, MessageReference reference)
        {
            await channel.SendMessageAsync($"L'utilisation de cette commande est interdite !", messageReference: reference);
        }

        internal async Task SendJellyfinNotAuthorizeHere(ISocketMessageChannel channel, MessageReference reference)
        {
            await channel.SendMessageAsync($"⚠️ Pour des raisons de sécurité l'utilisation de Jellyfin" +
                $" est limitée au channel <#816283362478129182>", messageReference: reference);
        }

        internal async Task SendJellyfinAlreadyInUse(ISocketMessageChannel channel, MessageReference reference)
        {
            await channel.SendMessageAsync($"Attention Jellyfin est déjà en cours d'utilisation ! Merci de regarder les PINS", messageReference: reference);
        }

        internal async Task JellyfinNotAvailable(ISocketMessageChannel channel, MessageReference reference)
        {
            await channel.SendMessageAsync($"La base de donnée est indisponible pour le moment.\n " +
                $"Pour rappel, /ping mets à jour mon statut", messageReference: reference);
        }
        #endregion
        #endregion        

        #region Embed
        /// <summary>
        /// Message Embed with link
        /// </summary>
        /// <param name="userMsg"></param>
        /// <param name="ngRockUrl"></param>
        /// <returns></returns>
        public EmbedBuilder MakeJellyfinMessageBuilder(SocketUserMessage userMsg, string ngRockUrl)
        {
            log.Info($"IMG_url: " + Helper._JellyfinImgUrl);
            return new EmbedBuilder
            {
                Url = ngRockUrl,
                Color = Color.DarkRed,                
                ImageUrl = Helper._JellyfinImgUrl,
                ThumbnailUrl = Helper._JellyfinGif,

                Title = $"{Helper.GetVerifiedEmote()}︱Cliquez ici︱{Helper.GetVerifiedEmote()}",
                //Description = $"{Helper.GetCoinEmote()}  En stream avec **Jellyfin Media Player** sur PC\n" +
                //    $"{Helper.GetCoinEmote()}  En **DL** avec Google CHrome sur PC\n" +
                //    $"{Helper.GetCoinEmote()}  ERR_NGROK = relancer **$Jellyfin** \n" +
                //    $"{Helper.GetCoinEmote()}  / à venir",
                Description = $"{Helper.GetCoinEmote()}  xxxxxxxxxxx\n" +
                    $"{Helper.GetCoinEmote()}  / à venir",

                Author = new EmbedAuthorBuilder { Name = "Jellyfin requested by " + userMsg.Author.Username, IconUrl = userMsg.Author.GetAvatarUrl() },
                Footer = GetFooterBuilder()
            };
        }

        private EmbedFooterBuilder GetFooterBuilder()
        {
            return new EmbedFooterBuilder
            {
                IconUrl = Helper._urlAvatarVince,
                Text = $"Powered with {Helper.GetCoeurEmoji()} by Vince"
            };
        }

        public async Task UnPinLastJelly(List<RestMessage> pinneds)
        {
            try
            {
                var lastPin = pinneds.First() as IUserMessage;
                if (lastPin != null)
                {
                    if (lastPin.Content.StartsWith('$'))
                        await lastPin.UnpinAsync();
                    else
                    {
                        var nextPin = pinneds.Skip(1).OfType<IUserMessage>().FirstOrDefault(x => x.Content.StartsWith('$'));
                        await nextPin.UnpinAsync();
                    }
                }
            }
            catch (Exception ex) { log.Warn("UnPinLastJelly"); log.Error(ex); }
        }

        #endregion
    }
}