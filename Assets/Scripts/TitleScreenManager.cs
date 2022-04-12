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
