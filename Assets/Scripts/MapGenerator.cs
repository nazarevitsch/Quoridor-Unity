using DefaultNamespace;
using UnityEngine;
using UnityEngine.UI;
using Color = UnityEngine.Color;
using Point = DefaultNamespace.Point;

public class MapGenerator : MonoBehaviour
{
    public GameObject platform;
    public GameObject wall;
    public GameObject player;
    public GameObject blocksCount1;
    public GameObject blocksCount2;
    public GameObject useBlock1;
    public GameObject useBlock2;

    public Point[][] points;
    public Player Player1;
    public Player Player2;
    
    public GameInstances CreateLevel()
    {
        GenerateDesk();
        SpawnPlayers();
        GenerateInformation();
        useBlock2.SetActive(false);
        return new GameInstances(points, Player1, Player2);
    }

    private void GenerateDesk()
    {
        Vector3 spawnPosition = new Vector3(-6f, 2.5f, 0f);
        points = new Point[17][];
        for (int i = 0; i < 17; i++)
        {
            points[i] = new Point[17];
            if (i % 2 == 0)
            {
                for (int j = 0; j < 9; j++)
                {
                    spawnPosition.x += 0.3f;
                    points[i][j * 2] = new Point(Instantiate(platform, spawnPosition, Quaternion.identity));
                    points[i][j * 2].GameObject.GetComponent<CoordinateScript>().Y = i;
                    points[i][j * 2].GameObject.GetComponent<CoordinateScript>().X = j * 2;
                    points[i][j * 2].GameObject.GetComponent<SpriteRenderer>().color = Color.black;
                    points[i][j * 2 ].GameObject.tag = "Platform";
                    if (j != 8)
                    {
                        spawnPosition.x += 0.3f;
                        points[i][j * 2 + 1] = new Point(Instantiate(wall, spawnPosition, Quaternion.Euler(0f, 0f, 90f)));
                        points[i][j * 2 + 1].GameObject.GetComponent<CoordinateScript>().Y = i;
                        points[i][j * 2 + 1].GameObject.GetComponent<CoordinateScript>().X = j * 2 + 1;
                        points[i][j * 2 + 1].GameObject.tag = "Unblocked";
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
                    points[i][j * 2] = new Point(Instantiate(wall, spawnPosition, Quaternion.identity));
                    points[i][j * 2].GameObject.GetComponent<CoordinateScript>().Y = i;
                    points[i][j * 2].GameObject.GetComponent<CoordinateScript>().X = j * 2;
                    points[i][j * 2].GameObject.tag = "Unblocked";
                    spawnPosition.x += 0.3f;
                }
                spawnPosition.x = -6f;
                spawnPosition.y -= 0.3f;
            }
        }
    }

    private void SpawnPlayers()
    {
        player.GetComponent<SpriteRenderer>().color = Color.red;
        
        Vector3 positionPlayer1 = points[16][8].GameObject.transform.position;
        Player1 = new Player(Instantiate(player, positionPlayer1, Quaternion.identity), 16, 0, 8, 16, 9);
        Player1.player.name = "Player 1";
        Player1.BlocksCountField = blocksCount1;
        Player1.BlocksUseButton = useBlock1;
        
        player.GetComponent<SpriteRenderer>().color = Color.cyan;
        
        Vector3 positionPlayer2 = points[4][8].GameObject.transform.position;
        Player2 = new Player(Instantiate(player, positionPlayer2, Quaternion.identity), 0, 16, 8, 4, 9);
        Player2.player.name = "Player 2";
        Player2.BlocksCountField = blocksCount2;
        Player2.BlocksUseButton = useBlock2;
    }

    private void GenerateInformation()
    {
        blocksCount1.GetComponent<Text>().text = Player1.player.name + ": " + Player1.BlocksCount;
        blocksCount2.GetComponent<Text>().text = Player2.player.name + ": " + Player2.BlocksCount;
    }
}
