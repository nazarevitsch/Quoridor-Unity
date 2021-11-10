using System;
using System.Text;
using AStar.models;

namespace AStar.Views
{
    public class FieldView
    {
        public Game Game { get; set; }

        public FieldView(Game game)
        {
            Game = game;
            Game.OnWallPlaced += _ => RenderGame();
            Game.OnPlayersChanged += (_, _) => RenderGame();
        }

        public void RenderGame()
        {
            var sb = new StringBuilder(50);
            foreach (var gamePoints in Game.Points)
            {
                foreach (var gamePoint in gamePoints)
                {
                    if (gamePoint is null)
                    {
                        sb.Append(". ");
                        continue;
                    }
                    if (gamePoint.Tag == "HalfBlocked" || gamePoint.Tag == "Blocked")
                    {
                        sb.Append(gamePoint.Y % 2 == 0 ? "|| " : "== ");
                    }
                    else
                    {
                        sb.Append(". ");
                    }

                    if (Game.CurrentPlayer.CurrentX == gamePoint.X && Game.CurrentPlayer.CurrentY == gamePoint.Y)
                    {
                        sb.Append(Game.CurrentPlayer.PlayColor == PlayColor.White ? "W " : "B ");
                    }
                    if (Game.EnemyPlayer.CurrentX == gamePoint.X && Game.EnemyPlayer.CurrentY == gamePoint.Y)
                    {
                        sb.Append(Game.EnemyPlayer.PlayColor == PlayColor.White ? "W " : "B ");
                    }
                }
                sb.Append("\n");
            }
            Console.WriteLine(sb.ToString());
        }
    }
}