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

        for (int i = -SIZE/2; i <= SIZE/2; i++)
        {
            Instantiate(wall, new Vector2(i * 0.17f, SIZE * 0.17f / 2), Quaternion.Euler(0, 0, 90));
            Instantiate(wall, new Vector2(i * 0.17f, -SIZE * 0.17f / 2), Quaternion.Euler(0, 0, 270));
            Instantiate(wall, new Vector2(SIZE * 0.17f / 2, i * 0.17f), Quaternion.identity);
            Instantiate(wall, new Vector2(-SIZE * 0.17f / 2, i * 0.17f), Quaternion.Euler(0, 0, 180));
        }

        //Instantiate(dot, new Vector2(x * 0.15f, y * 0.15f), Quaternion.identity);

    }

}
