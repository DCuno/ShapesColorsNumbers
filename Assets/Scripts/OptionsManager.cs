using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionsManager : MonoBehaviour
{
    [SerializeField] public AudioMixer _mixer;
    [SerializeField] public Slider _soundEffectsSlider;
    [SerializeField] public Slider _musicSlider;
    [SerializeField] public TextMeshProUGUI _musicSliderCounter;
    [SerializeField] public TextMeshProUGUI _SFXSliderCounter;

    public float Multiplier = 30f;
    [Range(0, 1)] public float DefaultSliderPercentage = 0.31f;
    float curMusicSliderVal;
    float curSFXSliderVal;

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

        //LoadFromPrefs();

        curMusicSliderVal = _musicSlider.value;
        curSFXSliderVal = _soundEffectsSlider.value;

        SoundEffectsSlider();
        MusicSlider();
    }

    // Start is called before the first frame update
    void Start()
    {
        /*_soundEffectsSlider = GameObject.FindGameObjectWithTag("SoundEffectsSlider").GetComponent<Slider>();
        _musicSlider = GameObject.FindGameObjectWithTag("MusicSlider").GetComponent<Slider>();*/

        /*
        _musicSlider.value = PlayerPrefs.GetFloat("Music", _musicSlider.value);
        _soundEffectsSlider.value = PlayerPrefs.GetFloat("SFX", _soundEffectsSlider.value);
        curMusicSliderVal = _musicSlider.value;
        curSFXSliderVal = _soundEffectsSlider.value;
        //*/
    }

    // Update is called once per frame
    void Update()
    {
        if (curMusicSliderVal != _musicSlider.value)
        {
            StartCoroutine(SetMixerFloat("Music", SliderToDecibelMusic(_musicSlider.value)));
            curMusicSliderVal = _musicSlider.value;
            /*_mixer.SetFloat("Music", (float)_musicSlider.value);
            PlayerPrefs.SetFloat("Music", (float)_musicSlider.value);*/
        }

        if (curSFXSliderVal != _soundEffectsSlider.value)
        {
            StartCoroutine(SetMixerFloat("SFX", SliderToDecibelSFX(_soundEffectsSlider.value)));
            curSFXSliderVal = _soundEffectsSlider.value;
            /*_mixer.SetFloat("Music", (float)_musicSlider.value);
            PlayerPrefs.SetFloat("Music", (float)_musicSlider.value);*/
        }
    }

    private void LoadFromPrefs()
    {
        _musicSlider?.SetValueWithoutNotify(PlayerPrefs.GetFloat("Music"));
        _soundEffectsSlider?.SetValueWithoutNotify(PlayerPrefs.GetFloat("SFX"));
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
        SaveToPrefs();
        SceneManager.LoadScene(sceneName: "TitleScene");
    }

    public void SoundEffectsSlider()
    {
        _SFXSliderCounter.text = _soundEffectsSlider.value.ToString();
        //Slider _soundEffectsSlider = GameObject.FindGameObjectWithTag("SoundEffectsSlider").GetComponent<Slider>();
        //GameObject.FindGameObjectWithTag("SoundEffectsSliderCounter").GetComponent<TextMeshProUGUI>().text = _soundEffectsSlider.value.ToString();
        //_mixer.SetFloat("SFX", (float)_soundEffectsSlider.value);
        //PlayerPrefs.SetFloat("SFX", (float)_soundEffectsSlider.value);
    }

    public void MusicSlider()
    {
        _musicSliderCounter.text = _musicSlider.value.ToString();
        //GameObject.FindGameObjectWithTag("MusicSliderCounter").GetComponent<TextMeshProUGUI>().text = _musicSlider.value.ToString();
    }

    public IEnumerator SetMixerFloat(string name, float val)
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
    }

    private float SliderToDecibelMusic(float value)
    {
        switch (value)
        {
/*            case 0:
                return -80f;
            case 1:
                return -10f;
            case 2:
                return -9f;
            case 3:
                return -8f;
            case 4:
                return -7f;
            case 5:
                return -6f;
            case 6:
                return -5f;
            case 7:
                return -4f;
            case 8:
                return -3f;
            case 9:
                return -2f;
            case 10:
                return 0f;
            default:
                return -6f;*/

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
                return -6f;
            case 6:
                return -3f;
            case 7:
                return 0f;
            case 8:
                return 3f;
            case 9:
                return 6f;
            case 10:
                return 9f;
            default:
                return -6f;
        }
    }

    private float SliderToDecibelSFX(float value)
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
                return 0f;
            case 6:
                return 1f;
            case 7:
                return 2f;
            case 8:
                return 3f;
            case 9:
                return 4f;
            case 10:
                return 5f;
            default:
                return 6f;
        }
    }
}