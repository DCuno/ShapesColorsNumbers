using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapesCollideZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Physics2D.IgnoreLayerCollision(3, 3, false);
    }
}
