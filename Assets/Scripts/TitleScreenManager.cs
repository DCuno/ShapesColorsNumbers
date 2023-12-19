using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScreenManager : MonoBehaviour
{
    [SerializeField] private AudioMixer _mixer;
    [SerializeField] private OptionsManager _optionsManager;
    private Audio _sfx;

    public GameObject[] UIButtonImages;

    void Start()
    {
        //if (_optionsManager == null && _optionsManager.name == "DontDestroyOnLoad")
            //DontDestroyOnLoad(_optionsManager);
        // Setup Volumes from Player Prefs
        _mixer.SetFloat("Music", OptionsManager.SliderToDecibelMusic(PlayerPrefs.GetFloat("Music", 5f)));
        _mixer.SetFloat("SFX", OptionsManager.SliderToDecibelSFX(PlayerPrefs.GetFloat("SFX", 5f)));
        _sfx = GameObject.FindGameObjectWithTag("SFXSource").GetComponent<Audio>();
        _mixer.GetFloat("Music", out float value);
        _mixer.GetFloat("SFX", out float value2);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public void LessonsButton()
    {
        _sfx.PopSound();
        SceneManager.LoadScene(sceneName: "LessonsSelectionScene");
    }

    public void AllButton()
    {
        _sfx.PopSound();
        PlayerPrefs.SetString("Chapter", "All");
        SceneManager.LoadScene(sceneName: "LessonsScene");
    }

    public void ShapesButton()
    {
        _sfx.PopSound();
        PlayerPrefs.SetString("Chapter", "Shapes");
        SceneManager.LoadScene(sceneName: "LessonsScene");
    }

    public void ColorsButton()
    {
        _sfx.PopSound();
        PlayerPrefs.SetString("Chapter", "Colors");
        SceneManager.LoadScene(sceneName: "LessonsScene");
    }

    public void NumbersButton()
    {
        _sfx.PopSound();
        PlayerPrefs.SetString("Chapter", "Numbers");
        SceneManager.LoadScene(sceneName: "LessonsScene");
    }

    public void LessonsBackButton()
    {
        _sfx.PopSound();
        SceneManager.LoadScene(sceneName: "TitleScene");
    }

    public void FunModeButton()
    {
        _sfx.PopSound();
        //StartCoroutine(FunModeButtonConfettiWait());
        SceneManager.LoadScene(sceneName: "FunModeGameScene2");
    }

    private IEnumerator FunModeButtonConfettiWait()
    {
        _sfx.PopSound();
        var async = SceneManager.LoadSceneAsync(sceneName: "FunModeGameScene2");

        async.allowSceneActivation = false;
        yield return new WaitForSeconds(0.5f);
        async.allowSceneActivation = true;
    }

    public void OptionsButton()
    {
        _sfx.PopSound();
        SceneManager.LoadScene(sceneName: "OptionsScene");
    }
}
