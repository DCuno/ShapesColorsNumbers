using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;
using Toggle = UnityEngine.UI.Toggle;

public class OptionsManager : MonoBehaviour
{
    [SerializeField] private AudioMixer _mixer;
    [SerializeField] private Slider _soundEffectsSlider;
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private TextMeshProUGUI _musicSliderCounter;
    [SerializeField] private TextMeshProUGUI _SFXSliderCounter;
    [SerializeField] private ScrollRect _settingsScrollRect;
    [SerializeField] private UnityEngine.UI.Button _backButton;
    public GameObject ScrollArrowUp;
    public GameObject ScrollArrowDown;

    private const float SCROLL_FORCE = 5000f;
    public const float SFX_SLIDER_DEFAULT = 5f;
    public const float MUSIC_SLIDER_DEFAULT = 5f;
    public const float MUTED_VOL = -80f;

    public float Multiplier = 30f;
    [Range(0, 1)] public float DefaultSliderPercentage = 0.31f;
    float _curMusicSliderVal;
    float _curSFXSliderVal;
    Toggle _musicToggle;
    Toggle _sfxToggle;
    Toggle _batterySaverToggle;
    Toggle[] _topicsToggles;
    ToggleGroup _topicsGroup;
    Toggle _listenToggle;
    Toggle _readToggle;
    Spawner.Topics _curTopic;
    bool _curListen;
    bool _curRead;
    bool _curSFXMute;
    bool _curMusicMute;
    bool _curBatterySaverToggleOn;

    private UnityAction PopSound = null;

    void Awake()
    {
        if (_musicSlider == null)
        {
            _musicSlider = GameObject.FindGameObjectWithTag("MusicSlider").GetComponent<Slider>();
        }

        if (_soundEffectsSlider == null)
        {
            _soundEffectsSlider = GameObject.FindGameObjectWithTag("SoundEffectsSlider").GetComponent<Slider>();
        }

        if (_musicSliderCounter == null)
        {
            _musicSliderCounter = GameObject.FindGameObjectWithTag("MusicSliderCounter").GetComponent<TextMeshProUGUI>();
        }

        if (_SFXSliderCounter == null)
        {
            _SFXSliderCounter = GameObject.FindGameObjectWithTag("SoundEffectsSliderCounter").GetComponent<TextMeshProUGUI>();
        }

        if (_settingsScrollRect == null)
        {
            _settingsScrollRect = GameObject.FindGameObjectWithTag("ScrollRect").GetComponent<ScrollRect>();
        }

        if (_backButton == null)
        {
            _backButton = GameObject.Find("Back (Button)").GetComponent<UnityEngine.UI.Button>();
        }

        if (_mixer == null)
        {
            _mixer = GameObject.FindGameObjectWithTag("MusicSource").GetComponent<AudioSource>().outputAudioMixerGroup.audioMixer;
        }

        if  (_musicToggle == null)
        {
            _musicToggle = GameObject.FindGameObjectWithTag("MusicMute").GetComponent<Toggle>();
        }

        if (_sfxToggle == null)
        {
            _sfxToggle = GameObject.FindGameObjectWithTag("SoundEffectsMute").GetComponent<Toggle>();
        }

        if (_batterySaverToggle == null)
        {
            _batterySaverToggle = GameObject.FindGameObjectWithTag("BatterySaverToggleOn").GetComponent<Toggle>();
        }

        if (ScrollArrowUp == null)
        {
            ScrollArrowUp = GameObject.FindGameObjectWithTag("ScrollArrowUp");
        }

        if (ScrollArrowDown == null)
        {
            ScrollArrowDown = GameObject.FindGameObjectWithTag("ScrollArrowDown");
        }

        // Shapes, Colors, Numbers topic toggle
        if (_topicsToggles == null)
        {
            _topicsToggles = GameObject.FindGameObjectWithTag("TopicsPanelGroup").GetComponentsInChildren<Toggle>();
            _topicsGroup = GameObject.FindGameObjectWithTag("TopicsPanelGroup").GetComponentInChildren<ToggleGroup>();
        }

        // Listen toggle
        if (_listenToggle == null)
        {
            _listenToggle = GameObject.FindGameObjectWithTag("VoicePanelGroup").GetComponentInChildren<Toggle>();
        }

        // Read toggle
        if (_readToggle == null)
        {
            _readToggle = GameObject.FindGameObjectWithTag("TextPanelGroup").GetComponentInChildren<Toggle>();
        }

        LoadFromPrefs();

        // Add listeners after preferences are loaded, so no events are triggered
        foreach (Toggle toggle in _topicsToggles)
        {
            toggle.onValueChanged.AddListener((isOn) => OnToggleGroupChanged(toggle, isOn));
        }

        PopSound = GameObject.FindGameObjectWithTag("SFXSource").GetComponent<Audio>().PopSound;
        _backButton.onClick.AddListener(BackButton);
        ScrollArrowUp.SetActive(false);
        _settingsScrollRect.onValueChanged.AddListener(ScrollArrow);
    }

    // Start is called before the first frame update
    void Start()
    {
        /*_soundEffectsSlider = GameObject.FindGameObjectWithTag("SoundEffectsSlider").GetComponent<Slider>();
        _musicSlider = GameObject.FindGameObjectWithTag("MusicSlider").GetComponent<Slider>();


        _musicSlider.value = PlayerPrefs.GetFloat("Music", _musicSlider.value);
        _soundEffectsSlider.value = PlayerPrefs.GetFloat("SFX", _soundEffectsSlider.value);
        curMusicSliderVal = _musicSlider.value;
        curSFXSliderVal = _soundEffectsSlider.value;*/
        //
    }

    // Update is called once per frame
    void Update()
    {
        if (_curMusicSliderVal != _musicSlider.value || _curMusicMute == _musicToggle.isOn)
        {
            PopSound?.Invoke();
            _curMusicMute = !_musicToggle.isOn;
            _curMusicSliderVal = _musicSlider.value;
            PlayerPrefs.SetFloat("Music", _musicSlider.value);
            PlayerPrefs.SetInt("MusicMute", _musicToggle.isOn == true ? 1 : 0);

            if (_curMusicMute)
                _mixer.SetFloat("Music", OptionsManager.MUTED_VOL);
            else
                _mixer.SetFloat("Music", SliderToDecibelMusic(PlayerPrefs.GetFloat("Music", OptionsManager.MUSIC_SLIDER_DEFAULT)));
        }

        if (_curSFXSliderVal != _soundEffectsSlider.value || _curSFXMute == _sfxToggle.isOn)
        {
            PopSound?.Invoke();
            _curSFXMute = !_sfxToggle.isOn;
            _curSFXSliderVal = _soundEffectsSlider.value;
            PlayerPrefs.SetFloat("SFX", _soundEffectsSlider.value);
            PlayerPrefs.SetInt("SFXMute", _sfxToggle.isOn == true ? 1 : 0);

            if (_curSFXMute)
                _mixer.SetFloat("SFX", OptionsManager.MUTED_VOL);
            else
                _mixer.SetFloat("SFX", SliderToDecibelSFX(PlayerPrefs.GetFloat("SFX", OptionsManager.SFX_SLIDER_DEFAULT)));
        }

        if (_curBatterySaverToggleOn != _batterySaverToggle.isOn)
        {
            PopSound?.Invoke();
            PlayerPrefs.SetInt("BatterySaver", _batterySaverToggle.isOn ? 1 : 0);
            _curBatterySaverToggleOn = _batterySaverToggle.isOn;
        }

        if (_curListen != _listenToggle.isOn)
        {
            PopSound?.Invoke();
            _curListen = _listenToggle.isOn;
            PlayerPrefs.SetInt("Listen", _listenToggle.isOn == true ? 1 : 0);
        }

        if (_curRead != _readToggle.isOn)
        {
            PopSound?.Invoke();
            _curRead = _readToggle.isOn;
            PlayerPrefs.SetInt("Read", _readToggle.isOn == true ? 1 : 0);
        }
    }

    private void OnToggleGroupChanged(Toggle changedToggle, bool isOn)
    {
        if (isOn)
        {
            PopSound?.Invoke();
            System.Enum.TryParse(changedToggle.name, out Spawner.Topics result);
            PlayerPrefs.SetInt("LearningTopic", (int)result);
        }
    }

    private void LoadFromPrefs()
    {
        _musicSlider.value = PlayerPrefs.GetFloat("Music", OptionsManager.MUSIC_SLIDER_DEFAULT);
        _soundEffectsSlider.value = PlayerPrefs.GetFloat("SFX", OptionsManager.SFX_SLIDER_DEFAULT);
        _musicToggle.isOn = PlayerPrefs.GetInt("MusicMute", 1) == 1;
        _sfxToggle.isOn = PlayerPrefs.GetInt("SFXMute", 1) == 1;
        _batterySaverToggle.isOn = PlayerPrefs.GetInt("BatterySaver", 0) == 1 ? true : false;
        _listenToggle.isOn = PlayerPrefs.GetInt("Listen", 1) == 1;
        _readToggle.isOn = PlayerPrefs.GetInt("Read", 1) == 1;
        int playerPrefsTopic = PlayerPrefs.GetInt("LearningTopic", 0);
        foreach (Toggle toggle in _topicsToggles)
        {
            System.Enum.TryParse(toggle.name, out Spawner.Topics result);
            if (playerPrefsTopic.Equals((int)result))
            {
                toggle.isOn = true;
                _curTopic = result;
            }
            else
            {
                toggle.isOn = false;
            }
        }

        _curMusicMute = !_musicToggle.isOn;
        _curSFXMute = !_sfxToggle.isOn;
        _curBatterySaverToggleOn = _batterySaverToggle.isOn;
        _curMusicSliderVal = _musicSlider.value;
        _curSFXSliderVal = _soundEffectsSlider.value;
        _curListen = _listenToggle.isOn;
        _curRead = _readToggle.isOn;

        if (_curMusicMute)
            _mixer.SetFloat("Music", OptionsManager.MUTED_VOL);
        else
            _mixer.SetFloat("Music", SliderToDecibelMusic(PlayerPrefs.GetFloat("Music", OptionsManager.MUSIC_SLIDER_DEFAULT)));

        if (_curSFXMute)
            _mixer.SetFloat("SFX", OptionsManager.MUTED_VOL);
        else
            _mixer.SetFloat("SFX", SliderToDecibelSFX(PlayerPrefs.GetFloat("SFX", OptionsManager.SFX_SLIDER_DEFAULT)));
    }

    private void SaveToPrefs()
    {
        if (_musicSlider)
            PlayerPrefs.SetFloat("Music", _musicSlider.value);
        else
            Debug.LogError("No music slider to get value from.");

        if (_soundEffectsSlider)
            PlayerPrefs.SetFloat("SFX", _soundEffectsSlider.value);
        else
            Debug.LogError("No sound effect slider to get value from.");
    }

    public void BackButton()
    {
        //SaveToPrefs();
        if (PlayerPrefs.GetInt("Tutorial", 0) == 0)
            PopSound?.Invoke();

        SceneManager.LoadScene(sceneName: "TitleScene");
        //StartCoroutine(BackCoR());
    }

    public void ScrollArrow(Vector2 position)
    {
        if (position.y >= 0.7f && ScrollArrowUp.activeSelf)
        {
            ScrollArrowUp.SetActive(false);
            ScrollArrowDown.SetActive(true);
        }

        if (position.y < 0.3f && !ScrollArrowUp.activeSelf)
        {
            ScrollArrowUp.SetActive(true);
            ScrollArrowDown.SetActive(false);
        }
    }

    public IEnumerator BackCoR()
    {
        SaveToPrefs();
        yield return new WaitForSeconds(1f);
        var async = SceneManager.LoadSceneAsync(sceneName: "TitleScene");

        async.allowSceneActivation = false;
        yield return new WaitForSeconds(0.5f);
        async.allowSceneActivation = true;
    }

    public void MoveScrollRectDown()
    {
        _settingsScrollRect.velocity = new Vector2(_settingsScrollRect.velocity.x, SCROLL_FORCE);
    }

    public void MoveScrollRectUp()
    {
        _settingsScrollRect.velocity = new Vector2(_settingsScrollRect.velocity.x, -SCROLL_FORCE);
    }

    public void SoundEffectsSlider()
    {
        //Slider _soundEffectsSlider = GameObject.FindGameObjectWithTag("SoundEffectsSlider").GetComponent<Slider>();
        _mixer.SetFloat("SFX", SliderToDecibelSFX(PlayerPrefs.GetFloat("SFX", OptionsManager.SFX_SLIDER_DEFAULT)));
        //PlayerPrefs.SetFloat("SFX", _soundEffectsSlider.value);
        //_SFXSliderCounter.text = _soundEffectsSlider.value.ToString();
        GameObject.FindGameObjectWithTag("SoundEffectsSliderCounter").GetComponent<TextMeshProUGUI>().text = _soundEffectsSlider.value.ToString();
    }

    public void MusicSlider()
    {
        //_musicSliderCounter.text = _musicSlider.value.ToString();
        _mixer.SetFloat("Music", SliderToDecibelMusic(PlayerPrefs.GetFloat("Music", OptionsManager.MUSIC_SLIDER_DEFAULT)));
        GameObject.FindGameObjectWithTag("MusicSliderCounter").GetComponent<TextMeshProUGUI>().text = _musicSlider.value.ToString();
    }

    /*public IEnumerator SetMixerFloat(string name, float val)
    {
        bool result = false;

        while (!result)
        {
            result = _mixer.SetFloat(name, val);
            PlayerPrefs.SetFloat(name, val);

            _mixer.GetFloat(name, out float value);

            yield return null;

            if (result && value != val)
                result = false;
        }

        yield break;
    }*/

    public void SetMixerFloat(string name, float val)
    {
        bool result = false;

        while (!result)
        {
            result = _mixer.SetFloat(name, val);
            PlayerPrefs.SetFloat(name, val);

            _mixer.GetFloat(name, out float value);

            if (result && value != val)
                result = false;
        }
    }

    public static float SliderToDecibelMusic(float value)
    {
        switch (value)
        {
            case 0:
                return MUTED_VOL;
            case 1:
                return -23f;
            case 2:
                return -22f;
            case 3:
                return -21f;
            case 4:
                return -20f;
            case 5:
                return -19f;
            case 6:
                return -18f;
            case 7:
                return -17f;
            case 8:
                return -16f;
            case 9:
                return -15f;
            case 10:
                return -14f;
            default:
                return -19f;
        }
    }

    public static float SliderToDecibelSFX(float value)
    {
        switch (value)
        {
            case 0:
                return MUTED_VOL;
            case 1:
                return -14f;
            case 2:
                return -12f;
            case 3:
                return -10f;
            case 4:
                return -8f;
            case 5:
                return -5f;
            case 6:
                return -4f;
            case 7:
                return -3f;
            case 8:
                return -2f;
            case 9:
                return -1f;
            case 10:
                return 1f;
            default:
                return -5f;
        }
    }

    // Because of physics weirdness, have to set the position to a flat number before imparting velocity on the ScrollRect
    public void ScrollArrowUpButton()
    {
        GameObject.FindGameObjectWithTag("SFXSource").GetComponent<Audio>().PopSound();
        ScrollArrowUp.SetActive(true);
        ScrollArrowDown.SetActive(false);
        _settingsScrollRect.verticalNormalizedPosition = 0.05f;
        _settingsScrollRect.StopMovement();
        _settingsScrollRect.velocity = Vector2.zero;
        _settingsScrollRect.velocity = new Vector2(0, -SCROLL_FORCE);
    }

    // Because of physics weirdness, have to set the position to a flat number before imparting velocity on the ScrollRect
    public void ScrollArrowDownButton()
    {
        GameObject.FindGameObjectWithTag("SFXSource").GetComponent<Audio>().PopSound();
        ScrollArrowUp.SetActive(false);
        ScrollArrowDown.SetActive(true);
        _settingsScrollRect.verticalNormalizedPosition = 0.95f;
        _settingsScrollRect.StopMovement();
        _settingsScrollRect.velocity = Vector2.zero;
        _settingsScrollRect.velocity = new Vector2(0, SCROLL_FORCE);
    }
}
