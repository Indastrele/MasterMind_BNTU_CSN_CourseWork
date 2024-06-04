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
            var socket = new TcpListener(IPAddress.Parse(DEAFALUT_GATEWAY), PORT);
            socket.Start();
            while (true)
            {
                HandleClient(await socket.AcceptTcpClientAsync(), lobbies);
            }
        }

        private static async Task HandleClient(TcpClient client, Dictionary<int, Lobby> lobbies)
        {
            using (client)
            {
                {
                    var stream = client.GetStream();
                    while (true)
                    {
                        string message = "";
                        var bytesRead = new byte[1024];
                        var count = await stream.ReadAsync(bytesRead, 0, bytesRead.Length);
                        var word = Encoding.UTF8.GetString(bytesRead, 0, count).Trim().TrimEnd('\n');

                        if (word == "END") break;

                        if (word == "CREATE")
                        {
                            var rnd = new Random();
                            var id = rnd.Next(0, 1024);

                            while (lobbies.ContainsKey(id))
                            {
                                id = rnd.Next(0, 1024);
                            }

                            lobbies.Add(id, new Lobby(new User(stream)));
                            await stream.WriteAsync(Encoding.UTF8.GetBytes($"LOBBY,{id}"));
                            await stream.FlushAsync();
                        }

                        var request = word.Split(',');

                        if (request[0] == "JOIN")
                        {
                            lock (lobbies)
                            {
                                var lobbyID = int.Parse(request[1]);

                                if (!lobbies.ContainsKey(lobbyID))
                                {
                                    message = "ERROR,Нет такого лобби";
                                }

                                if (message.Length == 0)
                                {
                                    lobbies[int.Parse(request[1])].Users.Add(new User(stream));
                                    var rnd = new Random();

                                    var crypto = rnd.Next(0, 2);
                                    lobbies[lobbyID].Users[crypto].Role = UserRole.Cryptographer;

                                    message = "START,";

                                    foreach (var user in lobbies[lobbyID].Users)
                                    {
                                        if (user.Role == UserRole.Cryptographer)
                                        {
                                            user.Address.Write(Encoding.UTF8.GetBytes(message + "0"));
                                            user.Address.Flush();
                                        }
                                        else
                                        {
                                            user.Address.Write(Encoding.UTF8.GetBytes(message + "1"));
                                            user.Address.Flush();
                                        }
                                    }

                                }
                            }

                            if (message.Length > 0)
                            {
                                await stream.WriteAsync(Encoding.UTF8.GetBytes(message));
                                await stream.FlushAsync();
                                continue;
                            }
                        }

                        if (request[0] == "GIVEUP")
                        {
                            lock (lobbies)
                            {
                                var users = lobbies[int.Parse(request[1])].Users;

                                foreach (var user in users)
                                {
                                    if (user.Role == UserRole.Cryptographer)
                                    {
                                        user.Address.Write("RESULT,Вы выиграли"u8.ToArray());
                                        user.Address.Flush();
                                        break;
                                    }
                                }
                            }
                        }

                        if (request[0] == "WIN")
                        {
                            lock (lobbies)
                            {
                                var users = lobbies[int.Parse(request[1])].Users;

                                foreach (var user in users)
                                {
                                    if (user.Role == UserRole.Cryptographer)
                                    {
                                        user.Address.Write("RESULT,Вы проиграли"u8.ToArray());
                                        user.Address.Flush();
                                        break;
                                    }
                                }
                            }
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

                            await stream.WriteAsync(Encoding.UTF8.GetBytes(message));
                            await stream.FlushAsync();
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
                }
            }
        }
    }
}