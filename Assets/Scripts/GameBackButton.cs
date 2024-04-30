using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameBackButton : MonoBehaviour
{
    public bool Touching = false;
    private Vector2 _startPos;
    private Spawner _spawner;
    private Animator _anim;
    public int TouchIndex;
    private Vector2 _screenBottomLeft;
    private Vector2 _screenTopRight;
    private Camera _camera;

    private void OnEnable()
    {
        ResetButton();
    }

    // Start is called before the first frame update
    void Start()
    {
        _camera = Camera.main;
        _screenBottomLeft = _camera.ScreenToWorldPoint(new Vector3(0, 0, _camera.nearClipPlane));
        _screenTopRight = _camera.ScreenToWorldPoint(new Vector3(_camera.pixelWidth, _camera.pixelHeight, _camera.nearClipPlane));

        _spawner = GameObject.FindGameObjectWithTag("spawner").GetComponent<Spawner>();
        _anim = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckTouch();
    }

    private void CheckTouch()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                // Convert touch position to world position
                Vector3 worldTouchPosition = Camera.main.ScreenToWorldPoint(touch.position);

                RaycastHit2D[] raycastArray = Physics2D.RaycastAll(worldTouchPosition, Vector2.zero);
                RaycastHit2D hit = new RaycastHit2D();

                // Perform a raycast to detect if the object is touched
                foreach (RaycastHit2D rayHit in raycastArray)
                {
                    if (rayHit.transform.CompareTag("GameBackButton"))
                        hit = rayHit;
                }

                // Check if the object was hit by the raycast
                if (hit.collider != null && hit.collider.gameObject == gameObject)
                {
                    TouchIndex = touch.fingerId;
                    SetButton();
                }
            }

            if (Touching && touch.phase == TouchPhase.Moved && touch.fingerId == TouchIndex)
            {
                Vector2 touchPosition = _camera.ScreenToWorldPoint(touch.position);
                if ((touchPosition.x > 0.0f) || (touchPosition.y < _screenTopRight.y/2))
                {
                    if (SceneManager.GetActiveScene().name == "FunModeGameScene2")
                    {
                        ResetButton();
                        _spawner.ResetFunMode();
                    }
                    else if (SceneManager.GetActiveScene().name == "LessonsScene")
                    {
                        ResetButton();
                        _spawner.LeaveLessons();
                    }
                }
            }


            if (touch.phase == TouchPhase.Ended && touch.fingerId == TouchIndex)
            {
                ResetButton();
            }
        }

        if (Input.touchCount > 0)
        {
            // Loop through all active touches
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);
/*
                if (touch.phase == TouchPhase.Began)
                {
                    // Convert touch position to world position
                    Vector3 worldTouchPosition = Camera.main.ScreenToWorldPoint(touch.position);

                    RaycastHit2D[] raycastArray = Physics2D.RaycastAll(worldTouchPosition, Vector2.zero);
                    RaycastHit2D hit = new RaycastHit2D();

                    // Perform a raycast to detect if the object is touched
                    foreach (RaycastHit2D rayHit in raycastArray)
                    {
                        if (rayHit.transform.CompareTag("GameBackButton"))
                            hit = rayHit;
                    }

                    // Check if the object was hit by the raycast
                    if (hit.collider != null && hit.collider.gameObject == gameObject)
                    {
                        TouchIndex = touch.fingerId;
                        SetButton();
                    }
                }*/

                if (touch.phase == TouchPhase.Ended && touch.fingerId == TouchIndex)
                {
                    ResetButton();
                }
            }
        }
    }

    /*private void OnMouseDown()
    {
        SetButton();
    }

    private void OnMouseUp()
    {
        ResetButton();
    }*/

    public void SetButton()
    {
        _anim.SetBool("Press 0", true);
        Touching = true;
    }

    public void ResetButton()
    {
        _anim.SetBool("Press 0", false);
        Touching = false;
    }

    /*private void OnMouseDrag()
    {
        //print("mouse: " + Camera.main.ScreenToWorldPoint(Input.mousePosition) + " transform: " + gameObject.transform.position/2f);
        if (SceneManager.GetActiveScene().name == "FunModeGameScene2")
        {
            if (Camera.main.ScreenToWorldPoint(Input.mousePosition).x > gameObject.transform.position.x / 8f || Camera.main.ScreenToWorldPoint(Input.mousePosition).y < gameObject.transform.position.y / 2f)
                _spawner.ResetFunMode();

        }
        else if (SceneManager.GetActiveScene().name == "LessonsScene")
        {
            if (Camera.main.ScreenToWorldPoint(Input.mousePosition).x > gameObject.transform.position.x / 8f || Camera.main.ScreenToWorldPoint(Input.mousePosition).y < gameObject.transform.position.y / 2f)
                _spawner.LeaveLessons();
        }
    }*/
}
