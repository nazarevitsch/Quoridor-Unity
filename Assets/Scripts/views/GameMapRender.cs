using System.Collections.Generic;
using Controllers;
using models;
using store;
using UnityEngine;
using UnityEngine.UI;

namespace views
{
    public class GameMapRender : MonoBehaviour
    {
        private Game _game;

        private UnityObjStore _objStore;
        
        private void Awake()
        {
            _objStore = FindObjectOfType<UnityObjStore>();
            _game = FindObjectOfType<Game>();
            _game.OnRenderWalls += ChangeWalls;
            _game.OnPointTagChanged += TagChanged;
            _game.OnPlayersChanged += ChangeButtons;
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
            Point[] gameObjects = _game.FindPointsWithTag("PossibleToBlock");
            foreach (var point in gameObjects)
            {
                point.Tag = "Unblocked";
                _objStore.PointGameObjects[point.Y][point.X].GetComponent<SpriteRenderer>().color = Color.white;
            }
            gameObjects = _game.FindPointsWithTag("HalfBlocked");
            foreach (var point in gameObjects)
            {
                _game.Points[point.Y][point.X].Tag = blocked ? "Blocked" : "Unblocked";
                if (!blocked)
                    _objStore.PointGameObjects[point.Y][point.X].GetComponent<SpriteRenderer>().color = Color.white;

            }
        }
        private void CreatePossiblePlatforms(Player player, Player opponent, Point[][] points)
        {
            List<Point> list = _game.FindPossiblePlatforms(player, opponent, points);
            foreach (var point in list)
            {
                _objStore.PointGameObjects[point.Y][point.X].GetComponent<SpriteRenderer>().color = Color.gray;
                point.Tag = "Possible";
            }
        }
        private void DestroyPossiblePlatforms()
        {
            Point[] gameObjects = _game.FindPointsWithTag("Possible");
            foreach (var point in gameObjects)
            {
                point.Tag = "Platform";
                _objStore.PointGameObjects[point.Y][point.X].GetComponent<SpriteRenderer>().color = Color.black;
            }
        }

        private void TagChanged(string tagName, int x, int y)
        {
            switch (tagName)
            {
                case "PossibleToBlock":
                    _objStore
                        .PointGameObjects[y][x]
                        .GetComponent<SpriteRenderer>()
                        .color = Color.yellow;
                    break;
                case "Blocked":
                case "HalfBlocked":
                    _objStore
                        .PointGameObjects[y][x]
                        .GetComponent<SpriteRenderer>()
                        .color = Color.magenta;
                    break;
            }
            
        }
        
        private void ChangeButtons(Player currentPlayer, Player enemyPlayer)
        {
            currentPlayer.BlocksCountField.GetComponent<Text>().text = currentPlayer.Name + ": " + currentPlayer.BlocksCount;
            enemyPlayer.BlocksCountField.GetComponent<Text>().text = enemyPlayer.Name + ": " + enemyPlayer.BlocksCount;
            enemyPlayer.BlocksUseButton.GetComponent<ButtonHandler>().ChangeText();
            if (currentPlayer.BlocksCount > 0)
            {
                currentPlayer.BlocksUseButton.SetActive(true);
            }
            enemyPlayer.BlocksUseButton.SetActive(false);
        }

    }
}
