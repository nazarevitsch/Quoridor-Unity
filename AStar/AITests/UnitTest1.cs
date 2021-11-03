using System;
using AStar.Controllers;
using AStar.models;
using AStar.models.IO;
using NUnit.Framework;

namespace AITests
{
    public class Tests
    {
        private Game Game { get; set; }
        private GameFlow GameFlow { get; set; }

        private IIoManager FakeIo { get; }= new FakeIo();

        [SetUp]
        public void Setup()
        {
            Game = new Game();
            GameFlow = new GameFlow(Game, FakeIo);
            Game.PlayWithFriend();
            Game.ChangePlayers();
            GameFlow.RegisterController("move", new MoveController(Game));
            GameFlow.RegisterController("wall", new WallController(Game));
            GameFlow.RegisterController("jump", new JumpController(Game));
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
        
        [Test]
        public void WallFromString()
        {
            var wall = new Wall("u1v");
            Assert.That(() => wall.Point.X == 5 && wall.Point.Y == 1 && wall.WallType == WallType.Vertical);
        }
        
        [Test]
        public void WallFromStringUpper()
        {
            var wall = new Wall("u1v");
            var wall2 = new Wall("U1v");
            Assert.That(() => wall.Point.X == wall2.Point.X && wall.Point.Y == wall2.Point.Y && wall.WallType == wall2.WallType);
        }

        [Test]
        public void PlaceVerticalWallTest()
        {
            Game.PlaceWall(new Wall("u1v"));
            Assert.That(() => Game.Points[0][5].Tag == "HalfBlocked" && Game.Points[2][5].Tag == "Blocked");
        }
        [Test]
        public void PlaceHorizontalWallTest()
        {
            Game.PlaceWall(new Wall("u1h"));
            Assert.That(() => Game.Points[1][4].Tag == "HalfBlocked" && Game.Points[1][6].Tag == "Blocked");
        }
        
        [Test]
        public void PlaceHorizontalWallTestW1H()
        {
            Game.PlaceWall(new Wall("w1h"));
            Assert.That(() => Game.Points[1][8].Tag == "HalfBlocked" && Game.Points[1][10].Tag == "Blocked");
        }
        
        
        /*
          Case when walls break game loop
         * // Start game
                2021-11-03 23:22:23.3092|INFO|Logger|// Start game
                2021-11-03 23:22:23.3158|INFO|Logger|// GOT white
                2021-11-03 23:22:23.3934|INFO|Logger|// Get Point: Point(X=8, Y=14)
                2021-11-03 23:22:23.3934|INFO|Logger|// MakeMove: e8
                2021-11-03 23:22:23.3934|INFO|Logger|<- move e8
                2021-11-03 23:22:23.5993|INFO|Logger|-> wall U1v
                2021-11-03 23:22:23.5993|INFO|Logger|// Game loop: Got command wall U1v
                2021-11-03 23:22:23.6992|INFO|Logger|// Get Point: Point(X=8, Y=12)
                2021-11-03 23:22:23.6992|INFO|Logger|// MakeMove: e7
                2021-11-03 23:22:23.6992|INFO|Logger|<- move e7
                2021-11-03 23:22:23.8238|INFO|Logger|-> wall X1v
                2021-11-03 23:22:23.8238|INFO|Logger|// Game loop: Got command wall X1v
                2021-11-03 23:22:23.8749|INFO|Logger|// Get Point: Point(X=8, Y=10)
                2021-11-03 23:22:23.8749|INFO|Logger|// MakeMove: e6
                2021-11-03 23:22:23.8749|INFO|Logger|<- move e6
                2021-11-03 23:22:23.9929|INFO|Logger|-> wall W1h
                2021-11-03 23:22:23.9929|INFO|Logger|// Game loop: Got command wall W1h
                2021-11-03 23:22:23.9929|INFO|Logger|// Get Point: Point(X=6, Y=0)
                2021-11-03 23:22:23.9929|INFO|Logger|// MakeMove: d1
                2021-11-03 23:22:23.9929|INFO|Logger|<- move d1
         */
        [Test]
        public void FailedCase1Manual()
        {
            var outPlayer = Game.CurrentPlayer;
            Game.DoStep(new Move("e8").Point);
            Game.PlaceWall(new Wall("U1v"));
            Game.DoStep(new Move("e7").Point);
            Game.PlaceWall(new Wall("X1v"));
            Game.DoStep(new Move("e6").Point);
            Game.PlaceWall(new Wall("W1h"));
            var move = GameFlow.FindBestMove();
            Assert.That(() => outPlayer == Game.CurrentPlayer);
        }
        [Test]
        public void FailedCase1Automatic()
        {
            FakeIo.Write("white");
            FakeIo.Write("wall U1v");
            FakeIo.Write("wall X1v");
            FakeIo.Write("wall W1h");
            FakeIo.Write("STOP");
            try
            {
                GameFlow.StartGame();
            }
            catch (Exception)
            {
                var move = new Move("e5");
                Assert.That(() => Game.EnemyPlayer.CurrentX == move.Point.X && Game.EnemyPlayer.CurrentY == move.Point.Y);
            }
        }
        
        /*
         * // Start game
                2021-11-04 01:31:16.6264|INFO|Logger|-> move E8
                2021-11-04 01:31:16.6270|INFO|Logger|// Start game
                2021-11-04 01:31:16.6270|INFO|Logger|// GOT black
                2021-11-04 01:31:16.6270|INFO|Logger|// Game loop: Got command move E8
                2021-11-04 01:31:16.6270|INFO|Logger|// MoveController: Got move E8
                2021-11-04 01:31:16.6854|INFO|Logger|// Get Point: Point(X=8, Y=2, str=e2)
                2021-11-04 01:31:16.6854|INFO|Logger|// MakeMove: e2
                2021-11-04 01:31:16.6854|INFO|Logger|<- move e2
                2021-11-04 01:31:16.8024|INFO|Logger|-> move E7
                2021-11-04 01:31:16.8024|INFO|Logger|// Game loop: Got command move E7
                2021-11-04 01:31:16.8024|INFO|Logger|// MoveController: Got move E7
                2021-11-04 01:31:16.8447|INFO|Logger|// Get Point: Point(X=8, Y=4, str=e3)
                2021-11-04 01:31:16.8447|INFO|Logger|// MakeMove: e3
                2021-11-04 01:31:16.8447|INFO|Logger|<- move e3
                2021-11-04 01:31:16.9162|INFO|Logger|-> wall W7h
                2021-11-04 01:31:16.9162|INFO|Logger|// Game loop: Got command wall W7h
                2021-11-04 01:31:16.9655|INFO|Logger|// Get Point: Point(X=8, Y=6, str=e4)
                2021-11-04 01:31:16.9655|INFO|Logger|// MakeMove: e4
                2021-11-04 01:31:16.9655|INFO|Logger|<- move e4
                2021-11-04 01:31:17.0340|INFO|Logger|-> move E6
                2021-11-04 01:31:17.0340|INFO|Logger|// Game loop: Got command move E6
                2021-11-04 01:31:17.0340|INFO|Logger|// MoveController: Got move E6
                2021-11-04 01:31:17.0737|INFO|Logger|// Get Point: Point(X=8, Y=8, str=e5)
                2021-11-04 01:31:17.0737|INFO|Logger|// MakeMove: e5
                2021-11-04 01:31:17.0737|INFO|Logger|<- move e5
                2021-11-04 01:31:17.1441|INFO|Logger|-> wall V7v
                2021-11-04 01:31:17.1441|INFO|Logger|// Game loop: Got command wall V7v
                2021-11-04 01:31:17.1441|INFO|Logger|// Get Point: Point(X=8, Y=12, str=e7)
                2021-11-04 01:31:17.1441|INFO|Logger|// MakeMove: e7
                2021-11-04 01:31:17.1441|INFO|Logger|<- move e7
         */
        [Test]
        public void FailedCase2Automatic()
        {
            FakeIo.Write("black");
            FakeIo.Write("move E8");
            FakeIo.Write("move E7");
            FakeIo.Write("wall W7h");
            FakeIo.Write("move E6");
            FakeIo.Write("wall V7v");
            FakeIo.Write("STOP");
            try
            {
                GameFlow.StartGame();
            }
            catch (Exception)
            {
                var move = new Move("e5");
                Assert.False(false);
            }
        }

        [Test]
        /*
        2021-11-04 01:54:09.4137|INFO|Logger|// Start game
        2021-11-04 01:54:09.4137|INFO|Logger|// GOT white
        2021-11-04 01:54:09.4629|INFO|Logger|// Get Point: Point(X=8, Y=14, str=e8)
        2021-11-04 01:54:09.4629|INFO|Logger|// MakeMove: e8, Current Player: White
        2021-11-04 01:54:09.4629|INFO|Logger|<- move e8
        2021-11-04 01:54:09.5889|INFO|Logger|-> move E2
        2021-11-04 01:54:09.5889|INFO|Logger|// Game loop: Got command move E2
        2021-11-04 01:54:09.5889|INFO|Logger|// MoveController: Got move E2
        2021-11-04 01:54:09.6436|INFO|Logger|// Get Point: Point(X=8, Y=12, str=e7)
        2021-11-04 01:54:09.6436|INFO|Logger|// MakeMove: e7, Current Player: White
        2021-11-04 01:54:09.6436|INFO|Logger|<- move e7
        2021-11-04 01:54:09.7512|INFO|Logger|-> move E3
        2021-11-04 01:54:09.7512|INFO|Logger|// Game loop: Got command move E3
        2021-11-04 01:54:09.7517|INFO|Logger|// MoveController: Got move E3
        2021-11-04 01:54:09.7775|INFO|Logger|// Get Point: Point(X=8, Y=10, str=e6)
        2021-11-04 01:54:09.7775|INFO|Logger|// MakeMove: e6, Current Player: White
        2021-11-04 01:54:09.7775|INFO|Logger|<- move e6
        2021-11-04 01:54:09.8523|INFO|Logger|-> move E4
        2021-11-04 01:54:09.8523|INFO|Logger|// Game loop: Got command move E4
        2021-11-04 01:54:09.8523|INFO|Logger|// MoveController: Got move E4
        2021-11-04 01:54:09.8795|INFO|Logger|// Get Point: Point(X=8, Y=8, str=e5)
        2021-11-04 01:54:09.8795|INFO|Logger|// MakeMove: e5, Current Player: White
        2021-11-04 01:54:09.8795|INFO|Logger|<- move e5
        2021-11-04 01:54:09.9529|INFO|Logger|-> jump E6
        2021-11-04 01:54:09.9547|INFO|Logger|// Game loop: Got command jump E6
        2021-11-04 01:54:09.9547|INFO|Logger|// JumpController: Got jump E6
        2021-11-04 01:54:09.9777|INFO|Logger|// Get Point: Point(X=8, Y=6, str=e4)
        2021-11-04 01:54:09.9777|INFO|Logger|// MakeMove: e4, Current Player: White
        2021-11-04 01:54:09.9777|INFO|Logger|<- move e4
        2021-11-04 01:54:10.0441|INFO|Logger|-> wall W3h
        2021-11-04 01:54:10.0441|INFO|Logger|// Game loop: Got command wall W3h
        2021-11-04 01:54:10.0555|INFO|Logger|// Get Point: Point(X=6, Y=6, str=d4)
        2021-11-04 01:54:10.0555|INFO|Logger|// MakeMove: d4, Current Player: White
        2021-11-04 01:54:10.0555|INFO|Logger|<- move d4
        2021-11-04 01:54:10.1089|INFO|Logger|-> move E7
        2021-11-04 01:54:10.1089|INFO|Logger|// Game loop: Got command move E7
        2021-11-04 01:54:10.1089|INFO|Logger|// MoveController: Got move E7
        2021-11-04 01:54:10.1193|INFO|Logger|// Get Point: Point(X=4, Y=6, str=c4)
        2021-11-04 01:54:10.1193|INFO|Logger|// MakeMove: c4, Current Player: White
        2021-11-04 01:54:10.1193|INFO|Logger|<- move c4
        2021-11-04 01:54:10.1571|INFO|Logger|-> wall W4h
        2021-11-04 01:54:10.1578|INFO|Logger|// Game loop: Got command wall W4h
        2021-11-04 01:54:10.1578|INFO|Logger|// Get Point: Point(X=4, Y=4, str=c3)
        2021-11-04 01:54:10.1578|INFO|Logger|// MakeMove: c3, Current Player: White
        2021-11-04 01:54:10.1578|INFO|Logger|<- move c3
        2021-11-04 01:54:10.1992|INFO|Logger|-> wall V4v
        2021-11-04 01:54:10.1992|INFO|Logger|// Game loop: Got command wall V4v
        2021-11-04 01:54:10.2201|INFO|Logger|// Get Point: Point(X=6, Y=12, str=d7)
        2021-11-04 01:54:10.2201|INFO|Logger|// MakeMove: d7, Current Player: Black
        2021-11-04 01:54:10.2201|INFO|Logger|<- move d7
         */
        public void FailedTest3Automatic()
        {
        }
    }
}