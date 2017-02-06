using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveLogic : MonoBehaviour
{
    //distance travelled per game tick
    public float Speed = 0.017f;

    //whether or not the player will continue moving when the key is released
    public bool CONTINTUE_MOVING_ON_KEYUP = true;

    //remember all the arrow keys that are pressed
    //this is kept ordered from the most- to least-recently pressed
    List<KeyCode> keyDownOrder = new List<KeyCode>();

    //where to return the player when they die
    Vector2 spawnPoint;

    //direction that the player is moving
    Vector2 direction = Vector2.zero;

    //is the player still alive?
    bool dead = false;

    void Start()
    {
        spawnPoint = new Vector2(0, -0.17f);
    }

    //process all key events
    void OnGUI()
    {
        //ignore anything that's not a keyboard key
        KeyCode key = Event.current.keyCode;
        if (!Event.current.isKey ||
            (key != KeyCode.UpArrow &&
            key != KeyCode.DownArrow &&
            key != KeyCode.LeftArrow &&
            key != KeyCode.RightArrow))
        {
            return;
        }

        //if the key was pressed
        if (Event.current.type == EventType.keyDown)
        {
            keyDownOrder.Remove(key);
            keyDownOrder.Insert(0, key);
        }
        //if the key was released
        else if (Event.current.type == EventType.keyUp)
        {
            keyDownOrder.Remove(key);
        }
    }

    void OnTriggerEnter2D(Collider2D co)
    {
        //when colliding with a ghost
        if (co.gameObject.tag == "ghost")
        {
            //start the death animation
            GetComponent<Animator>().SetBool("dead", true);

            //stop moving
            dead = true;
            direction = Vector2.zero;
            GetComponent<Rigidbody2D>().MoveRotation(0);
        }
    }

    void OnDeathComplete()
    {
        //move to starting point
        GetComponent<Rigidbody2D>().MovePosition(spawnPoint);

        //reset the rotation
        direction = GetMostPreferredDirection();
        GetComponent<Rigidbody2D>().MoveRotation(Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
    }

    void OnRespawn()
    {
        //stop the death animation
        GetComponent<Animator>().SetBool("dead", false);
        GetComponent<Animator>().SetBool("moving", false);

        //resume player control
        dead = false;
    }

    //To ensure consistent movement, we're using the fixedUpdate method
    void FixedUpdate()
    {
        if (!dead)
        {
            direction = GetMostPreferredDirection();

            if (direction != Vector2.zero && isValidMove(direction, Speed))
            {
                //move the player (pac-man) in the specified direction
                Vector2 pos = GetComponent<Rigidbody2D>().position;
                GetComponent<Rigidbody2D>().MovePosition(pos + direction * Speed);
                GetComponent<Rigidbody2D>().MoveRotation(Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);

                GetComponent<Animator>().SetBool("moving", true);
            }
            else
            {
                GetComponent<Animator>().SetBool("moving", false);
            }
        }
    }

    //Recalculate the direction of movement based on depressed key's depression order, map space, and 
    Vector3 GetMostPreferredDirection()
    {
        //try each depressed key to look for valid moves.
        foreach (var key in keyDownOrder)
        {
            var dir = GetDirectionFor(key);
            //ensure that the player can actually move in this direction
            if (isValidMove(dir, Speed))
            {
                //only allow a key when the opposite key isn't pressed at the same time (up & down) or (left & right)
                //up & down
                if (key == KeyCode.UpArrow || key == KeyCode.DownArrow)
                {
                    //one of the two must not be pressed
                    if (!keyDownOrder.Contains(KeyCode.UpArrow) || !keyDownOrder.Contains(KeyCode.DownArrow))
                        return dir;
                }
                //left & right
                else if (key == KeyCode.LeftArrow || key == KeyCode.RightArrow)
                {
                    if (!keyDownOrder.Contains(KeyCode.LeftArrow) || !keyDownOrder.Contains(KeyCode.RightArrow))
                        return dir;
                }
            }
        }
        //if no key is pressed, simply continue on the current trajectory.
        return CONTINTUE_MOVING_ON_KEYUP ? direction : Vector2.zero;
    }


    Vector2 GetDirectionFor(KeyCode key)
    {
        //convert the key to a direction
        switch (key)
        {
            case KeyCode.UpArrow:
                return Vector2.up;
            case KeyCode.DownArrow:
                return Vector2.down;
            case KeyCode.LeftArrow:
                return Vector2.left;
            case KeyCode.RightArrow:
                return Vector2.right;
            case KeyCode.None:
                //if no key is pressed, simply continue on the current trajectory.
                return direction;
            default:
                Debug.LogError("Unknown keyCode:");
                Debug.LogError(key);
                return direction;
        }
    }

    //helper to check if moving would cause a collision
    bool isValidMove(Vector2 dir, float dist)
    {
        //parameters are pulled from the Box2DCollier object, so modifying the size of the player shouldn't break this
        return Physics2D.BoxCast(transform.position, GetComponent<BoxCollider2D>().size, 0, dir, dist, LayerMask.GetMask("solids")).collider == null;
    }

}
