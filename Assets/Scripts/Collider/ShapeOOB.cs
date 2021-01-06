using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeOOB : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Polygon>().solid)
            collision.gameObject.transform.position = new Vector3(0, 0, 0);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
            collision.gameObject.transform.position = new Vector3(0, 0, 0);
    }
}
