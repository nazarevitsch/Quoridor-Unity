using System.Collections.Generic;
using models;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    
    private MapGenerator MapGenerator;
    private GameInstances gameInstances;
    
    public bool withPc;
    public bool isPaused;
    private bool hasWay;

    public Player CurrentPlayer { get; private set; }
    private Player enemyPlayer;
    
    public bool putBlock;
    public bool firstBlockWasPut;
    
    public delegate void RenderWalls(bool blocked);
    public delegate void ChangePossiblePlatforms(Player currentPlayer, Player opponent, Point [][] points, bool destroyed);
    public delegate void EndGame(string text);

    public event RenderWalls OnRenderWalls;
    public event ChangePossiblePlatforms OnChangePossiblePlatforms;
    public event EndGame OnEndGame;
    
    
    
    private void Awake()
    {
        MapGenerator = FindObjectOfType<MapGenerator>();
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
        OnChangePossiblePlatforms?.Invoke(CurrentPlayer, enemyPlayer, gameInstances.points, true);
        if (CheckWin(CurrentPlayer))
        {
            return;
        }
        ChangePlayers();
        ChangeButtons();
        OnChangePossiblePlatforms?.Invoke(CurrentPlayer, enemyPlayer, gameInstances.points, false);
    }

    public void PutBlock(GameObject gameObject)
    {
        if (putBlock && CurrentPlayer.BlocksCount > 0)
        {
            CoordinateScript cs = gameObject.GetComponent<CoordinateScript>();
            if (firstBlockWasPut)
            {
                if (gameObject.CompareTag("PossibleToBlock"))
                {
                    gameObject.tag = "Blocked";
                    gameObject.GetComponent<SpriteRenderer>().color = Color.magenta;
                    CurrentPlayer.BlocksCount = CurrentPlayer.BlocksCount - 1;
                    OnRenderWalls?.Invoke(true);  //ChangeWalls() -> blocked;
                    ChangePlayers();
                    Rewrite();
                    OnChangePossiblePlatforms?.Invoke(CurrentPlayer, enemyPlayer, gameInstances.points, false);
                    firstBlockWasPut = false;
                    putBlock = false;
                }
            }
            else
            {
                bool flag = false;
                if (cs.Y % 2 == 0)
                {
                    if (cs.Y > 0)
                    {
                        if (gameInstances.points[cs.Y - 2][cs.X].GameObject.CompareTag("Unblocked")
                            && (gameInstances.points[cs.Y - 1][cs.X + 1].GameObject.CompareTag("Unblocked")
                            || gameInstances.points[cs.Y - 1][cs.X - 1].GameObject.CompareTag("Unblocked")))
                        {
                            gameInstances.points[cs.Y - 2][cs.X].GameObject.tag = "PossibleToBlock";
                            gameInstances.points[cs.Y - 2][cs.X].GameObject.GetComponent<SpriteRenderer>().color =
                                Color.yellow;
                            flag = true;
                        }
                    }
                    if (cs.Y < 16)
                    {
                        if (gameInstances.points[cs.Y + 2][cs.X].GameObject.CompareTag("Unblocked")
                            && (gameInstances.points[cs.Y + 1][cs.X + 1].GameObject.CompareTag("Unblocked")
                            || gameInstances.points[cs.Y + 1][cs.X - 1].GameObject.CompareTag("Unblocked")))
                        {
                            gameInstances.points[cs.Y + 2][cs.X].GameObject.tag = "PossibleToBlock";
                            gameInstances.points[cs.Y + 2][cs.X].GameObject.GetComponent<SpriteRenderer>().color =
                                Color.yellow;
                            flag = true;
                        }
                    }
                }
                else
                {
                    if (cs.X > 0 && gameInstances.points[cs.Y ][cs.X - 2].GameObject.CompareTag("Unblocked") 
                                && gameInstances.points[cs.Y + 1][cs.X - 1].GameObject.CompareTag("Unblocked") 
                                && gameInstances.points[cs.Y - 1][cs.X - 1].GameObject.CompareTag("Unblocked"))
                    {
                        gameInstances.points[cs.Y][cs.X - 2].GameObject.tag = "PossibleToBlock";
                        gameInstances.points[cs.Y][cs.X - 2].GameObject.GetComponent<SpriteRenderer>().color = Color.yellow;
                        flag = true;
                    }
                    if (cs.X < 16 && gameInstances.points[cs.Y ][cs.X + 2].GameObject.CompareTag("Unblocked") 
                                  && gameInstances.points[cs.Y + 1][cs.X + 1].GameObject.CompareTag("Unblocked") 
                                  && gameInstances.points[cs.Y - 1][cs.X + 1].GameObject.CompareTag("Unblocked"))
                    {
                        gameInstances.points[cs.Y][cs.X + 2].GameObject.tag = "PossibleToBlock";
                        gameInstances.points[cs.Y][cs.X + 2].GameObject.GetComponent<SpriteRenderer>().color = Color.yellow;
                        flag = true;
                    }
                }
                if (flag)
                {
                    gameObject.tag = "HalfBlocked";
                    bool flag1 = HasWayToWin(CurrentPlayer);
                    Clean();
                    bool flag2 = HasWayToWin(enemyPlayer);
                    Clean();
                    if (flag1 && flag2)
                    {
                        gameObject.GetComponent<SpriteRenderer>().color = Color.magenta;
                        firstBlockWasPut = true;
                        OnChangePossiblePlatforms?.Invoke(CurrentPlayer, enemyPlayer, gameInstances.points, true);   
                    }
                    else
                    {
                        gameObject.tag = "Unblocked";
                        OnRenderWalls?.Invoke(true);
                    }
                }
            }
        }
    }

    private void StartGame()
    {
        isPaused = false;
        putBlock = false;
        firstBlockWasPut = false;
        gameInstances = MapGenerator.CreateLevel();
        CurrentPlayer = gameInstances.player1;
        enemyPlayer = gameInstances.player2;
        OnChangePossiblePlatforms?.Invoke(CurrentPlayer, enemyPlayer, gameInstances.points, false);
    }
    
    private void GenerateDesk()
    {
        Vector3 spawnPosition = new Vector3(-6f, 2.5f, 0f);
        var points = new Point[17][];
        for (int i = 0; i < 17; i++)
        {
            points[i] = new Point[17];
            if (i % 2 == 0)
            {
                for (int j = 0; j < 9; j++)
                {
                    points[i][j * 2] = new Point(null,j * 2, i);
                    if (j != 8)
                    {
                        spawnPosition.x += 0.3f;
                        points[i][j * 2 + 1] = new Point(null,j * 2 + 1  ,i);
                    }
                }
            }
            else
            {
                for (int j = 0; j < 9; j++)
                {
                    points[i][j * 2] = new Point(null,j * 2 , i);
                }
            }
        }
    }
    
    public void RestartGame()
    {
        DestroyObjects();
        OnEndGame?.Invoke("Nobody - WON");
    }
    

    public List<Point> FindPossiblePlatforms(Player player, Player opponent, Point[][] points)
    {
        List<Point> list = new List<Point>();
        if (player.CurrentY > 0)
        {
            if (points[player.CurrentY - 1][player.CurrentX].GameObject.CompareTag("Unblocked"))
            {
                if (opponent.CurrentX == player.CurrentX && opponent.CurrentY + 2 == player.CurrentY)
                {
                    if (opponent.CurrentY != 0)
                    {
                        if (points[player.CurrentY - 3][player.CurrentX].GameObject.CompareTag("Unblocked"))
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
            if (points[player.CurrentY + 1][player.CurrentX].GameObject.CompareTag("Unblocked"))
            {
                if (opponent.CurrentX == player.CurrentX && opponent.CurrentY - 2 == player.CurrentY)
                {
                    if (opponent.CurrentY != 16)
                    {
                        if (points[player.CurrentY + 3][player.CurrentX].GameObject.CompareTag("Unblocked"))
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
            if (points[player.CurrentY][player.CurrentX - 1].GameObject.CompareTag("Unblocked"))
            {
                if (opponent.CurrentX + 2 == player.CurrentX && opponent.CurrentY == player.CurrentY)
                {
                    if (opponent.CurrentX != 0)
                    {
                        if (points[player.CurrentY][player.CurrentX - 3].GameObject.CompareTag("Unblocked"))
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
            if (points[player.CurrentY][player.CurrentX + 1].GameObject.CompareTag("Unblocked"))
            {
                if (opponent.CurrentX - 2 == player.CurrentX && opponent.CurrentY == player.CurrentY)
                {
                    if (opponent.CurrentX != 16)
                    {
                        if (points[player.CurrentY][player.CurrentX + 3].GameObject.CompareTag("Unblocked"))
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
            if (points[player.CurrentY - 1][player.CurrentX].GameObject.CompareTag("Unblocked"))
            {

                list.Add(points[player.CurrentY - 2][player.CurrentX]);

            }
        }
        if (player.CurrentY < 16)
        {
            if (points[player.CurrentY + 1][player.CurrentX].GameObject.CompareTag("Unblocked"))
            {
                list.Add(points[player.CurrentY + 2][player.CurrentX]);
                
            }
        }
        if (player.CurrentX > 0)
        {
            if (points[player.CurrentY][player.CurrentX - 1].GameObject.CompareTag("Unblocked"))
            {
                list.Add(points[player.CurrentY][player.CurrentX - 2]);
            }
        }
        if (player.CurrentX < 16)
        {
            if (points[player.CurrentY][player.CurrentX + 1].GameObject.CompareTag("Unblocked"))
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
            DestroyObjects();
            OnEndGame?.Invoke(CurrentPlayer.Name + " - WON");
            return true;
        }
        if (player.StartY == 16 && player.CurrentY == 0)
        {
            DestroyObjects();
            OnEndGame?.Invoke(CurrentPlayer.Name + " - WON");
            return true;
        }
        return false;
    }

    private void DestroyObjects()
    {
        for (int i = 0; i < 17; i++)
        {
            for (int j = 0; j < 17; j++)
            {
                if (gameInstances.points[i][j] != null)
                {
                    Destroy(gameInstances.points[i][j].GameObject);
                }
            }
        } 
        Destroy(CurrentPlayer.player);
        Destroy(enemyPlayer.player);
    }
    
    public void ChangePuttBlockState()
    {
        if (!isPaused)
        {
            putBlock = !putBlock;
            if (putBlock)
            {
                OnChangePossiblePlatforms?.Invoke(CurrentPlayer,enemyPlayer, gameInstances.points, true);
            }
            else
            {
                OnChangePossiblePlatforms?.Invoke(CurrentPlayer, enemyPlayer, gameInstances.points, false);
            }

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
        CurrentPlayer = enemyPlayer;
        enemyPlayer = p;  
    }

    private void Rewrite()
    {
        CurrentPlayer.BlocksCountField.GetComponent<Text>().text = CurrentPlayer.Name + ": " + CurrentPlayer.BlocksCount;
        
        enemyPlayer.BlocksCountField.GetComponent<Text>().text = enemyPlayer.Name + ": " + enemyPlayer.BlocksCount;
        enemyPlayer.BlocksUseButton.GetComponent<ButtonHandler>().ChangeText();
        ChangeButtons();
    }

    private void ChangeButtons()
    {
        if (CurrentPlayer.BlocksCount > 0)
        {
            CurrentPlayer.BlocksUseButton.SetActive(true);
        }
        enemyPlayer.BlocksUseButton.SetActive(false);
    }

    public void Clean()
    {
        hasWay = false;
        for (int i = 0; i < 17; i += 2)
        {
            for (int j = 0; j < 9; j++)
            {
                gameInstances.points[i][j * 2].IsVisited = false;
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
        Player temporalPLayer = new Player(player.player, player.StartY, player.FinishY, player.CurrentX,
            player.CurrentY, player.BlocksCount, player.Name);
        Point currentPoint = gameInstances.points[player.CurrentY][player.CurrentX];
        currentPoint.IsVisited = true;
        List<Point> list = FindPossiblePlatformsForWay(temporalPLayer, gameInstances.points);
        foreach (var point in list)
        {
            if (!point.IsVisited)
            {
                temporalPLayer.CurrentX = point.GameObject.GetComponent<CoordinateScript>().X;
                temporalPLayer.CurrentY = point.GameObject.GetComponent<CoordinateScript>().Y;
                point.IsVisited = true;
                FindWayToWin(temporalPLayer);
            }
        }
    }
}