using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour
{
    void OnTriggerStay2D(Collider2D co)
    {
        if (co.name == "Player")
            if (Vector2.Distance(co.transform.position, transform.position) < 0.03)
                Destroy(gameObject);
    }
}
