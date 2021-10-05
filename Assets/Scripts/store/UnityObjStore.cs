using System.Collections.Generic;
using models;
using UnityEngine;

namespace store
{
    public class UnityObjStore : MonoBehaviour
    {
        const int FieldSize = 17; 
        public GameObject[][] PointGameObjects { get; } = new GameObject[17][];
        public Dictionary<string, GameObject> PlayerObjects { get; } = new Dictionary<string, GameObject>(2);

        private Game _game;

        public void Awake()
        {
            _game = FindObjectOfType<Game>();
            for (var i = 0; i < PointGameObjects.Length; i++)
            {
                PointGameObjects[i] = new GameObject[17];
            }
        }

        public void AddPlayer(string name, GameObject gameObject)
        {
            PlayerObjects[name] = gameObject;
        }

        public void AddPoint(int x, int y, GameObject gameObject)
        {
            PointGameObjects[y][x] = gameObject;
        }

        public GameObject[] FindGameObjectsWithTag(string tagName)
        {
            var res = new List<GameObject>(FieldSize * 2);
            for (var y = 0; y < FieldSize; y++)
            {
                for (var x = 0; x < FieldSize; x++)
                {
                    if (_game.Points[y][x]?.Tag == tagName && PointGameObjects[y][x] != null)
                    {
                        res.Add(PointGameObjects[y][x]);
                    }
                }
            }
            return res.ToArray();
        }
    }
}
