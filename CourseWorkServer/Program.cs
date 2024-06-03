using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
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
            int currentKey = 0;
            Dictionary<int, Lobby> lobbies = new Dictionary<int, Lobby>();
            var ipPoint = new IPEndPoint(IPAddress.Parse(DEAFALUT_GATEWAY), PORT);
            using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(ipPoint);
            socket.Listen(LENGTH);
            while (true)
            {
                var client = await socket.AcceptAsync();

                await HandleClient(client, lobbies);
            }
        }

        private static async Task HandleClient(Socket client, Dictionary<int, Lobby> lobbies)
        {
            while (client.Connected)
            {
                string message = "";
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
                
                if (word == "CREATE")
                {
                    var rnd = new Random();
                    var id = rnd.Next(0, 1024);

                    while (lobbies.ContainsKey(id))
                    {
                        id = rnd.Next(0, 1024);
                    }

                    lobbies.Add(id, new Lobby( new User(client)));
                    await client.SendAsync(Encoding.UTF8.GetBytes($"LOBBY,{id}"));
                }

                var request = word.Split(',');
                
                if (request[0] == "JOIN")
                {
                    User guesser = new User();
                    User cryptographer = new User();
                    lock (lobbies)
                    {
                        var lobbyID = int.Parse(request[1]);

                        if (!lobbies.ContainsKey(lobbyID))
                        {
                            message = "ERROR,Нет такого лобби";
                        }

                        if (message.Length == 0)
                        {
                            lobbies[int.Parse(request[1])].Users.Add(new User(client));
                            var rnd = new Random();

                            var crypto = rnd.Next(0, 2);
                            cryptographer = lobbies[lobbyID].Users[Math.Abs(crypto - 1)];
                            guesser = lobbies[lobbyID].Users[crypto];
                        }
                    }

                    if (message.Length > 0)
                    {
                        await client.SendAsync(Encoding.UTF8.GetBytes(message));
                        continue;
                    }

                    cryptographer.Role = UserRole.Cryptographer;
                    guesser.Role = UserRole.Guesser;
                    
                    message = "START,";
                    await cryptographer.Address
                        .SendAsync(Encoding.UTF8.GetBytes(message + "0"));
                    
                    await guesser.Address
                        .SendAsync(Encoding.UTF8.GetBytes(message + "1"));
                }

                if (request[0] == "GIVEUP")
                {
                    User cryptographer = new User();
                    lock (lobbies)
                    {
                        var users = lobbies[int.Parse(request[1])].Users;

                        foreach (var user in users)
                        {
                            if (user.Role == UserRole.Cryptographer)
                            {
                                cryptographer = user;
                                break;
                            }
                        }
                    }

                    await cryptographer.Address.SendAsync("RESULT,Вы выиграли"u8.ToArray());
                }
                
                if (request[0] == "WIN")
                {
                    User cryptographer = new User();
                    lock (lobbies)
                    {
                        var users = lobbies[int.Parse(request[1])].Users;

                        foreach (var user in users)
                        {
                            if (user.Role == UserRole.Cryptographer)
                            {
                                cryptographer = user;
                                break;
                            }
                        }
                    }

                    await cryptographer.Address.SendAsync("RESULT,Вы проиграли"u8.ToArray());
                }

                if (request[0] == "STATUS")
                {
                    List<uint> currentState;
                    var id = int.Parse(request[1]);
                    int position;
                    
                    lock (lobbies)
                    {
                        position = lobbies[id].CurrentPosition;
                        currentState = lobbies[id].CurrentButtonState[position];
                    }

                    message = $"STATUS,{position},";
                    for (int i = 0; i < currentState.Count; i++)
                    {
                        message += $"{currentState[i]}";

                        if (i != currentState.Count - 1) message += ";";
                    }

                    await client.SendAsync(Encoding.UTF8.GetBytes(message));
                }

                if (request[0] == "COMBINATION" && request.Length == 3)
                {
                    var nums = new List<uint>();
                    var stringNums = request[2].Split(';');

                    foreach (var stringNum in stringNums)
                    {
                        nums.Add(uint.Parse(stringNum));
                    }

                    lock (lobbies)
                    {
                        lobbies[int.Parse(request[1])].Combination = nums;
                    }
                }
                
                if (request[0] == "COMBINATION" && request.Length == 4)
                {
                    var nums = new List<uint>();
                    var stringNums = request[3].Split(';');

                    foreach (var stringNum in stringNums)
                    {
                        nums.Add(uint.Parse(stringNum));
                    }

                    lock (lobbies)
                    {
                        var id = int.Parse(request[1]);
                        var position = int.Parse(request[2]);
                        if (lobbies[id].CurrentPosition != position)
                        {
                            lobbies[id].CurrentButtonState.Add(nums);
                            lobbies[id].CurrentPosition = position;
                        }
                        else
                        {
                            lobbies[id].CurrentButtonState[position] = nums;
                        }
                    }
                }
            }
            
            Console.WriteLine($"{(IPEndPoint)client.RemoteEndPoint!} shutting down");
            client.Shutdown(SocketShutdown.Both);
            client.Close();
        }
    }
}