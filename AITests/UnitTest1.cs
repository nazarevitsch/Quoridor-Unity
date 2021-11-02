using AStar.models;
using NUnit.Framework;

namespace AITests
{
    public class Tests
    {
        private Game Game { get; set; }
        private GameFlow GameFlow { get; set; }

        [SetUp]
        public void Setup()
        {
            Game = new Game();
            GameFlow = new GameFlow(Game);
            Game.PlayWithFriend();
            Game.ChangePlayers();
        }
        [Test]
        public void TestOneStep()
        {
            var move = GameFlow.FindBestMove();
            Assert.That(() => move.X == 8 && move.Y == 2);
        }
        
        [Test]
        public void TwoSteps()
        {
            var move = GameFlow.FindBestMove();
            Game.DoStep(move);
            Game.ChangePlayers();
            move = GameFlow.FindBestMove();
            Assert.That(() => move.X == 8 && move.Y == 4);
        }
        
        [Test]
        public void ThreeSteps()
        {
            var move = GameFlow.FindBestMove();
            Game.DoStep(move);
            Game.ChangePlayers();
            move = GameFlow.FindBestMove();
            Game.DoStep(move);
            Game.ChangePlayers();
            move = GameFlow.FindBestMove();
            Assert.That(() => move.X == 8 && move.Y == 6);
        }
    }
}