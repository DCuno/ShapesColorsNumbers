using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShapeOOB : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Polygon polygon = collision.gameObject.GetComponent<Polygon>();
        Rigidbody2D rigidbody2D = collision.gameObject.GetComponent<Polygon>().GetComponent<Rigidbody2D>();

        if (polygon != null && polygon.solid && polygon.edgesOn)
        {
            polygon.TeleportSound();
            rigidbody2D.position = Vector2.zero;
            return;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Polygon polygon = collision.gameObject.GetComponent<Polygon>();
        Rigidbody2D rigidbody2D = collision.gameObject.GetComponent<Polygon>().GetComponent<Rigidbody2D>();

        if (polygon != null && polygon.edgesOn)
        {
            if (!polygon.popped)
                polygon.TeleportSound();

            rigidbody2D.position = Vector2.zero;
            return;
        }
        else if (polygon != null && !polygon.edgesOn)
        {
            Vector3 curPos = collision.gameObject.transform.position;

            if (curPos.x > 5.0f || curPos.x < -5.0f)
            {
                rigidbody2D.position = new Vector2(-curPos.x, curPos.y);
            }

            if (curPos.y > 10.0f || curPos.y < -10.0f)
            {
                rigidbody2D.position = new Vector2(curPos.x, -curPos.y);
            }
            return;
        }
    }
}
