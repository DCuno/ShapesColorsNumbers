using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapesOOBEdgesOff : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D collision)
    {
        GameObject obj = collision.gameObject;
        Polygon polygon = obj.GetComponent<Polygon>();
        if (polygon != null && polygon.solid && !polygon.edgesOn)
        {
            float randX = Random.Range(-8.0f, 8.0f);
            float randY = Random.Range(-15.0f, 15.0f);
            polygon.TeleportSound();
            obj.transform.position = new Vector3(randX, randY, 0);
            // random velocity and direction?
            // particle effect?
        }
    }
}
