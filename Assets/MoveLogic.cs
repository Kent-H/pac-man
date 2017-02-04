using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveLogic : MonoBehaviour
{
    //remember all the arrow keys that are pressed
    //this will always be ordered from the most- to least-recently pressed
    List<KeyCode> keyDownOrder = new List<KeyCode>();

    Vector2 direction = Vector2.zero;
    float speed = 0.0125f;

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

        RecalculateDirection();
    }


    //Recalculate the direction of movement based on depressed key's depression order
    void RecalculateDirection()
    {
        var key = GetMostPreferredKey();

        //convert the key to a direction
        switch (key)
        {
            case KeyCode.UpArrow:
                direction = Vector2.up;
                break;
            case KeyCode.DownArrow:
                direction = Vector2.down;
                break;
            case KeyCode.LeftArrow:
                direction = Vector2.left;
                break;
            case KeyCode.RightArrow:
                direction = Vector2.right;
                break;
            case KeyCode.None:
                //if no key is pressed, simply continue on the current trajectory.
                break;
            default:
                Debug.LogError("Unknown keyCode:");
                Debug.LogError(key);
                break;
        }
    }

    KeyCode GetMostPreferredKey()
    {
        //only allow a key when the opposite key isn't pressed at the same time (up & down) or (left & right)
        foreach (var key in keyDownOrder)
        {
            //up & down
            if (key == KeyCode.UpArrow || key == KeyCode.DownArrow)
            {
                //one of the two must not be pressed
                if (!keyDownOrder.Contains(KeyCode.UpArrow) || !keyDownOrder.Contains(KeyCode.DownArrow))
                    return key;
            }
            //left & right
            else if (key == KeyCode.LeftArrow || key == KeyCode.RightArrow)
            {
                if (!keyDownOrder.Contains(KeyCode.LeftArrow) || !keyDownOrder.Contains(KeyCode.RightArrow))
                    return key;
            }
        }
        return KeyCode.None;
    }


    //To ensure consistent movement, we're using the fixedUpdate method
    void FixedUpdate()
    {
        //move the player (pac-man) in the specified direction
        GetComponent<Rigidbody2D>().MovePosition(GetComponent<Rigidbody2D>().position + direction * speed);
    }


    void Update()
    {
        //always face the direction that we're moving
        GetComponent<Rigidbody2D>().MoveRotation(Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
    }

}
