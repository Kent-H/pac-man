using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{

    //distance travelled per game tick
    public float Speed = 0.017f;

    //AI to use, 
    public string AI = "Clyde";

    //direction that this ghost is moving
    Vector2 direction = Vector2.up;

    GameObject player;

    void Start()
    {
        player = GameObject.Find("Player");
    }

    void FixedUpdate()
    {
        if (AI.ToLowerInvariant() == "blinky")
            ChasePlayer();
        else
            RandomizeDirection();

        //if there is open space
        if (isValidMove(direction, Speed))
        {
            //move the player (pac-man) in the specified direction
            Vector2 pos = transform.position;
            GetComponent<Rigidbody2D>().MovePosition(pos + direction * Speed);
        }
    }

    //randomly wander the maze
    void RandomizeDirection()
    {
        //build a list of options, every direction except a 180 turn is included
        var options = new List<Vector2>();
        options.Add(direction);
        options.Add(new Vector2(-direction.y, direction.x));
        options.Add(new Vector2(direction.y, -direction.x));

        //pick randomly from the list until a valid direction is found
        for (int i = (int)(Random.value * options.Count); options.Count > 0; i = (int)(Random.value * options.Count))
        {
            if (isValidMove(options[i], Speed))
            {
                //valid direction found, we will go this way
                direction = options[i];
                return;
            }
            else
            {
                options.RemoveAt(i);
            }
        }

        //if we've reached a dead end, go back the way we came
        direction = -direction;
    }

    //determine the next step based on which direction will move closer to the player
    void ChasePlayer()
    {
        //build a list of options, every direction except a 180 turn is included
        var options = new List<Vector2>();
        options.Add(direction);
        options.Add(new Vector2(-direction.y, direction.x));
        options.Add(new Vector2(direction.y, -direction.x));

        //determine the direction that is most towards the player (original pac-man behavior for blinky)
        Vector2 closestDirection = Vector2.zero;
        float closestDistance = Mathf.Infinity;

        for (int i = 0; i < options.Count; i++)
        {
            float playerDistance = Vector2.Distance((Vector2)transform.position + options[i] * Speed, player.transform.position);
            if (playerDistance < closestDistance && isValidMove(options[i], Speed))
            {
                //valid direction found, we will go this way
                closestDirection = options[i];
                closestDistance = playerDistance;
            }
        }

        if (closestDirection != Vector2.zero)
            direction = closestDirection;
        else
            //if we've reached a dead end, go back the way we came
            direction = -direction;
    }

    //helper to check if moving would cause a collision
    bool isValidMove(Vector2 dir, float dist)
    {
        //parameters are pulled from the Box2DCollier object, so modifying the size of the player shouldn't break this
        return Physics2D.BoxCast(transform.position, GetComponent<BoxCollider2D>().size, 0, dir, dist, LayerMask.GetMask("solids")).collider == null;
    }
}
