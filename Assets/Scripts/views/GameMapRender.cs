using System.Collections.Generic;
using models;
using UnityEngine;

namespace views
{
    public class GameMapRender : MonoBehaviour
    {
        private Game _game;
        private void Awake()
        {
            _game = FindObjectOfType<Game>();
            _game.OnRenderWalls += ChangeWalls;
            _game.OnChangePossiblePlatforms += (player, opponent, points, destroyed) =>
            {
                if (destroyed)
                    DestroyPossiblePlatforms();
                else
                    CreatePossiblePlatforms(player, opponent, points);
            };
        }


        private void ChangeWalls(bool blocked)
        {
            GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("PossibleToBlock");
            foreach (var obj in gameObjects)
            {
                obj.tag = "Unblocked";
                obj.GetComponent<SpriteRenderer>().color = Color.white;
            }
            gameObjects = GameObject.FindGameObjectsWithTag("HalfBlocked");
            foreach (var obj in gameObjects)
            {
                obj.tag = blocked ? "Blocked" : "Unblocked";
                if (!blocked)
                    obj.GetComponent<SpriteRenderer>().color = Color.white;

            }
        }
        private void CreatePossiblePlatforms(Player player, Player opponent, Point[][] points)
        {
            List<Point> list = _game.FindPossiblePlatforms(player, opponent, points);
            foreach (var ob in list)
            {
                ob.GameObject.GetComponent<SpriteRenderer>().color = Color.gray;
                ob.GameObject.tag = "Possible";
            }
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

    }
}
