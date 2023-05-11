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
        /*Polygon polygon = collision.gameObject.GetComponent<Polygon>();
        Rigidbody2D rigidbody2D = collision.gameObject.GetComponent<Polygon>().GetComponent<Rigidbody2D>();

        if (polygon != null && polygon.solid && polygon.edgesOn)
        {
            if (!polygon.popped)
                polygon.TeleportSound();

            collision.gameObject.transform.position = new Vector3(0, 0, 0);
            return;
        }
        else if (polygon != null && polygon.solid && !polygon.edgesOn)
        {
            Vector3 curPos = collision.gameObject.transform.position;

            if (curPos.x > (Screen.width / Camera.main.orthographicSize) / 4 || curPos.x < -(Screen.width / Camera.main.orthographicSize) / 4)
            {
                collision.gameObject.transform.position = new Vector3(-curPos.x, curPos.y, curPos.z);
            }

            if (curPos.y > (Screen.height / Camera.main.orthographicSize) / 4 || curPos.y < -(Screen.height / Camera.main.orthographicSize) / 4)
            {
                collision.gameObject.transform.position = new Vector3(curPos.x, -curPos.y, curPos.z);
            }
            return;
        }*/
    }
}
