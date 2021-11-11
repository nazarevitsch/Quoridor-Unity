using System;
using System.Collections.Generic;
using System.Linq;
using AStar.models.AI.PathFinder;
using AStar.Controllers;
using AStar.models;
using AStar.models.AI.MoveStrategies;
using AStar.models.AI.MoveStrategies.BuildStrategies;
using AStar.models.AI.MoveStrategies.MoveStrategies;
using AStar.models.AI.PathFinder.PathFindStrategies;
using AStar.models.AI.PathFinder.Euristics;
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
            var moveStr = new MoveStrategyManager<Point>(Game, FakeIo);
            moveStr.UseMoveStrategy(() => new RandomMoveStrategy<Point>(Game, FakeIo));
            GameFlow = new GameFlow(Game, FakeIo, moveStr);
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
        public void PosiblePlatforms()
        {
            var list = Game.FindPossiblePlatforms(new Player(4, 14), Game.EnemyPlayer, Game.Points);
            Assert.That(() => list.Count == 4);
        }
        
        [Test]
        public void LowestH()
        {
            var list = Game.FindPossiblePlatforms(new Player(4, 14), Game.EnemyPlayer, Game.Points);
            var aStar = new AStar<Point>()
                .UseHeuristicStrategy<PifagorHeuristic>()
                .UseNeighbourFindStrategy<FourWayPath>();
            var xs = new List<int>
            {
                0, 2, 4, 6, 8, 10, 12, 14, 16 
                //  a, b, c, d, e, f,  g,  h,  i
            };
            var Y =  16; // first or last row
            var res = xs.Select(X =>
                {
                    return aStar.Resolve(Game.Points, new Point {X = 4, Y = 14}, new Point
                    {
                        X = X,
                        Y = Y 
                    }).Count;
                }
            ).Min();
            Assert.That(() => res == 2);
            
        }

        [Test]
        public void BestMove()
        {
            var miniMax = new MiniMax<Point>(3)
                .UseBuildTreeStrategy(() => new QuoridorBuildStrategy(Game, true)); // This mean we are white
            miniMax.BuildTree(Game.Points, new Point{ X = 8, Y = 2 });
            var res2 = miniMax.FindBestNodeBF();
            Assert.True(true);
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
         *  2021-11-04 13:10:01.8404|INFO|Logger|-> move E8
            2021-11-04 13:10:01.8404|INFO|Logger|// Start game
            2021-11-04 13:10:01.8404|INFO|Logger|// GOT black
            2021-11-04 13:10:01.8457|INFO|Logger|// Game loop: Got command move E8
            2021-11-04 13:10:01.8457|INFO|Logger|// MoveController: Got move E8
            2021-11-04 13:10:01.9653|INFO|Logger|// Get Point: Point(X=8, Y=2, str=e2)
            2021-11-04 13:10:01.9653|INFO|Logger|// MakeMove: e2, Current Player: Black
            2021-11-04 13:10:01.9653|INFO|Logger|<- move e2
            2021-11-04 13:10:02.1080|INFO|Logger|-> move E7
            2021-11-04 13:10:02.1080|INFO|Logger|// Game loop: Got command move E7
            2021-11-04 13:10:02.1123|INFO|Logger|// MoveController: Got move E7
            2021-11-04 13:10:02.1524|INFO|Logger|// Get Point: Point(X=8, Y=4, str=e3)
            2021-11-04 13:10:02.1524|INFO|Logger|// MakeMove: e3, Current Player: Black
            2021-11-04 13:10:02.1524|INFO|Logger|<- move e3
            2021-11-04 13:10:02.2881|INFO|Logger|-> move E6
            2021-11-04 13:10:02.2881|INFO|Logger|// Game loop: Got command move E6
            2021-11-04 13:10:02.2881|INFO|Logger|// MoveController: Got move E6
            2021-11-04 13:10:02.3354|INFO|Logger|// Get Point: Point(X=8, Y=6, str=e4)
            2021-11-04 13:10:02.3354|INFO|Logger|// MakeMove: e4, Current Player: Black
            2021-11-04 13:10:02.3354|INFO|Logger|<- move e4
            2021-11-04 13:10:02.4686|INFO|Logger|-> move E5
            2021-11-04 13:10:02.4686|INFO|Logger|// Game loop: Got command move E5
            2021-11-04 13:10:02.4704|INFO|Logger|// MoveController: Got move E5
            2021-11-04 13:10:02.5111|INFO|Logger|// Get Point: Point(X=8, Y=8, str=e5)
            2021-11-04 13:10:02.5111|INFO|Logger|// MakeMove: e5, Current Player: Black
            2021-11-04 13:10:02.5111|INFO|Logger|// Jump
            2021-11-04 13:10:02.5111|INFO|Logger|// Get point Point(X=8, Y=8, str=e5)
            2021-11-04 13:10:02.5179|INFO|Logger|// Point(X=8, Y=10, str=e6)
            2021-11-04 13:10:02.5179|INFO|Logger|<- jump e6
            2021-11-04 13:10:02.6339|INFO|Logger|-> move E4
            2021-11-04 13:10:02.6339|INFO|Logger|// Game loop: Got command move E4
            2021-11-04 13:10:02.6339|INFO|Logger|// MoveController: Got move E4
            2021-11-04 13:10:02.6579|INFO|Logger|// Get Point: Point(X=8, Y=12, str=e7)
            2021-11-04 13:10:02.6579|INFO|Logger|// MakeMove: e7, Current Player: Black
            2021-11-04 13:10:02.6579|INFO|Logger|<- move e7
            2021-11-04 13:10:02.7590|INFO|Logger|-> move E3
            2021-11-04 13:10:02.7590|INFO|Logger|// Game loop: Got command move E3
            2021-11-04 13:10:02.7590|INFO|Logger|// MoveController: Got move E3
            2021-11-04 13:10:02.7808|INFO|Logger|// Get Point: Point(X=8, Y=14, str=e8)
            2021-11-04 13:10:02.7808|INFO|Logger|// MakeMove: e8, Current Player: Black
            2021-11-04 13:10:02.7808|INFO|Logger|<- move e8
            2021-11-04 13:10:02.8593|INFO|Logger|-> wall W3h
            2021-11-04 13:10:02.8593|INFO|Logger|// Game loop: Got command wall W3h
            2021-11-04 13:10:02.8809|INFO|Logger|// Get Point: Point(X=8, Y=12, str=e7)
            2021-11-04 13:10:02.8809|INFO|Logger|// MakeMove: e7, Current Player: Black
            2021-11-04 13:10:02.8809|INFO|Logger|<- move e7
            2021-11-04 13:10:02.9567|INFO|Logger|-> move E2
            2021-11-04 13:10:02.9567|INFO|Logger|// Game loop: Got command move E2
            2021-11-04 13:10:02.9567|INFO|Logger|// MoveController: Got move E2
            2021-11-04 13:10:02.9818|INFO|Logger|// Get Point: Point(X=8, Y=14, str=e8)
            2021-11-04 13:10:02.9818|INFO|Logger|// MakeMove: e8, Current Player: Black
            2021-11-04 13:10:02.9818|INFO|Logger|<- move e8
            2021-11-04 13:10:03.0407|INFO|Logger|-> wall V3v
            2021-11-04 13:10:03.0407|INFO|Logger|// Game loop: Got command wall V3v
            2021-11-04 13:10:03.0712|INFO|Logger|// Get Point: Point(X=8, Y=4, str=e3)
            2021-11-04 13:10:03.0712|INFO|Logger|// MakeMove: e3, Current Player: White
            2021-11-04 13:10:03.0712|INFO|Logger|<- move e3
            2021-11-04 13:10:03.0712|ERROR|Logger|Internal error occured:    at Quoridor.AiTester.QuoridorGameRunner.Play(ILogger logger, StreamWriter input, StreamReader output)
   at IntroToGameDev.AiTester.SingleTestExecutor.Play(Process process)
         */
        public void FailedTest3Automatic()
        {
            FakeIo.Write("black");
            FakeIo.Write("move E8");
            FakeIo.Write("move E7");
            FakeIo.Write("move E6");
            FakeIo.Write("move E5");
            FakeIo.Write("move E4");
            FakeIo.Write("move E3");
            FakeIo.Write("wall W3h");
            FakeIo.Write("move E3");
            FakeIo.Write("wall V3v");
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

        /*
         *  2021-11-04 13:37:05.6842|INFO|Logger|-> wall U8v
            2021-11-04 13:37:05.6842|INFO|Logger|// Start game
            2021-11-04 13:37:05.6842|INFO|Logger|// GOT black
            2021-11-04 13:37:05.6842|INFO|Logger|// Game loop: Got command wall U8v
            2021-11-04 13:37:05.7876|INFO|Logger|// Get Point: Point(X=8, Y=2, str=e2)
            2021-11-04 13:37:05.7876|INFO|Logger|// MakeMove: e2, Current Player: Black
            2021-11-04 13:37:05.7876|INFO|Logger|<- move e2
            2021-11-04 13:37:05.9126|INFO|Logger|-> move E8
            2021-11-04 13:37:05.9126|INFO|Logger|// Game loop: Got command move E8
            2021-11-04 13:37:05.9126|INFO|Logger|// MoveController: Got move E8
            2021-11-04 13:37:05.9883|INFO|Logger|// Get Point: Point(X=8, Y=4, str=e3)
            2021-11-04 13:37:05.9883|INFO|Logger|// MakeMove: e3, Current Player: Black
            2021-11-04 13:37:05.9883|INFO|Logger|<- move e3
            2021-11-04 13:37:06.1128|INFO|Logger|-> move E7
            2021-11-04 13:37:06.1128|INFO|Logger|// Game loop: Got command move E7
            2021-11-04 13:37:06.1128|INFO|Logger|// MoveController: Got move E7
            2021-11-04 13:37:06.1564|INFO|Logger|// Get Point: Point(X=8, Y=6, str=e4)
            2021-11-04 13:37:06.1564|INFO|Logger|// MakeMove: e4, Current Player: Black
            2021-11-04 13:37:06.1564|INFO|Logger|<- move e4
            2021-11-04 13:37:06.2806|INFO|Logger|-> move E6
            2021-11-04 13:37:06.2806|INFO|Logger|// Game loop: Got command move E6
            2021-11-04 13:37:06.2806|INFO|Logger|// MoveController: Got move E6
            2021-11-04 13:37:06.3142|INFO|Logger|// Get Point: Point(X=8, Y=8, str=e5)
            2021-11-04 13:37:06.3145|INFO|Logger|// MakeMove: e5, Current Player: Black
            2021-11-04 13:37:06.3145|INFO|Logger|<- move e5
            2021-11-04 13:37:06.4376|INFO|Logger|-> wall W6h
            2021-11-04 13:37:06.4398|INFO|Logger|// Game loop: Got command wall W6h
            2021-11-04 13:37:06.4654|INFO|Logger|// Get Point: Point(X=8, Y=10, str=e6)
            2021-11-04 13:37:06.4654|INFO|Logger|// MakeMove: e6, Current Player: Black
            2021-11-04 13:37:06.4654|INFO|Logger|// Jump
            2021-11-04 13:37:06.4654|INFO|Logger|// Get point Point(X=8, Y=10, str=e6)
            2021-11-04 13:37:06.4654|INFO|Logger|// Point(X=8, Y=12, str=e7)
            2021-11-04 13:37:06.4654|INFO|Logger|<- jump e7
            2021-11-04 13:37:06.4654|ERROR|Logger|Internal error occured:    at Quoridor.AiTester.QuoridorGameRunner.Play(ILogger logger, StreamWriter input, StreamReader output)
               at IntroToGameDev.AiTester.SingleTestExecutor.Play(Process process)
         */
        [Test]
        public void FailedTest4Automatic()
        {
            FakeIo.Write("black");
            FakeIo.Write("wall U8v");
            FakeIo.Write("move E8");
            FakeIo.Write("move E7");
            FakeIo.Write("move E6");
            FakeIo.Write("wall W6h");
            FakeIo.Write("STOP");
            try
            {
                GameFlow.StartGame();
            }
            catch (Exception)
            {
                Assert.False(false);
            }
        }
        
        [Test]
        public void FailedTest5Automatic()
        {
            FakeIo.Write("black");
            FakeIo.Write("wall U8v");
            FakeIo.Write("STOP");
            try
            {
                GameFlow.StartGame();
            }
            catch (Exception)
            {
                Assert.False(false);
            }
        }
    }
}