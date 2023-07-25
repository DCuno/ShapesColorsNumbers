using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class TitleScreenManager : MonoBehaviour
{
    [SerializeField] public AudioMixer _mixer;

    void Start()
    {
        // Setup Volumes from Player Prefs
        _mixer.SetFloat("Music", PlayerPrefs.GetFloat("MusicVol", -6f));
        _mixer.SetFloat("SFX", PlayerPrefs.GetFloat("SFXVol", 0f));
    }

    public void LessonsButton()
    {
        SceneManager.LoadScene(sceneName: "LessonsSelectionScene");
    }

    public void AllButton()
    {
        PlayerPrefs.SetString("Chapter", "All");
        SceneManager.LoadScene(sceneName: "LessonsScene");
    }

    public void ShapesButton()
    {
        PlayerPrefs.SetString("Chapter", "Shapes");
        SceneManager.LoadScene(sceneName: "LessonsScene");
    }

    public void ColorsButton()
    {
        PlayerPrefs.SetString("Chapter", "Colors");
        SceneManager.LoadScene(sceneName: "LessonsScene");
    }

    public void NumbersButton()
    {
        PlayerPrefs.SetString("Chapter", "Numbers");
        SceneManager.LoadScene(sceneName: "LessonsScene");
    }

    public void LessonsBackButton()
    {
        SceneManager.LoadScene(sceneName: "TitleScene");
    }

    public void FunModeButton()
    {
        StartCoroutine(FunModeButtonConfettiWait());
    }

    private IEnumerator FunModeButtonConfettiWait()
    {
        var async = SceneManager.LoadSceneAsync(sceneName: "FunModeGameScene2");

        async.allowSceneActivation = false;
        yield return new WaitForSeconds(0.5f);
        async.allowSceneActivation = true;
    }

    public void OptionsButton()
    {
        SceneManager.LoadScene(sceneName: "OptionsScene");
    }
}
