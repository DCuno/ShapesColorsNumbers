using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCollider : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D collision)
    {
        Physics2D.IgnoreCollision(collision, GameObject.FindGameObjectWithTag("edge").GetComponent<EdgeCollider2D>(), false);
    }
}
