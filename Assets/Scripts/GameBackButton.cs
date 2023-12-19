using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameBackButton : MonoBehaviour
{
    Vector2 _startPos;
    Spawner _spawner;
    // Start is called before the first frame update
    void Start()
    {
        _spawner = GameObject.FindGameObjectWithTag("spawner").GetComponent<Spawner>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        _startPos = Input.mousePosition;
    }

    private void OnMouseDrag()
    {
        if (SceneManager.GetActiveScene().name == "FunModeGameScene2")
        {
            if (Input.mousePosition.normalized.x > gameObject.transform.position.normalized.x * 1 || Input.mousePosition.normalized.y > gameObject.transform.position.normalized.y * 1)
                _spawner.ResetFunMode();

        }
        else if (SceneManager.GetActiveScene().name == "LessonsScene")
        {
            if (Input.mousePosition.normalized.x > gameObject.transform.position.normalized.x * 1 || Input.mousePosition.normalized.y > gameObject.transform.position.normalized.y * 1)
                _spawner.LeaveLessons();
        }
    }
}
