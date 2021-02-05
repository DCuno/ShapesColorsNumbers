using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeOOB : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Polygon polygon = collision.gameObject.GetComponent<Polygon>();
        if (polygon.solid && polygon.edgesOn)
        {
            polygon.TeleportSound();
            collision.gameObject.transform.position = new Vector3(0, 0, 0);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Polygon polygon = collision.gameObject.GetComponent<Polygon>();
        if (polygon.edgesOn)
        {
            if (!polygon.popped)
                polygon.TeleportSound();

            collision.gameObject.transform.position = new Vector3(0, 0, 0);
        }

        if (!collision.gameObject.GetComponent<Polygon>().edgesOn)
        {
            Vector3 curPos = collision.gameObject.transform.position;

            if (curPos.x > 5.0f || curPos.x < -5.0f)
            {
                collision.gameObject.transform.position = new Vector3(-curPos.x, curPos.y, curPos.z);
            }

            if (curPos.y > 10.0f || curPos.y < -10.0f)
            {
                collision.gameObject.transform.position = new Vector3(curPos.x, -curPos.y, curPos.z);
            }
        }
    }
}
