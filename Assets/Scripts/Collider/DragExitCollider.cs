using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DragExitCollider : MonoBehaviour
{
    GameBackButton _gameBackButton;
    Spawner _spawner;
    // Start is called before the first frame update
    void Start()
    {
        _gameBackButton = GameObject.FindGameObjectWithTag("GameBackButton").GetComponent<GameBackButton>();
        _spawner = GameObject.FindGameObjectWithTag("spawner").GetComponent<Spawner>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnMouseExit()
    {
        if (_gameBackButton.Touching/* && _gameBackButton.TouchIndex*/)
        {
            if (SceneManager.GetActiveScene().name == "FunModeGameScene2")
            {
                _gameBackButton.ResetButton();
                _spawner.ResetFunMode();

            }
            else if (SceneManager.GetActiveScene().name == "LessonsScene")
            {
                _gameBackButton.ResetButton();
                _spawner.LeaveLessons();
            }
        }
    }
}
