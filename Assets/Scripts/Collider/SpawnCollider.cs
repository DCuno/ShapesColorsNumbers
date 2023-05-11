using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCollider : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D collision)
    {
        Polygon polygon = collision.gameObject.GetComponent<Polygon>();
        if (polygon != null && polygon.EdgesOn)
        {
            GameObject[] edges = GameObject.FindGameObjectsWithTag("edge");
            foreach (GameObject edge in edges)
            {
                Physics2D.IgnoreCollision(collision, edge.GetComponent<BoxCollider2D>(), false);
            }
            collision.gameObject.GetComponent<Polygon>().IsSolid = true;
        }
        else if (polygon != null && !polygon.EdgesOn)
        {
            collision.gameObject.GetComponent<Polygon>().IsSolid = true;
        }
    }
}
