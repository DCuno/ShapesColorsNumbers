using System;
using System.Collections;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialManager : Singleton<TutorialManager>
{
    [SerializeField] private Canvas _titleScreenCanvas = null;
    private OptionsManager _optionsManager = null;
    [SerializeField] private Canvas _optionsMenuCanvas = null;
    //[SerializeField] private GameObject _tutorialMessageParentPrefab = null;
    [SerializeField] private GameObject _raycastBlockerPrefab = null;
    [SerializeField] private GameObject _messageBubblePrefab = null;
    private GameObject _messageBubble = null;
    private GameObject _raycastBlocker = null;
    private MessageBubbleController _messageBubbleController = null;
    private UnityAction PopSound = null;

    private int _runTutorial = 0;
    private const int TUTORIAL_OFF = 0;
    private const int TUTORIAL_ON = 1;
    private int _tutorialStep = 0;

    protected override void Awake()
    {
        base.Awake();
        if (_raycastBlockerPrefab == null)
        {
            Debug.LogError($"Couldn't get {nameof(_raycastBlockerPrefab)}. Tutorial will not function correctly. Please load the prefab into the {nameof(_raycastBlockerPrefab)} field.");
            return;
        }

        if (_messageBubblePrefab == null)
        {
            Debug.LogError($"Couldn't get {nameof(_messageBubblePrefab)}. Tutorial will not function correctly. Please load the prefab into the {nameof(_messageBubblePrefab)} field.");
            return;
        }

        // Get PlayerPrefs tutorial bool to decide to start the tutorial or not
        _runTutorial = PlayerPrefs.GetInt("Tutorial", TUTORIAL_ON);

        if (_runTutorial == TUTORIAL_OFF)
        {
            Destroy(gameObject);
            return;
        }

        PlayerPrefs.SetInt("Tutorial", TUTORIAL_ON);
        PopSound = GameObject.FindGameObjectWithTag("SFXSource").GetComponent<Audio>().PopSound;

        SceneManager.activeSceneChanged += OnOptionsSceneLoaded;
        SceneManager.activeSceneChanged += OnTitleSceneLoaded;
    }

    private void OnDestroy()
    {
        DestroyTutorialObjects();
        SceneManager.activeSceneChanged -= OnOptionsSceneLoaded;
        SceneManager.activeSceneChanged -= OnTitleSceneLoaded;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (_runTutorial == TUTORIAL_OFF || _raycastBlockerPrefab == null || _messageBubblePrefab == null)
        {
            Destroy(gameObject);
            return;
        }

        StartCoroutine(SetupTutorial());
    }

    private IEnumerator SetupTutorial()
    {
        yield return new WaitUntil(TitleSceneLoaded);

        _messageBubbleController.HideBubble();
        _messageBubbleController.MoveBubbleToMiddle();
        _messageBubbleController.ShowBubble();
        _messageBubbleController.SetText("Welcome to Junebug's Shapes Colors Numbers!\n\nPlease press the green arrow to move through this tutorial.");
    }

    private void NextStep()
    {
        _tutorialStep++;
        PopSound?.Invoke();
        Debug.Log($"Tutorial Step: {_tutorialStep}");
        StartCoroutine(TutorialSteps());
    }

    private IEnumerator TutorialSteps()
    {
        yield return null;

        switch( _tutorialStep )
        {
            // Case 0 is the Intro message
            case 1:
                _messageBubbleController.HideBubble();
                _messageBubbleController.MoveBubbleToBottom();
                _messageBubbleController.ShowBubble();
                _messageBubbleController.SetText("This cog button is the settings area where you can customize the learning experience and more!");
                yield break;
            case 2:
                _messageBubbleController.HideBubble();
                TitleScreenManager.Instance.OpenPasswordWindow();
                _messageBubbleController.MoveBubbleToTop();
                _messageBubbleController.ShowBubble();
                _messageBubbleController.SetText("There is parental protection to get to the settings menu.\n\nThe = button will confirm your input.");
                yield break;
            case 3:
                _messageBubbleController.HideBubble();
                DestroyTutorialObjects();
                TitleScreenManager.Instance.OptionsButton();
                yield return new WaitUntil(OptionsSceneLoaded);
                _messageBubbleController.MoveBubbleToBottom();
                _messageBubbleController.ShowBubble();
                _messageBubbleController.SetText("Welcome to the options menu!");
                yield break;
            case 4:
                _messageBubbleController.SetText("You can set the volume for the music and sound effects here.\n\nYou can even mute them completely by un-checking the button.");
                yield break;
            case 5:
                _messageBubbleController.HideBubble();
                _messageBubbleController.MoveBubbleToMiddle();
                _messageBubbleController.ShowBubble();
                _messageBubbleController.SetText("There is a battery saver option that will make the game modes run just a little slower to save your device's battery.");
                yield break;
            case 6:
                _messageBubbleController.HideBubble();
                _optionsManager.MoveScrollRectDown();
                yield return new WaitForSeconds(1.5f);
                _messageBubbleController.MoveBubbleToTop();
                _messageBubbleController.ShowBubble();
                _messageBubbleController.SetText("Here are the settings used in the sandbox game mode.");
                yield break;
            case 7:
                _messageBubbleController.SetText("You can customize the learning experience through selecting what to hear or read when a shape is popped.");
                yield break;
            case 8:
                _messageBubbleController.HideBubble();
                DestroyTutorialObjects();
                _optionsManager.BackButton();
                yield return new WaitUntil(TitleSceneLoaded);
                _messageBubbleController.MoveBubbleToMiddle();
                _messageBubbleController.ShowBubble();
                _messageBubbleController.SetText("Thank you for your patience!\n\nJunebug hopes you enjoy your stay!");
                yield break;
            case 9:
                _runTutorial = TUTORIAL_OFF;
                PlayerPrefs.SetInt("Tutorial", TUTORIAL_OFF);
                _messageBubbleController.HideBubble();
                Destroy(gameObject);
                yield break;
            default:
                yield break;
        }
    }

    private bool TitleSceneLoaded()
    {
        //if (_titleScreenCanvas == null)
        //{
        //    _titleScreenCanvas = FindFirstObjectByType<Canvas>();
        //    return false;
        //}

        if (_raycastBlocker == null)
        {
            _raycastBlocker = Instantiate(_raycastBlockerPrefab, _titleScreenCanvas.transform);
            return false;
        }

        if (_messageBubble == null)
        {
            _messageBubble = Instantiate(_messageBubblePrefab, _titleScreenCanvas.transform);
            return false;
        }

        if (_messageBubbleController == null)
        {
            _messageBubbleController = _messageBubble.GetComponent<MessageBubbleController>();
            _messageBubbleController.OnNextButtonPressed.AddListener(NextStep);
            return false;
        }

        if (_messageBubbleController._TextMeshProUGUI == null)
        {
            _messageBubbleController._TextMeshProUGUI = _messageBubbleController.GetComponentInChildren<TextMeshProUGUI>();
            Debug.Log($"{nameof(_messageBubbleController._TextMeshProUGUI)} is null. Still loading...");
            return false;
        }

        if (_messageBubbleController._RectTransform == null)
        {
            _messageBubbleController._RectTransform = _messageBubbleController.GetComponentInChildren<RectTransform>();
            Debug.Log($"{nameof(_messageBubbleController._RectTransform)} is null. Still loading...");
            return false;
        }

        if (_messageBubbleController._NextButton == null)
        {
            _messageBubbleController._NextButton = _messageBubbleController.GetComponentInChildren<Button>();
            Debug.Log($"{nameof(_messageBubbleController._NextButton)} is null. Still loading...");
            return false;
        }

        Debug.Log("Title Scene loaded. Tutorial is ready.");
        return true;
    }

    private bool OptionsSceneLoaded()
    {
        if (_raycastBlocker == null)
        {
            _raycastBlocker = Instantiate(_raycastBlockerPrefab, _optionsMenuCanvas.transform);
            return false;
        }

        if (_messageBubble == null)
        {
            _messageBubble = Instantiate(_messageBubblePrefab, _optionsMenuCanvas.transform);
            return false;
        }

        if (_messageBubbleController == null)
        {
            _messageBubbleController = _messageBubble.GetComponent<MessageBubbleController>();
            _messageBubbleController.OnNextButtonPressed.AddListener(NextStep);
            return false;
        }

        if (_messageBubbleController._TextMeshProUGUI == null)
        {
            _messageBubbleController._TextMeshProUGUI = _messageBubbleController.GetComponentInChildren<TextMeshProUGUI>();
            Debug.Log($"{nameof(_messageBubbleController._TextMeshProUGUI)} is null. Still loading...");
            return false;
        }

        if (_messageBubbleController._RectTransform == null)
        {
            _messageBubbleController._RectTransform = _messageBubbleController.GetComponentInChildren<RectTransform>();
            Debug.Log($"{nameof(_messageBubbleController._RectTransform)} is null. Still loading...");
            return false;
        }

        if (_messageBubbleController._NextButton == null)
        {
            _messageBubbleController._NextButton = _messageBubbleController.GetComponentInChildren<Button>();
            Debug.Log($"{nameof(_messageBubbleController._NextButton)} is null. Still loading...");
            return false;
        }

        Debug.Log("Options Scene loaded. Tutorial is ready to continue.");
        return true;
    }

    private void DestroyTutorialObjects()
    {
        if (_messageBubble != null)
            Destroy(_messageBubble);

        if (_raycastBlocker != null)
            Destroy(_raycastBlocker);

        _messageBubbleController = null;
    }

    private void OnOptionsSceneLoaded(Scene oldScene, Scene newScene)
    {
        if (newScene.name != "OptionsScene")
            return;

        GameObject optionsGameObject = GameObject.Find("OptionsManager");
        _optionsManager = optionsGameObject.GetComponentInChildren<OptionsManager>();
        _optionsMenuCanvas = FindFirstObjectByType<Canvas>();
    }

    private void OnTitleSceneLoaded(Scene oldScene, Scene newScene)
    {
        if (newScene.name == "TitleScene")
            _titleScreenCanvas = FindFirstObjectByType<Canvas>();
    }
}
