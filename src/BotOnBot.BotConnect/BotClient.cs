﻿using System.Net.Sockets;
using System.Threading.Tasks;
using BotOnBot.Backend.DataModel;

namespace BotOnBot.BotConnect
{
    /// <summary>
    /// Connects your Bot to the game server.
    /// </summary>
    public sealed class BotClient
    {
        private readonly TcpClient _client;

        internal BotClient(TcpClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Reads and returns the next message from the game server.
        /// </summary>
        public async Task<ServerMessage> ReadNextMessage()
        {
            var result = string.Empty;
            using (var sr = StreamFactory.CreateReader(_client.GetStream()))
            {
                result = await sr.ReadLineAsync();
            }

            var parsed = Serializer.Deserialize<NetworkResponseModel>(result);
            if (!EnumParser.TryParse<ResponseStatusType>(parsed.ResponseStatus, out var status))
                status = ResponseStatusType.Invalid;

            return new ServerMessage(result, status.ToString());
        }

        /// <summary>
        /// Sends a custom message to the game server.
        /// </summary>
        public async Task SendMessage(string message)
        {
            using (var sw = StreamFactory.CreateWriter(_client.GetStream()))
            {
                await sw.WriteLineAsync(message);
            }
        }

        internal async Task SendStartInformation(string name, string author)
        {
            await SendMessage($"{{\"name\":\"{name}\",\"author\":\"{author}\"}}");
        }
    }
}
