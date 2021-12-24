using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using AStar.networking.PlayerIO;
using AStar.networking.Session;

namespace AStar.networking
{
    public class Server
    {
        private TcpListener _listener = new(IPAddress.Parse("127.0.0.1"), 8000);
        private SessionManager SessionManager = new();
        public void Start()
        {
            try
            {
                Console.WriteLine("Сервер запущен. Ожидание подключений...");
                _listener.Start();
                while (true)
                {
                    var client = _listener.AcceptTcpClient();
                    new Thread(() => HandleClient(client)).Start();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void HandleClient(TcpClient client)
        {
            var player = new Player(client);
            GameSession session;
            while (true)
            {
                try
                {
                    var message = player.GetMessage();
                    Console.WriteLine($"###########\n\n{message}");
                    var cmd = CommandParser.FromString(message);
                    switch (cmd.Code)
                    {
                        case CommandCodes.CreateSession:
                            session = SessionManager.CreateSession(cmd.Name);
                            SessionManager.AddPlayerToSession(session, player);
                            Console.WriteLine($"Created Session: {session.Guid}");
                            player.SendMessageToClient($"{session.Guid}");
                            break;
                        
                        case CommandCodes.ConnectToSession:
                            session = string.IsNullOrEmpty(cmd.Name) ?
                                SessionManager.FindSessionByGuid(new Guid(cmd.Guid)) :
                                SessionManager.FindSessionByName(cmd.Name);
                            
                            SessionManager.AddPlayerToSession(session, player);
                            Console.WriteLine($"Connected to Session: {session.Guid}");
                            break;
                        case CommandCodes.Move:
                            session = SessionManager.FindSessionByGuid(new Guid(cmd.Guid));
                            session.Move(cmd.Position);
                            break;
                        case CommandCodes.PutBlock:
                            session = SessionManager.FindSessionByGuid(new Guid(cmd.Guid));
                            session.PutBlock(cmd.Position);
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    break;
                }
            }
        }
    }
}