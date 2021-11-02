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
            
        }
    }
}