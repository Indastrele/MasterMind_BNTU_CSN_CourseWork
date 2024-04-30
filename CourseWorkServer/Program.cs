using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Server
    {
        private const string DEAFALUT_GATEWAY = "0.0.0.0";
        private const int PORT = 11000;
        private const int LENGTH = 10;
        private const int SIZE = 512;
        
        private static async Task Main()
        {
            var ipPoint = new IPEndPoint(IPAddress.Parse(DEAFALUT_GATEWAY), PORT);
            using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(ipPoint);
            socket.Listen(LENGTH);
            while (true)
            {
                var client = await socket.AcceptAsync();

                await Task.Run(async () => await HandleClient(client));
            }
        }

        private static async Task HandleClient(Socket client)
        {
            while (client.Connected)
            {
                var response = new List<byte>();
                var bytesRead = new byte[1];
                while (true)
                {
                    var count = await client.ReceiveAsync(bytesRead);
                    if (count == 0 || bytesRead[0] == '\n') break;
                    response.Add(bytesRead[0]);
                }
                var word = Encoding.UTF8.GetString(response.ToArray());
                if (word == "END") break;
            }
            
            Console.WriteLine($"{(IPEndPoint)client.RemoteEndPoint!} shutting down");
            client.Shutdown(SocketShutdown.Both);
            client.Close();
        }
    }
}