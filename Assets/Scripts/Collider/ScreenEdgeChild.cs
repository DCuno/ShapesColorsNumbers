using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScreenEdgeChild : MonoBehaviour
{
    public TextLerper.LerpDirection Dir;
    private TextLerper _textLerper;
    private GameObject _textGO;
    private GameObject edgeCollider;
    private GameObject _spawner;
    private float distX;
    private float distY;
    private float lerpPercent;
    private float lerpTimer = 0f;
    private float lerpTimeTotal = 3f;
    private bool lerping = false;
    private Vector2 _screenBottomLeft;
    private Vector2 _screenTopRight;
    private Camera _camera;

    private void Start()
    {
        _camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        _screenBottomLeft = _camera.ScreenToWorldPoint(new Vector3(0, 0, _camera.nearClipPlane));
        _screenTopRight = _camera.ScreenToWorldPoint(new Vector3(_camera.pixelWidth, _camera.pixelHeight, _camera.nearClipPlane));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<TextMeshPro>() != null)
        {
            _textGO = collision.gameObject;
            _textLerper = _textGO.GetComponent<TextLerper>();
            edgeCollider = this.transform.parent.gameObject;
            distX = _textLerper.transform.position.x - edgeCollider.transform.position.x;
            distY = _textLerper.transform.position.y - edgeCollider.transform.position.y;
            _textLerper.Triggered(distX, distY, _screenBottomLeft, _screenTopRight, Dir);
        }
    }

    /*    private void Update()
        {

            if (lerping)
            {
                lerpTimer += Time.deltaTime;
                if (lerpTimer > lerpTimeTotal)
                {
                    lerpTimer = lerpTimeTotal;
                }

                lerpPercent = lerpTimer / lerpTimeTotal;

                print(lerpTimer + " " + lerpPercent);

                if (textGO != null && lerpTimer < lerpTimeTotal)
                {
                    textGO.transform.position = new Vector2(Mathf.Lerp(textGO.transform.position.x, distX - textGO.transform.position.x, lerpPercent),
                                                                Mathf.Lerp(textGO.transform.position.y, distY - textGO.transform.position.y, lerpPercent));
                }
                else if (textGO != null && lerpTimer >= lerpTimeTotal)
                {
                    textGO.transform.position = new Vector2(distX, distY);
                }

                if (lerpTimer == lerpTimeTotal)
                {
                    textGO = null;
                    lerpTimer = 0f;
                    lerping = false;
                }
            }
        }*/
}
