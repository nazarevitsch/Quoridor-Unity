using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    private UIControllerScript UIControllerScript;
    
    private MapGenerator MapGenerator;
    private GameInstances gameInstances;
    
    public bool withPc;
    public bool isPaused;
    private bool hasWay;

    private Player currentPlayer;
    private Player enemyPlayer;
    
    public bool putBlock;
    public bool firstBlockWasPut;
    private void Awake()
    {
        MapGenerator = GameObject.FindObjectOfType<MapGenerator>();
        UIControllerScript = GameObject.FindObjectOfType<UIControllerScript>();
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
    
    public void DoStep(GameObject gameObject)
    {
        Vector3 destination = gameObject.transform.position;
        CoordinateScript coordinates = gameObject.gameObject.GetComponent<CoordinateScript>();
        currentPlayer.CurrentY = coordinates.Y;
        currentPlayer.CurrentX = coordinates.X;
        currentPlayer.player.transform.position = destination;
        DestroyPossiblePlatforms();
        if (CheckWin(currentPlayer))
        {
            return;
        }
        ChangePlayers();
        ChangeButtons();
        CreatePossiblePlatforms(currentPlayer, enemyPlayer, gameInstances.points);
    }

    public void PutBlock(GameObject gameObject)
    {
        if (putBlock && currentPlayer.BlocksCount > 0)
        {
            CoordinateScript cs = gameObject.GetComponent<CoordinateScript>();
            if (firstBlockWasPut)
            {
                if (gameObject.CompareTag("PossibleToBlock"))
                {
                    gameObject.tag = "Blocked";
                    gameObject.GetComponent<SpriteRenderer>().color = Color.magenta;
                    currentPlayer.BlocksCount = currentPlayer.BlocksCount - 1;
                    ChangeWalls();
                    ChangePlayers();
                    Rewrite();
                    CreatePossiblePlatforms(currentPlayer, enemyPlayer, gameInstances.points);
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
                    bool flag1 = HasWayToWin(currentPlayer);
                    Clean();
                    bool flag2 = HasWayToWin(enemyPlayer);
                    Clean();
                    if (flag1 && flag2)
                    {
                        gameObject.GetComponent<SpriteRenderer>().color = Color.magenta;
                        firstBlockWasPut = true;
                        DestroyPossiblePlatforms();   
                    }
                    else
                    {
                        gameObject.tag = "Unblocked";
                        ChangeWalls();
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
        currentPlayer = gameInstances.player1;
        enemyPlayer = gameInstances.player2;
        CreatePossiblePlatforms(currentPlayer, enemyPlayer, gameInstances.points);
    }
    
    public void RestartGame()
    {
        DestroyObjects();
        UIControllerScript.HideUI("Nobody - WON");
    }

    private void CreatePossiblePlatforms(Player player, Player opponent, Point[][] points)
    {
        List<Point> list = FindPossiblePlatforms(player, opponent, points);
        foreach (var ob in list)
        {
            ob.GameObject.GetComponent<SpriteRenderer>().color = Color.gray;
            ob.GameObject.tag = "Possible";
        }
    }

    private List<Point> FindPossiblePlatforms(Player player, Player opponent, Point[][] points)
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
            UIControllerScript.HideUI(currentPlayer.player.name + " - WON");
            return true;
        }
        if (player.StartY == 16 && player.CurrentY == 0)
        {
            DestroyObjects();
            UIControllerScript.HideUI(currentPlayer.player.name + " - WON");
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
        Destroy(currentPlayer.player);
        Destroy(enemyPlayer.player);
    }
    
    private void DestroyPossiblePlatforms()
    {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Possible");
        for (int i = 0; i < gameObjects.Length; i++)
        {
            gameObjects[i].tag = "Platform";
            gameObjects[i].GetComponent<SpriteRenderer>().color = Color.black;
        }
    }

    private void ChangeWalls()
    {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("PossibleToBlock");
        for (int i = 0; i < gameObjects.Length; i++)
        {
            gameObjects[i].tag = "Unblocked";
            gameObjects[i].GetComponent<SpriteRenderer>().color = Color.white;
        }
        gameObjects = GameObject.FindGameObjectsWithTag("HalfBlocked");
        for (int i = 0; i < gameObjects.Length; i++)
        {
            gameObjects[i].tag = "Blocked";
        }
    }
    
    private void ChangeWalls2()
    {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("PossibleToBlock");
        for (int i = 0; i < gameObjects.Length; i++)
        {
            gameObjects[i].tag = "Unblocked";
            gameObjects[i].GetComponent<SpriteRenderer>().color = Color.white;
        }
        gameObjects = GameObject.FindGameObjectsWithTag("HalfBlocked");
        for (int i = 0; i < gameObjects.Length; i++)
        {
            gameObjects[i].tag = "Unblocked";
            gameObjects[i].GetComponent<SpriteRenderer>().color = Color.white;
        }
    }

    public void ChangePuttBlockState()
    {
        if (!isPaused)
        {
            putBlock = !putBlock;
            if (putBlock)
            {
                DestroyPossiblePlatforms();
            }
            else
            {
                CreatePossiblePlatforms(currentPlayer, enemyPlayer, gameInstances.points);
            }

            if (firstBlockWasPut)
            {
                firstBlockWasPut = false;
                ChangeWalls2();
            }
        }
    }

    public void ChangePauseState()
    {
        isPaused = !isPaused;
    }

    private void ChangePlayers()
    {
        Player p = currentPlayer;
        currentPlayer = enemyPlayer;
        enemyPlayer = p;  
    }

    private void Rewrite()
    {
        currentPlayer.BlocksCountField.GetComponent<Text>().text = currentPlayer.player.name + ": " + currentPlayer.BlocksCount;
        
        enemyPlayer.BlocksCountField.GetComponent<Text>().text = enemyPlayer.player.name + ": " + enemyPlayer.BlocksCount;
        enemyPlayer.BlocksUseButton.GetComponent<ButtonHandler>().ChangeText();
        ChangeButtons();
    }

    private void ChangeButtons()
    {
        if (currentPlayer.BlocksCount > 0)
        {
            currentPlayer.BlocksUseButton.SetActive(true);
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
            player.CurrentY, player.BlocksCount);
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