using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class TitleScreenManager : MonoBehaviour
{
    [SerializeField] public AudioMixer _mixer;

    private Audio _audio;

    void Start()
    {
        // Setup Volumes from Player Prefs
        _mixer.SetFloat("Music", PlayerPrefs.GetFloat("MusicVol", -6f));
        _mixer.SetFloat("SFX", PlayerPrefs.GetFloat("SFXVol", 0f));

        _audio = GameObject.FindGameObjectWithTag("SFXSource").GetComponent<Audio>();
    }

    public void LessonsButton()
    {
        _audio.PopSound();
        SceneManager.LoadScene(sceneName: "LessonsSelectionScene");
    }

    public void AllButton()
    {
        _audio.PopSound();
        PlayerPrefs.SetString("Chapter", "All");
        SceneManager.LoadScene(sceneName: "LessonsScene");
    }

    public void ShapesButton()
    {
        _audio.PopSound();
        PlayerPrefs.SetString("Chapter", "Shapes");
        SceneManager.LoadScene(sceneName: "LessonsScene");
    }

    public void ColorsButton()
    {
        _audio.PopSound();
        PlayerPrefs.SetString("Chapter", "Colors");
        SceneManager.LoadScene(sceneName: "LessonsScene");
    }

    public void NumbersButton()
    {
        _audio.PopSound();
        PlayerPrefs.SetString("Chapter", "Numbers");
        SceneManager.LoadScene(sceneName: "LessonsScene");
    }

    public void LessonsBackButton()
    {
        _audio.PopSound();
        SceneManager.LoadScene(sceneName: "TitleScene");
    }

    public void FunModeButton()
    {
        _audio.PopSound();
        StartCoroutine(FunModeButtonConfettiWait());
    }

    private IEnumerator FunModeButtonConfettiWait()
    {
        _audio.PopSound();
        var async = SceneManager.LoadSceneAsync(sceneName: "FunModeGameScene2");

        async.allowSceneActivation = false;
        yield return new WaitForSeconds(0.5f);
        async.allowSceneActivation = true;
    }

    public void OptionsButton()
    {
        _audio.PopSound();
        SceneManager.LoadScene(sceneName: "OptionsScene");
    }
}
