using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameBackButton : MonoBehaviour
{
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
        
    }

    private void OnMouseDown()
    {
        _anim.SetTrigger("Press");
    }

    private void OnMouseDrag()
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
    }
}
