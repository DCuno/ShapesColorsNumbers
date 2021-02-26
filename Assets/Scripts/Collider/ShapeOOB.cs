using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShapeOOB : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Polygon polygon = collision.gameObject.GetComponent<Polygon>();
        if (polygon != null && polygon.solid && polygon.edgesOn)
        {
            polygon.TeleportSound();
            collision.gameObject.transform.position = new Vector3(0, 0, 0);
            return;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Polygon polygon = collision.gameObject.GetComponent<Polygon>();

        if (polygon != null && polygon.edgesOn)
        {
            if (!polygon.popped)
                polygon.TeleportSound();

            collision.gameObject.transform.position = new Vector3(0, 0, 0);
            return;
        }
        else if (polygon != null && !polygon.edgesOn)
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
            return;
        }
    }
}
