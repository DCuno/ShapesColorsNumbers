using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionsManager : MonoBehaviour
{
    [SerializeField] private AudioMixer _mixer;
    [SerializeField] private Slider _soundEffectsSlider;
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private TextMeshProUGUI _musicSliderCounter;
    [SerializeField] private TextMeshProUGUI _SFXSliderCounter;

    public float Multiplier = 30f;
    [Range(0, 1)] public float DefaultSliderPercentage = 0.31f;
    float curMusicSliderVal;
    float curSFXSliderVal;
    Toggle _musicToggle;
    Toggle _sfxToggle;
    Toggle _batterySaverToggle;
    bool _curSFXMute;
    bool _curMusicMute;
    bool _curBatterySaverToggleOn;

    private void Awake()
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


        LoadFromPrefs();
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
        if (curMusicSliderVal != _musicSlider.value || _curMusicMute == _musicToggle.isOn)
        {
            _curMusicMute = !_musicToggle.isOn;
            curMusicSliderVal = _musicSlider.value;
            PlayerPrefs.SetFloat("Music", _musicSlider.value);
            PlayerPrefs.SetInt("MusicMute", _musicToggle.isOn == true ? 1 : 0);

            if (_curMusicMute)
                _mixer.SetFloat("Music", -80f);
            else
                _mixer.SetFloat("Music", OptionsManager.SliderToDecibelMusic(PlayerPrefs.GetFloat("Music", 5f)));
        }

        if (curSFXSliderVal != _soundEffectsSlider.value || _curSFXMute == _sfxToggle.isOn)
        {
            _curSFXMute = !_sfxToggle.isOn;
            curSFXSliderVal = _soundEffectsSlider.value;
            PlayerPrefs.SetFloat("SFX", _soundEffectsSlider.value);
            PlayerPrefs.SetInt("SFXMute", _sfxToggle.isOn == true ? 1 : 0);

            if (_curSFXMute)
                _mixer.SetFloat("SFX", -80f);
            else
                _mixer.SetFloat("SFX", OptionsManager.SliderToDecibelSFX(PlayerPrefs.GetFloat("SFX", 5f)));
        }

        if (_curBatterySaverToggleOn != _batterySaverToggle.isOn)
        {
            PlayerPrefs.SetInt("BatterySaver", _batterySaverToggle.isOn ? 1 : 0);
            _curBatterySaverToggleOn = _batterySaverToggle.isOn;
        }
    }

    private void LoadFromPrefs()
    {
        _musicSlider.value = PlayerPrefs.GetFloat("Music", 5f);
        _soundEffectsSlider.value = PlayerPrefs.GetFloat("SFX", 5f);
        _musicToggle.isOn = PlayerPrefs.GetInt("MusicMute", 1) == 1;
        _sfxToggle.isOn = PlayerPrefs.GetInt("SFXMute", 1) == 1;
        _batterySaverToggle.isOn = PlayerPrefs.GetInt("BatterySaver", 0) == 1 ? true : false;
        _curMusicMute = !_musicToggle.isOn;
        _curSFXMute = !_sfxToggle.isOn;
        _curBatterySaverToggleOn = _batterySaverToggle.isOn;
        curMusicSliderVal = _musicSlider.value;
        curSFXSliderVal = _soundEffectsSlider.value;

        if (_curMusicMute)
            _mixer.SetFloat("Music", -80f);
        else
            _mixer.SetFloat("Music", OptionsManager.SliderToDecibelMusic(PlayerPrefs.GetFloat("Music", 5f)));

        if (_curSFXMute)
            _mixer.SetFloat("SFX", -80f);
        else
            _mixer.SetFloat("SFX", OptionsManager.SliderToDecibelSFX(PlayerPrefs.GetFloat("SFX", 5f)));
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
        GameObject.FindGameObjectWithTag("SFXSource").GetComponent<Audio>().PopSound();
        SceneManager.LoadScene(sceneName: "TitleScene");
        //StartCoroutine(BackCoR());
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

    public void SoundEffectsSlider()
    {
        //Slider _soundEffectsSlider = GameObject.FindGameObjectWithTag("SoundEffectsSlider").GetComponent<Slider>();
        _mixer.SetFloat("SFX", OptionsManager.SliderToDecibelSFX(PlayerPrefs.GetFloat("SFX", 5f)));
        //PlayerPrefs.SetFloat("SFX", _soundEffectsSlider.value);
        //_SFXSliderCounter.text = _soundEffectsSlider.value.ToString();
        GameObject.FindGameObjectWithTag("SoundEffectsSliderCounter").GetComponent<TextMeshProUGUI>().text = _soundEffectsSlider.value.ToString();
    }

    public void MusicSlider()
    {
        //_musicSliderCounter.text = _musicSlider.value.ToString();
        _mixer.SetFloat("Music", OptionsManager.SliderToDecibelMusic(PlayerPrefs.GetFloat("Music", 5f)));
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
                return -80f;
            case 1:
                return -18f;
            case 2:
                return -15f;
            case 3:
                return -12f;
            case 4:
                return -9f;
            case 5:
                return -8f;
            case 6:
                return -7f; // new max
            case 7:
                return -6f;
            case 8:
                return -5f;
            case 9:
                return -4f;
            case 10:
                return -3f;
            default:
                return -3f;
        }
    }

    public static float SliderToDecibelSFX(float value)
    {
        switch (value)
        {
            case 0:
                return -80f;
            case 1:
                return -4f;
            case 2:
                return -3f;
            case 3:
                return -2f;
            case 4:
                return -1f;
            case 5:
                return 1f; // new max
            case 6:
                return 2f;
            case 7:
                return 3f;
            case 8:
                return 4f;
            case 9:
                return 5f;
            case 10:
                return 6f;
            default:
                return 6f;
        }
    }
}
