using System;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TestClient.Services
{
    static class ServerConnection
    {

        private static Socket socket;
        private static bool isConnected = false;
        private static TaskCompletionSource<string> tcs;

        public static async Task<bool> ConnectAsync()
        {
            if (!isConnected)
            {
                try
                {
                    socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    await socket.ConnectAsync("127.0.0.1", 8888);
                    isConnected = true;
                }
                catch (SocketException)
                {
                    return false;
                }
            }
            return true;
        }

        public static bool Disconnect()
        {
            if (isConnected)
            {
                try
                {
                    socket?.Shutdown(SocketShutdown.Both);
                    socket?.Close();
                    isConnected = false;
                }
                catch (SocketException)
                {
                    //Console.WriteLine($"Не удалось отключиться от {socket?.RemoteEndPoint}");
                    return false;
                }
            }
            return true;
        }

        public static async Task<string> SendData(object obj)
        {
            // need exception handling
            tcs = new TaskCompletionSource<string>(); ;
            string jsonString = JsonSerializer.Serialize(obj);
            byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonString);
            ArraySegment<byte> bufferSegment = new(jsonBytes);
            _ = await socket.SendAsync(bufferSegment, SocketFlags.None);
            return await tcs.Task;
        }
        public static async Task ReceiveData()
        {
            byte[] buffer = new byte[51200];
            while (true)
            {
                int result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);
                if (result > 0)
                {
                    string receivedString = Encoding.UTF8.GetString(buffer, 0, result);
                    tcs.SetResult(receivedString);
                }
            }
        }
    }
}
