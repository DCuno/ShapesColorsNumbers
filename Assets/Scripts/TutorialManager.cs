using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : Singleton<TutorialManager>
{
    [SerializeField] private Canvas _titleScreenCanvas = null;
    [SerializeField] private Canvas _optionsMenuCanvas = null;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.activeSceneChanged += OnOptionsSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= OnOptionsSceneLoaded;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get PlayerPrefs tutorial bool
        int runTutorial = PlayerPrefs.GetInt("Tutorial", 1);
        if (runTutorial == 0)
            return;

        // If tutorial, then pull the TitleManager. 
        // Use TitleManager methods to interface with the menus.
        // TODO: Create message objects with Next button to move along tutorial. Mask behind message objects to block interaction 
        // Maybe: Block all interaction and only allow pressing Next button?
        // When Options is entered, pull OptionsManager.
        // Use OptionsManager methods to interface with menus.
        // Maybe: Need to pull scroll rect and impart force like in FunMode to show bottom of screen, or set the value explicitly
    }

    private void OnOptionsSceneLoaded(Scene oldScene, Scene newScene)
    {
        if (newScene.name == "OptionsScene")
            _optionsMenuCanvas = FindFirstObjectByType<Canvas>();
    }
}
