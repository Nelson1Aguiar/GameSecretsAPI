using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;

namespace GameSecretsAPI.Transactional
{
    public class ChatWebSocketHandler
    {
        private static readonly ConcurrentDictionary<string, WebSocket> _clients = new();

        public static async Task HandleAsync(WebSocket socket)
        {
            var clientId = Guid.NewGuid().ToString();
            _clients[clientId] = socket;

            var buffer = new byte[1024 * 4];
            var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            while (!result.CloseStatus.HasValue)
            {
                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Console.WriteLine($"Mensagem do chat: {message}");

                await BroadcastAsync(message);

                result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            _clients.TryRemove(clientId, out _);
            await socket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }

        private static async Task BroadcastAsync(string message)
        {
            var buffer = Encoding.UTF8.GetBytes(message);
            var segment = new ArraySegment<byte>(buffer);

            foreach (var socket in _clients.Values)
            {
                if (socket.State == WebSocketState.Open)
                {
                    await socket.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }
    }
}
