using System;
using System.Collections.Generic;
using AStar.models;

namespace AStar.Controllers
{
    public class MoveController : IController
    {
        private Game Game { get; }
        public MoveController(Game game)
        {
            Game = game;
        }
        public void ExecuteCommand(string command)
        {
            Console.WriteLine($"// MoveController: Got {command}");
            var cmdSplitted = command.Split(" ");
            if (cmdSplitted.Length != 2)
            {
                throw new Exception("Wrong move cmd");
            }

            var coords = cmdSplitted[1];
            Game.DoStep(new Move(coords.ToLower()).Point);
        }
    }
}