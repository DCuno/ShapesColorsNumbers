using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextLerper : MonoBehaviour
{
    public bool Lerping = false;
    private float _distX;
    private float _distY;
    float _lerpTimeTotal = 700f;
    float _lerpPercent;
    private Vector2 _screenBottomLeft;
    private Vector2 _screenTopRight;
    private Vector2[] _boundsVec;
    Vector2 _bottomLeft;
    Vector2 _topRight;
    Vector2 _startPosition;
    Vector2 _endPosition;
    float _height;
    float _width;
    private LerpDirection _dir;
    private MeshRenderer _meshRenderer;
    private bool _triggered = false;

    public enum LerpDirection { Top, Right, Bottom, Left };

    public void Triggered(float distX, float distY, Vector2 screenBottomLeft, Vector2 screenBottomRight, LerpDirection dir)
    {
        if (!_triggered)
        {
            _meshRenderer = gameObject.GetComponent<MeshRenderer>();
            Lerping = true;
            _distX = distX;
            _distY = distY;
            _screenBottomLeft = screenBottomLeft;
            _screenTopRight = screenBottomRight;
            _dir = dir;
            _height = _meshRenderer.bounds.size.y;
            _width = _meshRenderer.bounds.size.x / 3;
            _startPosition = gameObject.transform.position;

            switch (_dir)
            {
                case LerpDirection.Right:
                    _endPosition = new Vector2(_startPosition.x - _width, _startPosition.y);
                    break;
                case LerpDirection.Left:
                    _endPosition = new Vector2(_startPosition.x + _width, _startPosition.y);
                    break;
                case LerpDirection.Top:
                    _endPosition = new Vector2(_startPosition.x, _startPosition.y - _height);
                    break;
                case LerpDirection.Bottom:
                    _endPosition = new Vector2(_startPosition.x, _startPosition.y + _height);
                    break;
            }

            _triggered = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Lerping)
        {
            StartCoroutine(Lerp());
        }
    }
    IEnumerator Lerp()
    {
        float timeElapsed = 0;
        while (timeElapsed < _lerpTimeTotal)
        {
            _lerpPercent = timeElapsed / _lerpTimeTotal;

            gameObject.transform.position = new Vector2(Mathf.Lerp(gameObject.transform.position.x, _endPosition.x, _lerpPercent),
                                                        Mathf.Lerp(gameObject.transform.position.y, _endPosition.y, _lerpPercent));

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        gameObject.transform.position = _endPosition;
    }
}
