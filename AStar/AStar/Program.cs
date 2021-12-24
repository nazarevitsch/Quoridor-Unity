using System;
using System.Text.Json;
using AStar.networking;

namespace AStar
{
    
    class Program
    {
        static void Main(string[] args)
        {
            var server = new Server();
            server.Start();
        }
    }
}