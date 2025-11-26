using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScreenManager : Singleton<TitleScreenManager>
{
    [SerializeField] private AudioMixer _mixer;
    [SerializeField] private PasswordWindow _passwordWindow;
    private Audio _sfx;

    public GameObject[] UIButtonImages;

    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 30;
        Physics2D.simulationMode = SimulationMode2D.Script;

        //if (_optionsManager == null && _optionsManager.name == "DontDestroyOnLoad")
        //DontDestroyOnLoad(_optionsManager);
        // Setup Volumes from Player Prefs
        _mixer.SetFloat("Music", OptionsManager.SliderToDecibelMusic(PlayerPrefs.GetFloat("Music", OptionsManager.MUSIC_SLIDER_DEFAULT)));
        _mixer.SetFloat("SFX", OptionsManager.SliderToDecibelSFX(PlayerPrefs.GetFloat("SFX", OptionsManager.SFX_SLIDER_DEFAULT)));

        if (PlayerPrefs.GetInt("MusicMute", 1) == 0)
            _mixer.SetFloat("Music", OptionsManager.MUTED_VOL);
        else
            _mixer.SetFloat("Music", OptionsManager.SliderToDecibelMusic(PlayerPrefs.GetFloat("Music", OptionsManager.MUSIC_SLIDER_DEFAULT)));

        if (PlayerPrefs.GetInt("SFXMute", 1) == 0)
            _mixer.SetFloat("SFX", OptionsManager.MUTED_VOL);
        else
            _mixer.SetFloat("SFX", OptionsManager.SliderToDecibelSFX(PlayerPrefs.GetFloat("SFX", OptionsManager.SFX_SLIDER_DEFAULT)));

        _sfx = GameObject.FindGameObjectWithTag("SFXSource").GetComponent<Audio>();

        PlayerPrefs.SetInt("Listen", PlayerPrefs.GetInt("Listen", 1));

        PlayerPrefs.SetInt("Read", PlayerPrefs.GetInt("Read", 1));

        PlayerPrefs.SetInt("LearningTopic", PlayerPrefs.GetInt("LearningTopic", 0));

        if (_passwordWindow == null)
            _passwordWindow = GameObject.FindGameObjectWithTag("PasswordWindow").GetComponentInChildren<PasswordWindow>();

        if (_passwordWindow == null)
        {
            Debug.LogError($"Couldn't get {nameof(_passwordWindow)}. Game will not function correctly.");
            return;
        }

        SceneManager.activeSceneChanged += OnLessonSelectionSceneLoaded;
        SceneManager.activeSceneChanged += OnTitleSceneLoaded;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public void OpenPasswordWindow()
    {
        _passwordWindow.OpenWindowTutorial();
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
        SceneManager.LoadScene(sceneName: "FunModeGameScene2");
    }

    public void OptionsButton()
    {
        SceneManager.LoadScene(sceneName: "OptionsScene");
    }

    private void OnLessonSelectionSceneLoaded(Scene oldScene, Scene newScene)
    {
        if (newScene.name != "LessonsSelectionScene")
            return;

        GameObject.Find("All (Button)").GetComponentInChildren<Button>().onClick.AddListener(AllButton);
        GameObject.Find("Shapes (Button)").GetComponentInChildren<Button>().onClick.AddListener(ShapesButton);
        GameObject.Find("Colors (Button)").GetComponentInChildren<Button>().onClick.AddListener(ColorsButton);
        GameObject.Find("Numbers (Button)").GetComponentInChildren<Button>().onClick.AddListener(NumbersButton);
        GameObject.Find("Back (Button)").GetComponentInChildren<Button>().onClick.AddListener(LessonsBackButton);
    }

    private void OnTitleSceneLoaded(Scene oldScene, Scene newScene)
    {
        if (newScene.name != "TitleScene")
            return;

        GameObject.Find("Lessons (Button)").GetComponentInChildren<Button>().onClick.AddListener(LessonsButton);
        GameObject.Find("Sandbox (Button)").GetComponentInChildren<Button>().onClick.AddListener(FunModeButton);
        _passwordWindow = GameObject.FindGameObjectWithTag("PasswordWindow").GetComponentInChildren<PasswordWindow>();
    }
}
