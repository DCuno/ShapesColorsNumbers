using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PasswordWindow : MonoBehaviour
{
    public TitleScreenManager _TitleScreenManager;
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


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _optionButton.onClick.AddListener(OpenWindow);
        _CloseWindowButton.onClick.AddListener(CloseWindow);
        _0Button.onClick.AddListener(() => NumberButtonPress(0));
        _1Button.onClick.AddListener(() => NumberButtonPress(1));
        _2Button.onClick.AddListener(() => NumberButtonPress(2));
        _3Button.onClick.AddListener(() => NumberButtonPress(3));
        _4Button.onClick.AddListener(() => NumberButtonPress(4));
        _5Button.onClick.AddListener(() => NumberButtonPress(5));
        _6Button.onClick.AddListener(() => NumberButtonPress(6));
        _7Button.onClick.AddListener(() => NumberButtonPress(7));
        _8Button.onClick.AddListener(() => NumberButtonPress(8));
        _9Button.onClick.AddListener(() => NumberButtonPress(9));
        _ClearButton.onClick.AddListener(ClearGuess);
        _EqualsButton.onClick.AddListener(CheckAnswer);
        CloseWindow();
    }

    private void OnDestroy()
    {
        _optionButton.onClick.RemoveListener(OpenWindow);
        _CloseWindowButton.onClick.RemoveListener(CloseWindow);
        _0Button.onClick.RemoveListener(() => NumberButtonPress(0));
        _1Button.onClick.RemoveListener(() => NumberButtonPress(1));
        _2Button.onClick.RemoveListener(() => NumberButtonPress(2));
        _3Button.onClick.RemoveListener(() => NumberButtonPress(3));
        _4Button.onClick.RemoveListener(() => NumberButtonPress(4));
        _5Button.onClick.RemoveListener(() => NumberButtonPress(5));
        _6Button.onClick.RemoveListener(() => NumberButtonPress(6));
        _7Button.onClick.RemoveListener(() => NumberButtonPress(7));
        _8Button.onClick.RemoveListener(() => NumberButtonPress(8));
        _9Button.onClick.RemoveListener(() => NumberButtonPress(9));
        _ClearButton.onClick.RemoveListener(ClearGuess);
        _EqualsButton.onClick.RemoveListener(CheckAnswer);
        CloseWindow();
    }

    private void OpenWindow()
    {
        GenerateQuestion();
        _question = $"{_leftHandNum} x {_rightHandNum} = ?";
        _QuestionTMP.text = _question;
        gameObject.SetActive(true);
    }

    private void CloseWindow()
    {
        ClearPasswordWindow();
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

    private void NumberButtonPress(int number)
    {
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
                ClearPasswordWindow();
                gameObject.SetActive(false);
            }
        }
    }

    private void ClearGuess()
    {
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

    private void CheckAnswer()
    {
        if (_curTotal == _leftHandNum * _rightHandNum)
        {
            ClearPasswordWindow();
            gameObject.SetActive(false);
            _TitleScreenManager.OptionsButton();
        }
    }
}
