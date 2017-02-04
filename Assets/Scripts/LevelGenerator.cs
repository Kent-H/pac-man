using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{

    const int SIZE = 10;

    // Use this for initialization
    void Start()
    {

        var dot = Resources.Load("Prefabs/Dot");
        var wall = Resources.Load("Prefabs/Wall");

        bool[,] complete = new bool[10, 10];

        //generate the middle
        for (int x = -2; x <= 1; x++)
        {
            for (int y = -2; y <= 1; y++)
            {
                Instantiate(dot, new Vector2(x * 0.17f, y * 0.17f), Quaternion.identity);
            }
        }

        //Instantiate(dot, new Vector2(x * 0.15f, y * 0.15f), Quaternion.identity);

    }

}
