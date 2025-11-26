using System;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PasswordWindow : MonoBehaviour
{
    [SerializeField] private Canvas _titleScreenCanvas = null;
    [SerializeField] private GameObject _raycastBlockerPrefab = null;
    private GameObject _raycastBlocker = null;
    public TextMeshProUGUI _QuestionTMP = null;
    private int _leftHandNum = 0;
    private int _rightHandNum = 0;
    private int _curTotal = 0;
    private int _safetyClose = 0;
    private string _question = "";
    private bool _isLeftHandAnswerCleared = true;
    private bool _isRightHandAnswerCleared = true;
    public Button _optionButton = null;
    public Button _CloseWindowButton = null;
    public Button _0Button = null;
    public Button _1Button = null;
    public Button _2Button = null;
    public Button _3Button = null;
    public Button _4Button = null;
    public Button _5Button = null;
    public Button _6Button = null;
    public Button _7Button = null;
    public Button _8Button = null;
    public Button _9Button = null;
    public Button _ClearButton = null;
    public Button _EqualsButton = null;

    private UnityAction PopSound = null;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (_raycastBlockerPrefab == null)
        {
            Debug.LogError($"Couldn't get {nameof(_raycastBlockerPrefab)}. Tutorial will not function correctly. Please load the prefab into the {nameof(_raycastBlockerPrefab)} field.");
            return;
        }

        if (_titleScreenCanvas == null)
            _titleScreenCanvas = FindFirstObjectByType<Canvas>();

        if ( _titleScreenCanvas == null)
        {
            Debug.LogError($"Couldn't get {nameof(_titleScreenCanvas)}. Password Window will not function correctly.");
            return;
        }

        PopSound = GameObject.FindGameObjectWithTag("SFXSource").GetComponent<Audio>().PopSound;

        _optionButton.onClick.AddListener(OptionButtonPressed);
        _CloseWindowButton.onClick.AddListener(CloseButtonPressed);
        _0Button.onClick.AddListener(() => NumberButtonPressed(0));
        _1Button.onClick.AddListener(() => NumberButtonPressed(1));
        _2Button.onClick.AddListener(() => NumberButtonPressed(2));
        _3Button.onClick.AddListener(() => NumberButtonPressed(3));
        _4Button.onClick.AddListener(() => NumberButtonPressed(4));
        _5Button.onClick.AddListener(() => NumberButtonPressed(5));
        _6Button.onClick.AddListener(() => NumberButtonPressed(6));
        _7Button.onClick.AddListener(() => NumberButtonPressed(7));
        _8Button.onClick.AddListener(() => NumberButtonPressed(8));
        _9Button.onClick.AddListener(() => NumberButtonPressed(9));
        _ClearButton.onClick.AddListener(ClearGuessButtonPressed);
        _EqualsButton.onClick.AddListener(CheckAnswer);
        CloseWindow();
    }

    private void OnDestroy()
    {
        _optionButton.onClick.RemoveListener(OptionButtonPressed);
        _CloseWindowButton.onClick.RemoveListener(CloseButtonPressed);
        _0Button.onClick.RemoveListener(() => NumberButtonPressed(0));
        _1Button.onClick.RemoveListener(() => NumberButtonPressed(1));
        _2Button.onClick.RemoveListener(() => NumberButtonPressed(2));
        _3Button.onClick.RemoveListener(() => NumberButtonPressed(3));
        _4Button.onClick.RemoveListener(() => NumberButtonPressed(4));
        _5Button.onClick.RemoveListener(() => NumberButtonPressed(5));
        _6Button.onClick.RemoveListener(() => NumberButtonPressed(6));
        _7Button.onClick.RemoveListener(() => NumberButtonPressed(7));
        _8Button.onClick.RemoveListener(() => NumberButtonPressed(8));
        _9Button.onClick.RemoveListener(() => NumberButtonPressed(9));
        _ClearButton.onClick.RemoveListener(ClearGuessButtonPressed);
        _EqualsButton.onClick.RemoveListener(CheckAnswer);
        CloseWindow();
    }

    private void OptionButtonPressed()
    {
        PopSound?.Invoke();
        OpenWindow();
    }

    private void CloseButtonPressed()
    {
        PopSound?.Invoke();
        CloseWindow();
    }

    public void OpenWindowTutorial()
    {
        GenerateQuestion();
        _question = $"{_leftHandNum} x {_rightHandNum} = ?";
        _QuestionTMP.text = _question;
        gameObject.SetActive(true);
    }

    public void OpenWindow()
    {
        _raycastBlocker = Instantiate(_raycastBlockerPrefab, _titleScreenCanvas.transform);

        if (_raycastBlocker == null)
        {
            Debug.LogError($"Failed to instantiate {nameof(_raycastBlocker)}. Password Window will not function correctly.");
            return;
        }

        gameObject.transform.SetAsLastSibling();

        GenerateQuestion();
        _question = $"{_leftHandNum} x {_rightHandNum} = ?";
        _QuestionTMP.text = _question;
        gameObject.SetActive(true);
    }

    private void CloseWindow()
    {
        ClearPasswordWindow();
        RemoveRaycastBlocker();
        gameObject.SetActive(false);
    }

    private void GenerateQuestion()
    {
        _leftHandNum = Random.Range(2, 10);

        _rightHandNum = _leftHandNum switch
        {
            2 => Random.Range(5, 10),
            3 => Random.Range(4, 10),
            4 => Random.Range(3, 10),
            _ => Random.Range(2, 10),
        };
    }

    private void NumberButtonPressed(int number)
    {
        PopSound?.Invoke();
        if (_isLeftHandAnswerCleared)
        {
            if (number == 0)
                return;

            _isLeftHandAnswerCleared = false;
            _curTotal *= 10;
            _curTotal += number;
            _question = $"{_leftHandNum} x {_rightHandNum} = {_curTotal}";
            _QuestionTMP.text = _question;
        }
        else if (_isRightHandAnswerCleared)
        {
            _isRightHandAnswerCleared = false;
            _curTotal *= 10;
            _curTotal += number;
            _question = $"{_leftHandNum} x {_rightHandNum} = {_curTotal}";
            _QuestionTMP.text = _question;
        }
        else
        {
            _safetyClose++;

            if (_safetyClose == 3)
            {
                CloseWindow();
            }
        }
    }

    private void ClearGuessButtonPressed()
    {
        PopSound?.Invoke();
        _safetyClose = 0;
        _isLeftHandAnswerCleared = true;
        _isRightHandAnswerCleared = true;
        _curTotal = 0;
        _question = $"{_leftHandNum} x {_rightHandNum} = ?";
        _QuestionTMP.text = _question;
    }

    private void ClearPasswordWindow()
    {
        _safetyClose = 0;
        _leftHandNum = 0;
        _rightHandNum = 0;
        _curTotal = 0;
        _isLeftHandAnswerCleared = true;
        _isRightHandAnswerCleared = true;
        _question = $"{_leftHandNum} x {_rightHandNum} = ?";
        _QuestionTMP.text = _question;
    }

    private void RemoveRaycastBlocker()
    {
        if (_raycastBlocker != null)
        {
            Destroy(_raycastBlocker);
            _raycastBlocker = null;
        }
    }

    private void CheckAnswer()
    {
        PopSound?.Invoke();
        if (_curTotal == _leftHandNum * _rightHandNum)
        {
            CloseWindow();
            TitleScreenManager.Instance.OptionsButton();
        }
    }
}
