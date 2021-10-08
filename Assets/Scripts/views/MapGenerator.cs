using models;
using store;
using UnityEngine;
using UnityEngine.UI;
using Color = UnityEngine.Color;

public class MapGenerator : MonoBehaviour
{
    public GameObject platform;
    public GameObject wall;
    public GameObject player;
    public GameObject blocksCount1;
    public GameObject blocksCount2;
    public GameObject useBlock1;
    public GameObject useBlock2;

    private Game _game;

    public void Awake()
    {
        _game = FindObjectOfType<Game>();
        _game.OnStartGame += (p1, p2, points) => CreateLevel(p1, p2);
        _game.OnEndGame += t => DestroyObjects();
    }

    public void CreateLevel(Player pl1, Player pl2)
    {
        GenerateDesk();
        SpawnPlayers(pl1, pl2);
        GenerateInformation(pl1, pl2);
        useBlock2.SetActive(false);
    }

    private void GenerateDesk()
    {
        var objStore = FindObjectOfType<UnityObjStore>();
        Vector3 spawnPosition = new Vector3(-6f, 2.5f, 0f);
        for (int i = 0; i < 17; i++)
        {
            if (i % 2 == 0)
            {
                for (int j = 0; j < 9; j++)
                {
                    spawnPosition.x += 0.3f;
                    var gameObj = Instantiate(platform, spawnPosition, Quaternion.identity);
                    var cs = gameObj.GetComponent<CoordinateScript>();
                    cs.X = j * 2; cs.Y = i;
                    gameObj.GetComponent<SpriteRenderer>().color = Color.black;
                    objStore.AddPoint(j * 2, i, gameObj);
                    if (j != 8)
                    {
                        spawnPosition.x += 0.3f;
                        gameObj = Instantiate(wall, spawnPosition, Quaternion.Euler(0f, 0f, 90f));
                        cs = gameObj.GetComponent<CoordinateScript>();
                        cs.X = j * 2 + 1; cs.Y = i;
                        objStore.AddPoint(j * 2 + 1, i, gameObj);
                    }
                }
                spawnPosition.x = -6f;
                spawnPosition.y -= 0.3f;
            }
            else
            {
                for (int j = 0; j < 9; j++)
                {
                    spawnPosition.x += 0.3f;
                    var gameObj = Instantiate(wall, spawnPosition, Quaternion.identity);
                    var cs = gameObj.GetComponent<CoordinateScript>();
                    cs.X = j * 2; cs.Y = i;
                    objStore.AddPoint(j * 2, i, gameObj);
                    spawnPosition.x += 0.3f;
                }
                spawnPosition.x = -6f;
                spawnPosition.y -= 0.3f;
            }
        }
    }

    private void DestroyObjects()
    {
        var objStore = FindObjectOfType<UnityObjStore>();
        for (int i = 0; i < 17; i++)
        {
            for (int j = 0; j < 17; j++)
            {
                if (objStore.PointGameObjects[i][j] != null)
                {
                    Destroy(objStore.PointGameObjects[i][j]);
                }
            }
        } 
        Destroy(objStore.PlayerObjects["Player 1"]);
        Destroy(objStore.PlayerObjects["Player 2"]);
    }
    
    private void SpawnPlayers(Player player1, Player player2)
    {
        var objStore = FindObjectOfType<UnityObjStore>();
        player.GetComponent<SpriteRenderer>().color = Color.red;
        Vector3 positionPlayer1 = objStore.PointGameObjects[16][8].transform.position;
        var playerObj = Instantiate(player, positionPlayer1, Quaternion.identity);
        objStore.AddPlayer("Player 1", playerObj);
        player1.BlocksCountField = blocksCount1;
        player1.BlocksUseButton = useBlock1;
        
        player.GetComponent<SpriteRenderer>().color = Color.cyan;
        
        Vector3 positionPlayer2 = objStore.PointGameObjects[0][8].transform.position;
        playerObj = Instantiate(player, positionPlayer2, Quaternion.identity);
        objStore.AddPlayer("Player 2", playerObj);
        player2.BlocksCountField = blocksCount2;
        player2.BlocksUseButton = useBlock2;
    }

    private void GenerateInformation(Player player1, Player player2)
    {
        blocksCount1.GetComponent<Text>().text = player1.Name + ": " + player1.BlocksCount;
        blocksCount2.GetComponent<Text>().text = player2.Name + ": " + player2.BlocksCount;
    }
}
