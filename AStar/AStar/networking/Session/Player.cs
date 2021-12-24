using System.Net.Sockets;
using System.Text;
using AStar.models;

namespace AStar.networking.Session
{
    public class Player
    {
        public PlayColor Color { get; set; }
        public Point Position { get; set; }
        public GameSession GameSession { get; set; }
        private TcpClient Client { get; }

        public Player(TcpClient client)
        {
            Client = client;
        }

        public string GetMessage()
        {
            var stream = Client.GetStream();
            var data = new byte[64];
            var builder = new StringBuilder();
            int bytes;
            do
            {
                bytes = stream.Read(data, 0, data.Length);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            }
            while (stream.DataAvailable);
            return builder.ToString();
        }

        public void SendMessageToClient(string msg)
        {
            Client.GetStream().Write(Encoding.Unicode.GetBytes(msg));
        }

        public void Close()
        {
            Client.Close();
        }

    }
}