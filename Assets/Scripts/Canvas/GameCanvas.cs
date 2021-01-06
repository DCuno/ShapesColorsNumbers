using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameCanvas : MonoBehaviour
{
    private GameObject _quitButton;

    private Spawner spawner;

    private void Awake()
    {
        _quitButton = GameObject.FindGameObjectWithTag("quitButton");
        spawner = GameObject.FindGameObjectWithTag("spawner").GetComponent<Spawner>();
        _quitButton.SetActive(false);
    }

    public void QuitButton()
    {
        SceneManager.LoadScene(sceneName: "TitleScene");
    }

    private void Update()
    {
        if (spawner.finished)
            _quitButton.SetActive(true);
    }
}
