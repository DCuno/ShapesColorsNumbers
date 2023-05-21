using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    [SerializeField] string _volumeMusic = "Music";
    [SerializeField] string _volumeSFX = "SFX";
    [SerializeField] AudioMixer _mixer;
    [SerializeField] Slider _sliderMusic;
    [SerializeField] Slider _sliderSFX;
    [SerializeField] float _multiplier = 5f;

    public void HandleSFXSliderValueChanged(float value = 5f)
    {
        _mixer.SetFloat(_volumeSFX, Mathf.Log10(value) * _multiplier);
        GameObject.FindGameObjectWithTag("SoundEffectsSlider").GetComponent<TextMeshProUGUI>().text = _sliderSFX.value.ToString();
    }

    public void HandleMusicSliderValueChanged(float value = 5f)
    {
        _mixer.SetFloat(_volumeMusic, Mathf.Log10(value) * _multiplier);
        GameObject.FindGameObjectWithTag("MusicSlider").GetComponent<TextMeshProUGUI>().text = _sliderMusic.value.ToString();
    }

    // Start is called before the first frame update
    void Start()
    {
        _sliderMusic.value = PlayerPrefs.GetFloat(_volumeMusic, _sliderMusic.value);
        _sliderSFX.value = PlayerPrefs.GetFloat(_volumeSFX, _sliderSFX.value);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
