using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityObjStore : MonoBehaviour
{

    public GameObject[][] PointGameObjects { get; } = new GameObject[17][];
    public Dictionary<string, GameObject> PlayerObjects { get; } = new Dictionary<string, GameObject>(2);

    public void Awake()
    {
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
}
