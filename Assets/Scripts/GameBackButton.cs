using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameBackButton : MonoBehaviour
{
    public bool Touching = false;
    Vector2 _startPos;
    Spawner _spawner;
    Animator _anim;
    // Start is called before the first frame update
    void Start()
    {
        _spawner = GameObject.FindGameObjectWithTag("spawner").GetComponent<Spawner>();
        _anim = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //CheckTouch();
    }

    private void CheckTouch()
    {
        if (Input.touchCount > 0)
        {
            // Loop through all active touches
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);

                if (touch.phase == TouchPhase.Began)
                {
                    // Convert touch position to world position
                    Vector3 worldTouchPosition = Camera.main.ScreenToWorldPoint(touch.position);

                    // Perform a raycast to detect if the object is touched
                    RaycastHit2D hit = Physics2D.Raycast(worldTouchPosition, Vector2.zero);

                    // Check if the object was hit by the raycast
                    if (hit.collider != null && hit.collider.gameObject == gameObject)
                    {
                        //Pop();
                    }
                }
            }
        }
    }

    private void OnMouseDown()
    {
        SetButton();
    }

    private void OnMouseUp()
    {
        ResetButton();
    }

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
