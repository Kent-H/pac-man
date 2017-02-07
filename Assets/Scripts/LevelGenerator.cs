using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    //size of the game grid
    public int Size = 11;

    public float TileSize = 0.17f;

    // Use this for initialization
    void Start()
    {
        //store grid space that has already been used
        bool[,] complete = new bool[11, 11];

        //horizontal and vertical walls
        bool[,] vWalls = new bool[Size - 1, Size];
        bool[,] hWalls = new bool[Size, Size - 1];

        //block off the middle where ghosts spawn(2x1 box)
        vWalls[Size / 2 + 1, Size / 2] = true;
        vWalls[Size / 2 - 1, Size / 2] = true;
        hWalls[Size / 2, Size / 2 - 1] = true;
        hWalls[Size / 2 + 1, Size / 2] = true;
        hWalls[Size / 2 + 1, Size / 2 - 1] = true;

        //plus one hallway around it. (4x3 box)
        for (int x = 0; x < 4; x++)
        {
            //mark the entire box as used space
            for (int y = 0; y < 3; y++)
                complete[Size / 2 + x - 1, Size / 2 + y - 1] = true;

            //top and bottom border
            hWalls[Size / 2 + x - 1, Size / 2 + 1] = true;
            hWalls[Size / 2 + x - 1, Size / 2 - 2] = true;
        }

        for (int y = 0; y < 3; y++)
        {
            //left and right border
            vWalls[Size / 2 - 2, Size / 2 + y - 1] = true;
            vWalls[Size / 2 + 2, Size / 2 + y - 1] = true;
        }
        
        //randomly generate 12 "hallways"
        for (int i = 0; i < 12; i++)
            RandomHallway(complete, vWalls, hWalls, 0.4f);

        //now that we've devided where everything will go, place the game elements
        Generate(complete, vWalls, hWalls);
    }


    //places entities into the game
    void Generate(bool[,] complete, bool[,] vertical, bool[,] horizontal)
    {
        var dot = Resources.Load("Prefabs/Dot");
        var wall = Resources.Load("Prefabs/Wall");

        //walls around the map
        for (int i = 0; i < Size; i++)
        {
            Instantiate(wall, new Vector2((-Size / 2f + i + 0.5f) * TileSize, (Size / 2f) * TileSize), Quaternion.Euler(0, 0, 90));
            Instantiate(wall, new Vector2((-Size / 2f + i + 0.5f) * TileSize, (-Size / 2f) * TileSize), Quaternion.Euler(0, 0, 90));
            Instantiate(wall, new Vector2((Size / 2f) * TileSize, (-Size / 2f + i + 0.5f) * TileSize), Quaternion.identity);
            Instantiate(wall, new Vector2((-Size / 2f) * TileSize, (-Size / 2f + i + 0.5f) * TileSize), Quaternion.identity);
        }

        //dot at the center of every grid tile
        for (int x = 0; x < Size; x++)
            for (int y = 0; y < Size; y++)
                //only generate walls inside the grid
                if (complete[x, y])
                    Instantiate(dot, new Vector2((-Size / 2f + x + 0.5f) * TileSize, (-Size / 2f + y + 0.5f) * TileSize), Quaternion.identity);

        //all vertical walls, or dots where there are no walls
        for (int x = 0; x < vertical.GetLength(0); x++)
            for (int y = 0; y < vertical.GetLength(1); y++)
            {
                if (vertical[x, y])
                {
                    Instantiate(wall, new Vector2((-Size / 2f + x + 1) * TileSize, (-Size / 2f + y + 0.5f) * TileSize), Quaternion.identity);
                }
                else
                {
                    if (complete[x, y] || complete[x + 1, y])
                        Instantiate(dot, new Vector2((-Size / 2f + x + 1) * TileSize, (-Size / 2f + y + 0.5f) * TileSize), Quaternion.identity);
                }
            }

        //all horizontal walls, dots where there are no walls
        for (int x = 0; x < horizontal.GetLength(0); x++)
            for (int y = 0; y < horizontal.GetLength(1); y++)
            {
                if (horizontal[x, y])
                {
                    Instantiate(wall, new Vector2((-Size / 2f + x + 0.5f) * TileSize, (-Size / 2f + y + 1) * TileSize), Quaternion.Euler(0, 0, 90));
                }
                else
                {
                    if (complete[x, y] || complete[x, y + 1])
                        Instantiate(dot, new Vector2((-Size / 2f + x + +0.5f) * TileSize, (-Size / 2f + y + 1) * TileSize), Quaternion.identity);
                }
            }
    }

    void RandomHallway(bool[,] complete, bool[,] vWalls, bool[,] hWalls, float endChance)
    {
        Vector2 position, direction;
        FindRandomStart(complete, out position, out direction);

        Debug.DrawLine(Vector3.zero, direction, Color.green, 1000);

        int hallwayLength = 0;
        while (true)
        {
            hallwayLength++;
            //mark this tile complete
            complete[(int)position.x, (int)position.y] = true;

            //create a wall on every side of this tile
            //except the direction we came from, remove that wall instead
            //up
            if (IsValidHWall(position.x, position.y))
                hWalls[(int)position.x, (int)position.y] = direction != Vector2.down;
            //down
            if (IsValidHWall(position.x, position.y - 1))
                hWalls[(int)position.x, (int)position.y - 1] = direction != Vector2.up;
            //left
            if (IsValidVWall(position.x - 1, position.y))
                vWalls[(int)position.x - 1, (int)position.y] = direction != Vector2.right;
            //right
            if (IsValidVWall(position.x, position.y))
                vWalls[(int)position.x, (int)position.y] = direction != Vector2.left;


            Vector2 left = new Vector2(-direction.y, direction.x);
            Vector2 right = new Vector2(direction.y, -direction.x);

            //should we end the hallway?
            if (hallwayLength > 3 && Random.value < endChance &&
                IsInBounds(position.x + left.x, position.y + left.y) &&
                complete[(int)(position.x + left.x), (int)(position.y + left.y)])
            {
                EndHallway(vWalls, hWalls, position, left);
                return;
            }

            //determine which direction to go next

            //always try to go left first
            if (IsInBounds(position.x + left.x, position.y + left.y) &&
                !complete[(int)(position.x + left.x), (int)(position.y + left.y)])
            {
                direction = left;
            }
            //then try straight.  There's a special out-of-bounds case here (to avoid a circle-the-map situation)
            else if (!IsInBounds(position.x + direction.x, position.y + direction.y))
            {
                //if we can, cleanly end the hallway
                if (hallwayLength > 3 &&
                    IsInBounds(position.x + left.x, position.y + left.y) &&
                    complete[(int)(position.x + left.x), (int)(position.y + left.y)])
                    EndHallway(vWalls, hWalls, position, left);

                //if straight is out of bounds, don't continue
                return;
            }
            //second part of move-straight check
            else if (!complete[(int)(position.x + direction.x), (int)(position.y + direction.y)])
            {
                //nothing to do
            }
            //finally try right
            else if (IsInBounds(position.x + right.x, position.y + right.y) &&
                !complete[(int)(position.x + right.x), (int)(position.y + right.y)])
            {
                direction = right;
            }
            //if we're trapped in a corner
            else
            {
                //try to cleanly end the hallway
                if (hallwayLength > 3 &&
                    IsInBounds(position.x + left.x, position.y + left.y) &&
                    complete[(int)(position.x + left.x), (int)(position.y + left.y)])
                    EndHallway(vWalls, hWalls, position, left);

                //don't continue
                return;
            }

            position += direction;
        }

    }

    //to end a hallway, attach it to the left
    void EndHallway(bool[,] vWalls, bool[,] hWalls, Vector2 position, Vector2 left)
    {
        if (left == Vector2.up)
            hWalls[(int)position.x, (int)position.y] = false;
        else if (left == Vector2.down)
            hWalls[(int)position.x, (int)position.y - 1] = false;
        else if (left == Vector2.left)
            vWalls[(int)position.x - 1, (int)position.y] = false;
        else if (left == Vector2.right)
            vWalls[(int)position.x, (int)position.y] = false;
    }


    void FindRandomStart(bool[,] complete, out Vector2 position, out Vector2 direction)
    {
        //start in the center and pick a random direction
        float dir = Random.value * 2 * Mathf.PI;
        float xAmount = Mathf.Cos(dir);
        float yAmount = Mathf.Sin(dir);
        Debug.DrawLine(Vector3.zero, new Vector3(xAmount, yAmount, 0), Color.green, 1000);
        direction = Vector2.zero;
        int x = 0, y = 0;

        //march until an unused space is found
        while (!IsInBounds(x + Size / 2, y + Size / 2) || complete[x + Size / 2, y + Size / 2])
        {
            if ((Mathf.Abs(x) + 1) / Mathf.Abs(xAmount) < (Mathf.Abs(y) + 1) / Mathf.Abs(yAmount))
            {
                direction = xAmount > 0 ? Vector2.right : Vector2.left;
                x += xAmount > 0 ? 1 : -1;
            }
            else
            {
                direction = yAmount > 0 ? Vector2.up : Vector2.down;
                y += yAmount > 0 ? 1 : -1;
            }

            //if we leave the grid without finding an open space
            if (x + Size / 2 < 0 || y + Size / 2 < 0 || x + Size / 2 > Size || y + Size / 2 > Size)
            {
                //restart
                dir = Random.value * 2 * Mathf.PI;
                xAmount = Mathf.Cos(dir);
                yAmount = Mathf.Sin(dir);
                x = y = 0;
            }
        }

        position = new Vector2(x + Size / 2, y + Size / 2);
    }

    bool IsInBounds(float x, float y)
    {
        return x >= 0 && y >= 0 && x < Size && y < Size;
    }

    bool IsValidVWall(float x, float y)
    {
        //vertical walls have size-1 on the x-axis
        return x >= 0 && y >= 0 && x < Size - 1 && y < Size;
    }

    bool IsValidHWall(float x, float y)
    {
        //horizontal walls have size-1 on the y-axis
        return x >= 0 && y >= 0 && x < Size && y < Size - 1;
    }

}
