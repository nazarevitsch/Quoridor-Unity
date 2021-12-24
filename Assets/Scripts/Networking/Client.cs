using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using models;
using UnityEngine;
using UnityEngine.Purchasing.MiniJSON;

namespace Networking
{
    public class Client
    {
        private Guid SessionGuid;
        private TcpClient _client = new TcpClient();

        private Game _game;
        
        public Client(Game game)
        {
            _game = game;
        }

        public void Connect()
        {
            _client.Connect(IPAddress.Parse("127.0.0.1"), 8000);
            new Thread(() => ReceiveMessage(_client)).Start();
        }

        public void SendCommand(Command cmd)
        {
            cmd.Guid = $"{SessionGuid}";
            var json = JsonUtility.ToJson(cmd);
            _client.GetStream().Write(Encoding.Unicode.GetBytes(json), 0, json.Length);
        }

        private void ReceiveMessage(TcpClient client)
        {
            var stream = client.GetStream();
            while (true)
            {
                try
                {
                    var data = new byte[120];
                    var builder = new StringBuilder();
                    int bytes;
                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (stream.DataAvailable);
 
                    var message = builder.ToString();
                    Debug.Log("Get message: " + message);
                }
                catch
                {
                    Debug.Log("Connect Dropped");
                }
            }
        }
    }
}