using System;
using System.Collections.Generic;
using System.Text.Json;
using AStar.models;
using AStar.networking.PlayerIO;

namespace AStar.networking.Session
{
    public class GameSession
    {
        public string Name { get; set; }
        public Guid Guid { get; }
        private List<Player> Players { get; } = new();
        
        public Game Game { get; }

        public GameSession(string name)
        {
            Game = new Game();
            Guid = Guid.NewGuid();
            Name = name;
            Game.PlayWithFriend();
        }

        public void Close()
        {
            foreach (var player in Players)
            {
                player.Close();
            }
        }

        public void AddPlayer(Player player)
        {
            player.GameSession = this;
            Players.Add(player);
        }

        public void Move(Point point)
        {
            Game.DoStep(point);
            foreach (var player in Players)
            {
                player.SendMessageToClient(JsonSerializer.Serialize(new Command
                {
                    Code = "Move",
                    Guid = $"{Guid}",
                    Position = point
                }));
            }
        }

        public void PutBlock(Point point)
        {
            Game.PutBlock(point);
        }
    }
}