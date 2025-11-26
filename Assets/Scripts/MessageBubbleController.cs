using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MessageBubbleController : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI _TextMeshProUGUI = null;
    [SerializeField] public RectTransform _RectTransform = null;
    [SerializeField] public Button _NextButton = null;
    private const float MARGIN = 250f;

    public UnityEvent OnNextButtonPressed;

    private void Awake()
    {
        if (_RectTransform == null)
            _RectTransform = GetComponent<RectTransform>();

        if (_RectTransform == null)
        {
            Debug.LogError($"Couldn't get {nameof(_RectTransform)}. Tutorial will not function correctly.");
            return;
        }

        if (_TextMeshProUGUI == null)
            _TextMeshProUGUI = GetComponentInChildren<TextMeshProUGUI>();

        if (_TextMeshProUGUI == null)
        {
            Debug.LogError($"Couldn't get {nameof(_TextMeshProUGUI)}. Tutorial will not function correctly.");
            return;
        }

        if (_NextButton == null)
            _NextButton = GetComponentInChildren<Button>();

        if (_NextButton == null)
        {
            Debug.LogError($"Couldn't get {nameof(_NextButton)}. Tutorial will not function correctly.");
            return;
        }

        _NextButton.onClick.AddListener(NextButtonPressed);
    }

    private void NextButtonPressed()
    {
        OnNextButtonPressed.Invoke();
    }

    public void SetText(string text)
    {
        _TextMeshProUGUI.text = text;
    }

    public void MoveBubbleToTop()
    {
        _RectTransform.anchorMin = new Vector2(0.5f, 1f);
        _RectTransform.anchorMax = new Vector2(0.5f, 1f);
        _RectTransform.pivot = new Vector2(0.5f, 1f);
        _RectTransform.anchoredPosition = new Vector2(0, -MARGIN);
    }

    public void MoveBubbleToMiddle()
    {
        _RectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        _RectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        _RectTransform.pivot = new Vector2(0.5f, 0.5f);
        _RectTransform.anchoredPosition = Vector2.zero;
    }

    public void MoveBubbleToBottom()
    {
        _RectTransform.anchorMin = new Vector2(0.5f, 0f);
        _RectTransform.anchorMax = new Vector2(0.5f, 0f);
        _RectTransform.pivot = new Vector2(0.5f, 0f);
        _RectTransform.anchoredPosition = new Vector2(0, MARGIN);
    }

    public void HideBubble()
    {
        gameObject.SetActive(false);
    }

    public void ShowBubble()
    {
        gameObject.SetActive(true);
    }

    public void MoveBubble(float x, float y)
    {
        gameObject.transform.position = new Vector3(x, y, 0f);
    }
}
