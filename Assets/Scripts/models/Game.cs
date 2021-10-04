using System.Collections.Generic;
using UnityEngine;

namespace models
{
    public class Game : MonoBehaviour
    {
        private const int FieldSize = 17;
        public bool withPc;
        public bool isPaused;
        private bool hasWay;

        public Player CurrentPlayer { get; private set; }
        public Player EnemyPlayer { get; private set; }
    
        public Point[][] Points { get; private set; }
    
        public bool putBlock;
        public bool firstBlockWasPut;
    
        public delegate void RenderWalls(bool blocked);
        public delegate void ChangePossiblePlatforms(Player currentPlayer, Player opponent, Point [][] points, bool destroyed);
        public delegate void EndGame(string text);
        public delegate void StartGameE(Player curPlayer, Player opponent, Point [][] points);
        public delegate void PointTagChanged(string tag, int x, int y);
        public delegate void PlayersChanged(Player currentPlayer, Player opponent);

        public event RenderWalls OnRenderWalls;
        public event ChangePossiblePlatforms OnChangePossiblePlatforms;
        public event EndGame OnEndGame;
        public event StartGameE OnStartGame;
        public event PointTagChanged OnPointTagChanged;
        public event PlayersChanged OnPlayersChanged;
    
        private void Awake()
        {
            putBlock = false;
            firstBlockWasPut = false;
            hasWay = false;
        }

        public void PlayWithPC()
        {
            withPc = true;
            StartGame();
        }
    
        public void PlayWithFriend()
        {
            withPc = false;
            StartGame();
        }
    
        public void DoStep(Point point)
        {
            CurrentPlayer.CurrentY = point.Y;
            CurrentPlayer.CurrentX = point.X;
            OnChangePossiblePlatforms?.Invoke(CurrentPlayer, EnemyPlayer, Points, true);
            if (CheckWin(CurrentPlayer)) return;
            ChangePlayers();
            OnPlayersChanged?.Invoke(CurrentPlayer, EnemyPlayer);
            OnChangePossiblePlatforms?.Invoke(CurrentPlayer, EnemyPlayer, Points, false);
        }
        public void PutBlock(Point point)
        {
            if (putBlock && CurrentPlayer.BlocksCount > 0)
            {
                if (firstBlockWasPut)
                {
                    if (point.Tag == "PossibleToBlock")
                    {
                        point.Tag = "Blocked";
                        OnPointTagChanged?.Invoke("Blocked", point.X, point.Y);
                        CurrentPlayer.BlocksCount -= 1;
                        OnRenderWalls?.Invoke(true);  //ChangeWalls() -> blocked;
                        ChangePlayers();
                        OnPlayersChanged?.Invoke(CurrentPlayer, EnemyPlayer);
                        OnChangePossiblePlatforms?.Invoke(CurrentPlayer, EnemyPlayer, Points, false);
                        firstBlockWasPut = false;
                        putBlock = false;
                    }
                }
                else
                {
                    bool flag = false;
                    if (point.Y % 2 == 0)
                    {
                        if (point.Y > 0)
                        {
                            if (Points[point.Y - 2][point.X].Tag == "Unblocked"
                                && (Points[point.Y - 1][point.X + 1].Tag == "Unblocked"
                                    || Points[point.Y - 1][point.X - 1].Tag == "Unblocked"))
                            {
                                Points[point.Y - 2][point.X].Tag = "PossibleToBlock";
                                OnPointTagChanged?.Invoke("PossibleToBlock", point.X, point.Y - 2);
                                flag = true;
                            }
                        }
                        if (point.Y < 16)
                        {
                            if (Points[point.Y + 2][point.X].Tag == "Unblocked"
                                && (Points[point.Y + 1][point.X + 1].Tag == "Unblocked"
                                    || Points[point.Y + 1][point.X - 1].Tag == "Unblocked"))
                            {
                                Points[point.Y + 2][point.X].Tag = "PossibleToBlock";
                                OnPointTagChanged?.Invoke("PossibleToBlock", point.X, point.Y + 2);
                                flag = true;
                            }
                        }
                    }
                    else
                    {
                        if (point.X > 0 && Points[point.Y ][point.X - 2].Tag == "Unblocked"
                                        && Points[point.Y + 1][point.X - 1].Tag == "Unblocked"
                                        && Points[point.Y - 1][point.X - 1].Tag == "Unblocked")
                        {
                            Points[point.Y][point.X - 2].Tag = "PossibleToBlock";
                            OnPointTagChanged?.Invoke("PossibleToBlock", point.X - 2, point.Y);
                            flag = true;
                        }
                        if (point.X < 16 && Points[point.Y ][point.X + 2].Tag == "Unblocked"
                                         && Points[point.Y + 1][point.X + 1].Tag == "Unblocked"
                                         && Points[point.Y - 1][point.X + 1].Tag == "Unblocked")
                        {
                            Points[point.Y][point.X + 2].Tag = "PossibleToBlock";
                            OnPointTagChanged?.Invoke("PossibleToBlock", point.X + 2, point.Y);
                            flag = true;
                        }
                    }
                    if (flag)
                    {
                        point.Tag = "HalfBlocked";
                        bool flag1 = HasWayToWin(CurrentPlayer);
                        Clean();
                        bool flag2 = HasWayToWin(EnemyPlayer);
                        Clean();
                        if (flag1 && flag2)
                        {
                            firstBlockWasPut = true;
                            OnPointTagChanged?.Invoke("HalfBlocked", point.X, point.Y);
                            OnChangePossiblePlatforms?.Invoke(CurrentPlayer, EnemyPlayer, Points, true);   
                        }
                        else
                        {
                            point.Tag = "Unblocked";
                            OnPointTagChanged?.Invoke("Unblocked", point.X, point.Y);
                            OnRenderWalls?.Invoke(true);
                        }
                    }
                }
            }
        }

        private void StartGame()
        {
            isPaused = putBlock = firstBlockWasPut = false;
            SpawnPlayers();
            Points = GenerateDesk();
            OnStartGame?.Invoke(CurrentPlayer, EnemyPlayer, Points);
            OnChangePossiblePlatforms?.Invoke(CurrentPlayer, EnemyPlayer, Points, false);
        }
    
        private void SpawnPlayers()
        {
            CurrentPlayer = new Player( 16, 0, 8, 16, 9, "Player 1");
            EnemyPlayer = new Player( 0, 16, 8, 4, 9, "Player 2");
        }
    
        private Point[][] GenerateDesk()
        {
            var points = new Point[17][];
            for (int i = 0; i < 17; i++)
            {
                points[i] = new Point[17];
                if (i % 2 == 0)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        points[i][j * 2] = new Point(j * 2, i, "Platform");
                        if (j != 8)
                        {
                            points[i][j * 2 + 1] = new Point(j * 2 + 1  ,i, "Unblocked");
                        }
                    }
                }
                else
                {
                    for (int j = 0; j < 9; j++)
                    {
                        points[i][j * 2] = new Point(j * 2 , i, "Unblocked");
                    }
                }
            }

            return points;
        }
    
        public void RestartGame()
        {
            OnEndGame?.Invoke("Nobody - WON");
        }
    
        public List<Point> FindPossiblePlatforms(Player player, Player opponent, Point[][] points)
        {
            List<Point> list = new List<Point>();
            if (player.CurrentY > 0)
            {
                if (points[player.CurrentY - 1][player.CurrentX].Tag == "Unblocked")
                {
                    if (opponent.CurrentX == player.CurrentX && opponent.CurrentY + 2 == player.CurrentY)
                    {
                        if (opponent.CurrentY != 0)
                        {
                            if (points[player.CurrentY - 3][player.CurrentX].Tag == "Unblocked")
                            {
                                list.Add(points[player.CurrentY - 4][player.CurrentX]);
                            }
                        }
                    }
                    else
                    {
                        list.Add(points[player.CurrentY - 2][player.CurrentX]);
                    }
                }
            }
            if (player.CurrentY < 16)
            {
                if (points[player.CurrentY + 1][player.CurrentX].Tag == "Unblocked")
                {
                    if (opponent.CurrentX == player.CurrentX && opponent.CurrentY - 2 == player.CurrentY)
                    {
                        if (opponent.CurrentY != 16)
                        {
                            if (points[player.CurrentY + 3][player.CurrentX].Tag == "Unblocked")
                            {
                                list.Add(points[player.CurrentY + 4][player.CurrentX]);
                            }
                        }
                    }
                    else
                    {
                        list.Add(points[player.CurrentY + 2][player.CurrentX]);
                    }
                }
            }
            if (player.CurrentX > 0)
            {
                if (points[player.CurrentY][player.CurrentX - 1].Tag == "Unblocked")
                {
                    if (opponent.CurrentX + 2 == player.CurrentX && opponent.CurrentY == player.CurrentY)
                    {
                        if (opponent.CurrentX != 0)
                        {
                            if (points[player.CurrentY][player.CurrentX - 3].Tag == "Unblocked")
                            {
                                list.Add(points[player.CurrentY][player.CurrentX - 4]);
                            }
                        }
                    }
                    else
                    {
                        list.Add(points[player.CurrentY][player.CurrentX - 2]);
                    }
                }
            }
            if (player.CurrentX < 16)
            {
                if (points[player.CurrentY][player.CurrentX + 1].Tag == "Unblocked")
                {
                    if (opponent.CurrentX - 2 == player.CurrentX && opponent.CurrentY == player.CurrentY)
                    {
                        if (opponent.CurrentX != 16)
                        {
                            if (points[player.CurrentY][player.CurrentX + 3].Tag == "Unblocked")
                            {
                                list.Add(points[player.CurrentY][player.CurrentX + 4]);
                            }
                        }
                    }
                    else
                    {
                        list.Add(points[player.CurrentY][player.CurrentX + 2]);
                    }
                }
            }
            return list;
        }
    
        private List<Point> FindPossiblePlatformsForWay(Player player, Point[][] points)
        {
            List<Point> list = new List<Point>();
            if (player.CurrentY > 0)
            {
                if (points[player.CurrentY - 1][player.CurrentX].Tag == "Unblocked")
                {

                    list.Add(points[player.CurrentY - 2][player.CurrentX]);

                }
            }
            if (player.CurrentY < 16)
            {
                if (points[player.CurrentY + 1][player.CurrentX].Tag == "Unblocked")
                {
                    list.Add(points[player.CurrentY + 2][player.CurrentX]);
                
                }
            }
            if (player.CurrentX > 0)
            {
                if (points[player.CurrentY][player.CurrentX - 1].Tag == "Unblocked")
                {
                    list.Add(points[player.CurrentY][player.CurrentX - 2]);
                }
            }
            if (player.CurrentX < 16)
            {
                if (points[player.CurrentY][player.CurrentX + 1].Tag == "Unblocked")
                {
                    list.Add(points[player.CurrentY][player.CurrentX + 2]);

                }
            }
            return list;
        }

        private bool CheckWin(Player player)
        {
            if (player.StartY == 0 && player.CurrentY == 16)
            {
                OnEndGame?.Invoke(CurrentPlayer.Name + " - WON");
                return true;
            }
            if (player.StartY == 16 && player.CurrentY == 0)
            {
                OnEndGame?.Invoke(CurrentPlayer.Name + " - WON");
                return true;
            }
            return false;
        }
    
        public void ChangePuttBlockState()
        {
            if (!isPaused)
            {
                putBlock = !putBlock;
                OnChangePossiblePlatforms?.Invoke(CurrentPlayer, EnemyPlayer, Points, putBlock);

                if (firstBlockWasPut)
                {
                    firstBlockWasPut = false;
                    OnRenderWalls?.Invoke(false);
                }
            }
        }

        public void ChangePauseState()
        {
            isPaused = !isPaused;
        }

        private void ChangePlayers()
        {
            Player p = CurrentPlayer;
            CurrentPlayer = EnemyPlayer;
            EnemyPlayer = p;  
        }

        private void Clean()
        {
            hasWay = false;
            for (int i = 0; i < 17; i += 2)
            {
                for (int j = 0; j < 9; j++)
                {
                    Points[i][j * 2].IsVisited = false;
                }
            }
        }

        private bool HasWayToWin(Player player)
        {
            FindWayToWin(player);
            return hasWay;
        }

        private void FindWayToWin(Player player)
        {
            if (player.CurrentY == player.FinishY || hasWay)
            {
                hasWay = true;
                return;
            }
            Player temporalPLayer = new Player(player.StartY, player.FinishY, player.CurrentX,
                player.CurrentY, player.BlocksCount, player.Name);
            Point currentPoint = Points[player.CurrentY][player.CurrentX];
            currentPoint.IsVisited = true;
            List<Point> list = FindPossiblePlatformsForWay(temporalPLayer, Points);
            foreach (var point in list)
            {
                if (!point.IsVisited)
                {
                    temporalPLayer.CurrentX = point.X;
                    temporalPLayer.CurrentY = point.Y;
                    point.IsVisited = true;
                    FindWayToWin(temporalPLayer);
                }
            }
        }

        public Point[] FindPointsWithTag(string tagName)
        {
            var res = new List<Point>(FieldSize * 2);
            for (var y = 0; y < FieldSize; y++)
            {
                for (var x = 0; x < FieldSize; x++)
                {
                    if (Points[y][x]?.Tag == tagName)
                    {
                        res.Add(Points[y][x]);
                    }
                }
            }

            return res.ToArray();
        }
    }
}