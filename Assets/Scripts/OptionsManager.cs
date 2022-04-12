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

    float curMusicSliderVal;
    float curSFXSliderVal;

    // Start is called before the first frame update
    void Start()
    {
        _musicSlider.value = PlayerPrefs.GetFloat("MusicSlider", _musicSlider.value);
        _musicSliderCounter.text = _musicSlider.value.ToString();
        _soundEffectsSlider.value = PlayerPrefs.GetFloat("SFXSlider", _soundEffectsSlider.value);
        _SFXSliderCounter.text = _soundEffectsSlider.value.ToString();
        //_mixer.SetFloat("Music", PlayerPrefs.GetFloat("MusicVol", SliderToDecibelMusic(_musicSlider.value)));
        //_mixer.SetFloat("SFX", PlayerPrefs.GetFloat("SFXVol", SliderToDecibelSFX(_soundEffectsSlider.value)));
        StartCoroutine(SetMixerFloat("Music", SliderToDecibelMusic(_musicSlider.value)));
        StartCoroutine(SetMixerFloat("SFX", SliderToDecibelSFX(_soundEffectsSlider.value)));
        curMusicSliderVal = _musicSlider.value;
        curSFXSliderVal = _soundEffectsSlider.value;
    }

    // Update is called once per frame
    void Update()
    {
        // Back key leave to Title
        if (Input.GetKeyDown(KeyCode.Escape))
            SceneManager.LoadScene(sceneName: "TitleScene");

        if (curMusicSliderVal != _musicSlider.value)
        {
            _musicSliderCounter.text = _musicSlider.value.ToString();
            _mixer.SetFloat("Music", SliderToDecibelMusic(_musicSlider.value));
            PlayerPrefs.SetFloat("MusicSlider", _musicSlider.value);
            PlayerPrefs.SetFloat("MusicVol", SliderToDecibelMusic(_musicSlider.value));
            curMusicSliderVal = _musicSlider.value;
        }

        if (curSFXSliderVal != _soundEffectsSlider.value)
        {
            _SFXSliderCounter.text = _soundEffectsSlider.value.ToString();
            _mixer.SetFloat("SFX", SliderToDecibelSFX(_soundEffectsSlider.value));
            PlayerPrefs.SetFloat("SFXSlider", _soundEffectsSlider.value);
            PlayerPrefs.SetFloat("SFXVol", SliderToDecibelSFX(_soundEffectsSlider.value));
            curSFXSliderVal = _soundEffectsSlider.value;
        }
    }

    public void BackButton()
    {
        SceneManager.LoadScene(sceneName: "TitleScene");
    }

    public IEnumerator SetMixerFloat(string name, float val)
    {
        bool result = false;

        while (!result)
        {
            result = _mixer.SetFloat(name, val);

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
                return -12f;
            case 2:
                return -9f;
            case 3:
                return -6f;
            case 4:
                return -3f;
            case 5:
                return 0f;
            case 6:
                return 3f;
            case 7:
                return 6f;
            case 8:
                return 9f;
            case 9:
                return 12f;
            case 10:
                return 15f;
            default:
                return 0f;
        }
    }
}
