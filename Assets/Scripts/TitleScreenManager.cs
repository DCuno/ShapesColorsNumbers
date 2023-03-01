using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenManager : MonoBehaviour
{
    public void LessonsButton()
    {
        SceneManager.LoadScene(sceneName: "LessonsScene");
    }

    public void FunModeButton()
    {
        SceneManager.LoadScene(sceneName: "FunModeGameScene2");
    }

    public void OptionsButton()
    {
        SceneManager.LoadScene(sceneName: "OptionsScene");
    }
}
