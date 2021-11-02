using System;
using AStar.models;

namespace AStar.Controllers
{
    public class JumpController : IController
    {
        private Game Game { get; }
        public JumpController(Game game)
        {
            Game = game;
        }

        public void ExecuteCommand(string command)
        {
            Console.WriteLine($"// JumpController: Got {command}");
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