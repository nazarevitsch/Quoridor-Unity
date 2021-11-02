using System;
using System.Collections.Generic;
using AStar.models;

namespace AStar.Controllers
{
    public class WallController : IController
    {
        private Game Game { get; }
        
        private Dictionary<char, int> MoveDictionary { get; } = new()
        {
            {'s', 1},
            {'t', 3},
            {'u', 5},
            {'v', 7},
            {'w', 9},
            {'x', 11},
            {'y', 13},
            {'z', 15},
        };
        
        public WallController(Game game)
        {
            Game = game;
        }
        public void ExecuteCommand(string command)
        {
            var cmdSplitted = command.Split(" ");
            if (cmdSplitted.Length != 2)
            {
                throw new Exception("Wrong move cmd");
            }
            var coords = cmdSplitted[1];
            Game.PlaceWall(new Wall(coords.ToLower()));
        }
    }
}