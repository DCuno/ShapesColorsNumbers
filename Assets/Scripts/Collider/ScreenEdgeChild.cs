using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScreenEdgeChild : MonoBehaviour
{
    private GameObject textGO;
    private GameObject edgeCollider;
    private float distX;
    private float distY;
    private float lerpPercent;
    private float lerpTimer = 0f;
    private float lerpTimeTotal = 2f;
    private bool lerping = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // NOT IMPLEMENTATED COMPLETELY. Should shift text to not be out of bounds.
        // Only for Text objects, that's why the if statement earlier returns if it's a polygon.
        if (collision.gameObject.GetComponent<TextMeshPro>() != null)
        {
            textGO = collision.gameObject;
            edgeCollider = this.transform.parent.gameObject;
            distX = textGO.transform.position.x - edgeCollider.transform.position.x;
            distY = textGO.transform.position.y - edgeCollider.transform.position.y;
            lerping = true;
        }
    }

    private void Update()
    {

        if (lerping)
        {
            lerpTimer += Time.deltaTime;
            if (lerpTimer > lerpTimeTotal)
            {
                lerpTimer = lerpTimeTotal;
            }

            lerpPercent = lerpTimer / lerpTimeTotal;

            if (textGO != null)
                textGO.transform.position = new Vector2(Mathf.Lerp(textGO.transform.position.x, distX - textGO.transform.position.x, lerpPercent),
                                                            Mathf.Lerp(textGO.transform.position.y, distY*1.5f - textGO.transform.position.y, lerpPercent));

            if (lerpTimer == lerpTimeTotal)
            {
                textGO = null;
                lerpTimer = 0f;
                lerpTimeTotal = 2f;
                lerping = false;
            }
        }
    }
}
