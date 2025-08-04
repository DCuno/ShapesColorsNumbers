using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class FunModeButtonManager : MonoBehaviour
{
    GameObject spawner;
    GameObject settingsPanel;
    List<Spawner.Shape> shapes;
    List<Spawner.Colors> colors;
    float size;
    float amount;
    bool edges;
    bool tilt;
    Spawner.Topics topic;
    bool voice;
    bool text;
    bool changingVoiceTextToggles = false;
    GameObject ScrollArrowUp;
    GameObject ScrollArrowDown;
    ScrollRect SettingsCanvasScrollRect;
    private Audio _sfx;
    private Image colorSliderBackground;

    private void Awake()
    {
        ScrollArrowUp = GameObject.FindGameObjectWithTag("ScrollArrowUp");
        ScrollArrowDown = GameObject.FindGameObjectWithTag("ScrollArrowDown");
        SettingsCanvasScrollRect = GameObject.FindGameObjectWithTag("SettingsCanvasScroll").GetComponentInChildren<ScrollRect>();
        GameObject.FindGameObjectWithTag("SettingsCanvasScroll").GetComponent<Canvas>().worldCamera = Camera.main;

        // Default is scroll arrow down is showing
        ScrollArrowUp.SetActive(false);
    }

    private void Start()
    {
        
    }

    public void FunModeButtonManagerConstructor(Spawner.SpawnerSettingsStruct SpawnerSettings)
    {
        shapes = SpawnerSettings.shapes;
        colors = SpawnerSettings.colors;
        size = SpawnerSettings.size;
        amount = SpawnerSettings.amount;
        edges = SpawnerSettings.edges;
        tilt = SpawnerSettings.tilt;
        //topic = SpawnerSettings.topic;
        //voice = SpawnerSettings.voice;
        //text = SpawnerSettings.text;

        Toggle[] tempToggles;

        tempToggles = GameObject.FindGameObjectWithTag("ShapesPanelGroup").GetComponentsInChildren<Toggle>();
        foreach (Toggle i in tempToggles)
        {
            System.Enum.TryParse(i.name, out Spawner.Shape result);
            if (shapes.Contains(result))
            {
                i.isOn = true;
            }
            else
            {
                i.isOn = false;
            }
        }

        tempToggles = GameObject.FindGameObjectWithTag("ColorsPanelGroup").GetComponentsInChildren<Toggle>();
        foreach (Toggle i in tempToggles)
        {
            System.Enum.TryParse(i.name, out Spawner.Colors result);
            if (colors.Contains(result))
            {
                i.isOn = true;
            }
            else
            {
                i.isOn = false;
            }
        }

        GameObject.FindGameObjectWithTag("SizePanelGroup").GetComponentInChildren<Slider>().value = size;
        GameObject.FindGameObjectWithTag("AmountPanelGroup").GetComponentInChildren<Slider>().value = amount;
        // Edgeless feature turned off... probably forever.
        //GameObject.FindGameObjectWithTag("EdgesToggleOn").GetComponent<Toggle>().isOn = edges;
        //GameObject.FindGameObjectWithTag("TiltToggleOn").GetComponent<Toggle>().isOn = tilt;

        /*tempToggles = GameObject.FindGameObjectWithTag("EdgesPanelGroup").GetComponentsInChildren<Toggle>();
        foreach (Toggle i in tempToggles)
        {
            if (i.name.Equals("On"))
            {
                if (edges)
                    i.isOn = true;
                else
                    i.isOn = false;
            }
            else if (i.name.Equals("Off"))
            {
                if (edges)
                    i.isOn = false;
                else
                    i.isOn = true;
            }
        }*/

        /*tempToggles = GameObject.FindGameObjectWithTag("TiltPanelGroup").GetComponentsInChildren<Toggle>();
        foreach (Toggle i in tempToggles)
        {
            if (i.name.Equals("On"))
            {
                if (tilt)
                    i.isOn = true;
                else
                    i.isOn = false;
            }
            else if (i.name.Equals("Off"))
            {
                if (tilt)
                    i.isOn = false;
                else
                    i.isOn = true;
            }
        }*/

        //// Shapes, Colors, Numbers topic toggle
        //tempToggles = GameObject.FindGameObjectWithTag("TopicsPanelGroup").GetComponentsInChildren<Toggle>();
        //foreach (Toggle i in tempToggles)
        //{
        //    System.Enum.TryParse(i.name, out Spawner.Topics result);
        //    if (topic.Equals(result))
        //    {
        //        i.isOn = true;
        //    }
        //    else
        //    {
        //        i.isOn = false;
        //    }
        //}

        //// Listen toggle
        //tempToggles = GameObject.FindGameObjectWithTag("VoicePanelGroup").GetComponentsInChildren<Toggle>();
        //foreach (Toggle i in tempToggles)
        //{
        //    if (i.name.Equals("Toggle"))
        //    {
        //        if (voice)
        //            i.isOn = true;
        //        else
        //            i.isOn = false;
        //    }
        //    else if (i.name.Equals("Off"))
        //    {
        //        if (voice)
        //            i.isOn = false;
        //        else
        //            i.isOn = true;
        //    }
        //}

        //// Read toggle
        //tempToggles = GameObject.FindGameObjectWithTag("TextPanelGroup").GetComponentsInChildren<Toggle>();
        //foreach (Toggle i in tempToggles)
        //{
        //    if (i.name.Equals("Toggle"))
        //    {
        //        if (text)
        //            i.isOn = true;
        //        else
        //            i.isOn = false;
        //    }
        //    else if (i.name.Equals("Off"))
        //    {
        //        if (text)
        //            i.isOn = false;
        //        else
        //            i.isOn = true;
        //    }
        //}
    }

    public void PlayButton()
    {
        Toggle[] tempToggles;

        spawner = GameObject.FindGameObjectWithTag("spawner");
        settingsPanel = GameObject.FindGameObjectWithTag("SettingsPanel");

        shapes = new List<Spawner.Shape>();
        GameObject.FindGameObjectWithTag("SFXSource").GetComponent<Audio>().PopSound();

        tempToggles = GameObject.FindGameObjectWithTag("ShapesPanelGroup").GetComponentsInChildren<Toggle>();
        foreach (Toggle i in tempToggles)
        {
            if (i.isOn && System.Enum.TryParse(i.name, out Spawner.Shape result))
            {  
                shapes.Add(result);
            }
        }

        if (shapes.Count == 0)
            shapes.Add(Spawner.Shape.Circle);

        colors = new List<Spawner.Colors>();

        tempToggles = GameObject.FindGameObjectWithTag("ColorsPanelGroup").GetComponentsInChildren<Toggle>();
        foreach (Toggle i in tempToggles)
        {
            if (i.isOn)
                if (System.Enum.TryParse(i.name, out Spawner.Colors result))
                    colors.Add(result);

        }

        if (colors.Count == 0)
            colors.Add(Spawner.Colors.White);

        size = GameObject.FindGameObjectWithTag("SizePanelGroup").GetComponentInChildren<Slider>().value;
        amount = GameObject.FindGameObjectWithTag("AmountPanelGroup").GetComponentInChildren<Slider>().value;
        // Edgeless feature turned off... probably forever.
        //edges = GameObject.FindGameObjectWithTag("EdgesToggleOn").GetComponent<Toggle>().isOn;
        edges = true;
        //tilt = GameObject.FindGameObjectWithTag("TiltToggleOn").GetComponent<Toggle>().isOn;
        tilt = false;


        //tempToggles = GameObject.FindGameObjectWithTag("TopicsPanelGroup").GetComponentsInChildren<Toggle>();
        //foreach (Toggle i in tempToggles)
        //{
        //    if (i.isOn)
        //        if (System.Enum.TryParse(i.name, out Spawner.Topics result))
        //        {
        //            topic = result;
        //            break;
        //        }
        //}

        // Setting learning topic, voice on/off, text on/off
        topic = (Spawner.Topics) PlayerPrefs.GetInt("LearningTopic", 0);
        voice = PlayerPrefs.GetInt("Listen", 1) == 1;
        text = PlayerPrefs.GetInt("Read", 1) == 1;

        //voice = GameObject.FindGameObjectWithTag("VoiceOnToggle").GetComponent<Toggle>().isOn;
        //text = GameObject.FindGameObjectWithTag("TextOnToggle").GetComponent<Toggle>().isOn;

        spawner.GetComponent<Spawner>().SettingsSetup(shapes, colors, size, amount, edges, tilt, topic, voice, text);
        spawner.GetComponent<Spawner>().Started = true;
        //Destroy(GameObject.FindGameObjectWithTag("SettingsCanvasScroll"));
    }

    public void BackButton()
    {
        GameObject.FindGameObjectWithTag("SFXSource").GetComponent<Audio>().PopSound();
        //GameObject.FindGameObjectWithTag("SFXSource").GetComponent<Audio>().PopSound();
        SceneManager.LoadScene(sceneName: "TitleScene");
    }

    public void RandomButton()
    {
        GameObject.FindGameObjectWithTag("SFXSource").GetComponent<Audio>().PopSound();

        // Randomize shapes
        Toggle[] tempToggles;
        tempToggles = GameObject.FindGameObjectWithTag("ShapesPanelGroup").GetComponentsInChildren<Toggle>();
        foreach (Toggle i in tempToggles)
        {
            System.Enum.TryParse(i.name, out Spawner.Shape result);
            if (Random.Range(0, 2) == 0)
            {
                i.isOn = true;
            }
            else
            {
                i.isOn = false;
            }
        }

        // Randomize colors
        tempToggles = GameObject.FindGameObjectWithTag("ColorsPanelGroup").GetComponentsInChildren<Toggle>();
        foreach (Toggle i in tempToggles)
        {
            System.Enum.TryParse(i.name, out Spawner.Colors result);
            if (Random.Range(0, 2) == 0)
            {
                i.isOn = true;
            }
            else
            {
                i.isOn = false;
            }
        }

        // Count how many color toggles are on
        int toggleOnCount = 0;
        foreach (Toggle i in tempToggles)
        {
            if (i.isOn)
            {
                toggleOnCount++;
            }
        }

        // Randomize size slider
        Slider sizeSlider = GameObject.FindGameObjectWithTag("SizePanelGroup").GetComponentInChildren<Slider>();
        Slider amountSlider = GameObject.FindGameObjectWithTag("AmountPanelGroup").GetComponentInChildren<Slider>();
        sizeSlider.value = Random.Range(1, 11);

        // Set amount slider based on size (your existing logic)
        if (sizeSlider.value >= 9)
        {
            amountSlider.maxValue = 6; // Fixed: was 4, should match your other methods
            amountSlider.value = Random.Range(Mathf.Max(1, toggleOnCount), amountSlider.maxValue + 1);
            amountSlider.minValue = Mathf.Max(1, toggleOnCount);
        }
        else if (sizeSlider.value >= 7)
        {
            amountSlider.maxValue = 8;
            amountSlider.value = Random.Range(Mathf.Max(1, toggleOnCount), amountSlider.maxValue + 1);
            amountSlider.minValue = Mathf.Max(1, toggleOnCount);
        }
        else if (sizeSlider.value >= 6)
        {
            amountSlider.maxValue = 10;
            amountSlider.value = Random.Range(Mathf.Max(1, toggleOnCount), amountSlider.maxValue + 1);
            amountSlider.minValue = Mathf.Max(1, toggleOnCount);
        }
        else if (sizeSlider.value >= 5)
        {
            amountSlider.maxValue = 14;
            amountSlider.value = Random.Range(Mathf.Max(1, toggleOnCount), amountSlider.maxValue + 1);
            amountSlider.minValue = Mathf.Max(1, toggleOnCount);
        }
        else if (sizeSlider.value >= 4)
        {
            amountSlider.maxValue = 24;
            amountSlider.value = Random.Range(Mathf.Max(1, toggleOnCount), amountSlider.maxValue + 1);
            amountSlider.minValue = Mathf.Max(1, toggleOnCount);
        }
        else if (sizeSlider.value >= 3)
        {
            amountSlider.maxValue = 35;
            amountSlider.value = Random.Range(Mathf.Max(1, toggleOnCount), amountSlider.maxValue + 1);
            amountSlider.minValue = Mathf.Max(1, toggleOnCount);
        }
        else if (sizeSlider.value >= 2)
        {
            amountSlider.maxValue = 50;
            amountSlider.value = Random.Range(Mathf.Max(1, toggleOnCount), amountSlider.maxValue + 1);
            amountSlider.minValue = Mathf.Max(1, toggleOnCount);
        }
        else if (sizeSlider.value >= 1)
        {
            amountSlider.maxValue = 100;
            amountSlider.value = Random.Range(Mathf.Max(1, toggleOnCount), amountSlider.maxValue + 1);
            amountSlider.minValue = Mathf.Max(1, toggleOnCount);
        }

        // Handle the min == max visual case
        HandleSliderMinMaxEqual();
    }

    public void ColorPanelToggle()
    {
        Toggle[] toggles = GameObject.FindGameObjectWithTag("ColorsPanelGroup").GetComponentsInChildren<Toggle>();
        Slider amountSlider = GameObject.FindGameObjectWithTag("AmountPanelGroup").GetComponent<Slider>();
        GameObject.FindGameObjectWithTag("AmountSliderCounter").GetComponent<TextMeshProUGUI>().text = amountSlider.value.ToString();
        int toggleOnCount = 0;
        foreach (Toggle i in toggles)
        {
            //System.Enum.TryParse(i.name, out Spawner.Colors result);
            if (i.isOn)
            {
                toggleOnCount++;
            }
        }

        if (amountSlider.value < toggleOnCount)
        {
            amountSlider.value = toggleOnCount;
        }
        
        amountSlider.minValue = Mathf.Max(1, toggleOnCount);

        HandleSliderMinMaxEqual();
    }

    public void SizeSliderAmount()
    {
        Slider sizeSlider = GameObject.FindGameObjectWithTag("SizePanelGroup").GetComponent<Slider>();
        Slider amountSlider = GameObject.FindGameObjectWithTag("AmountPanelGroup").GetComponent<Slider>();
        GameObject.FindGameObjectWithTag("SizeSliderCounter").GetComponent<TextMeshProUGUI>().text = sizeSlider.value.ToString();

        if (sizeSlider.value >= 9)
        {
            if (amountSlider.value > 6)
                amountSlider.value = 6;
            
            amountSlider.maxValue = 6;
        }
        else if (sizeSlider.value >= 7)
        {
            if (amountSlider.value > 8)
                amountSlider.value = 8;
            
            amountSlider.maxValue = 8;
        }
        else if (sizeSlider.value >= 6)
        {
            if (amountSlider.value > 10)
                amountSlider.value = 10;
            
            amountSlider.maxValue = 10;
        }
        else if (sizeSlider.value >= 5)
        {
            if (amountSlider.value > 14)
                amountSlider.value = 14;
            
            amountSlider.maxValue = 14;
        }
        else if (sizeSlider.value >= 4)
        {
            if (amountSlider.value > 24)
                amountSlider.value = 24;

            amountSlider.maxValue = 24;
        }
        else if (sizeSlider.value >= 3)
        {
            if (amountSlider.value > 35)
                amountSlider.value = 35;
            
            amountSlider.maxValue = 35;
        }
        else if (sizeSlider.value >= 2)
        {
            if (amountSlider.value > 50)
                amountSlider.value = 50;
            
            amountSlider.maxValue = 50;
        }
        else if (sizeSlider.value >= 1)
        {
            if (amountSlider.value > 100)
                amountSlider.value = 100;
            
            amountSlider.maxValue = 100;
        }

        HandleSliderMinMaxEqual();
    }

    public void AmountSliderAmount()
    {
        Toggle[] toggles = GameObject.FindGameObjectWithTag("ColorsPanelGroup").GetComponentsInChildren<Toggle>();
        Slider amountSlider = GameObject.FindGameObjectWithTag("AmountPanelGroup").GetComponent<Slider>();
        GameObject.FindGameObjectWithTag("AmountSliderCounter").GetComponent<TextMeshProUGUI>().text = amountSlider.value.ToString();
        int toggleOnCount = 0;
        foreach (Toggle i in toggles)
        {
            //System.Enum.TryParse(i.name, out Spawner.Colors result);
            if (i.isOn)
            {
                toggleOnCount++;
            }
        }

        if (amountSlider.value < toggleOnCount)
        { 
            amountSlider.value = toggleOnCount;
        }

        if (amountSlider.minValue < toggleOnCount)
        {
            amountSlider.minValue = toggleOnCount;
        }

        HandleSliderMinMaxEqual();
    }

    private void HandleSliderMinMaxEqual()
    {
        Slider amountSlider = GameObject.FindGameObjectWithTag("AmountPanelGroup").GetComponent<Slider>();
        colorSliderBackground = GameObject.FindGameObjectWithTag("AmountPanelGroup").GetComponent<Slider>().transform.Find("Background")?.GetComponent<Image>();

        if (amountSlider.minValue == amountSlider.maxValue)
        {
            // Force fill to appear full
            amountSlider.fillRect.anchorMax = new Vector2(1f, amountSlider.fillRect.anchorMax.y);
            // Force handle to appear at the right
            amountSlider.handleRect.anchorMin = new Vector2(1f, amountSlider.handleRect.anchorMin.y);
            amountSlider.handleRect.anchorMax = new Vector2(1f, amountSlider.handleRect.anchorMax.y);

            // Set background to white
            if (colorSliderBackground != null)
            {
                colorSliderBackground.color = Color.white;
            }
        }
        else
        {
            // Restore original background color
            if (colorSliderBackground != null)
            {
                // Convert hex #323232 to Color
                colorSliderBackground.color = new Color32(50, 50, 50, 255); // #323232 in RGB
            }
        }
    }

    public void VoiceToggles(Toggle toggle)
    {
        // Keep the onValueChanged from infinite looping
        if (!changingVoiceTextToggles)
        {
            changingVoiceTextToggles = true;

            // Don't change anything if the TextToggle "Off" is on.
            if (!GameObject.FindGameObjectWithTag("TextOnToggle").GetComponent<Toggle>().isOn)
            {
                changingVoiceTextToggles = false;
                return;
            }

            Toggle[] textToggles = GameObject.FindGameObjectWithTag("TextPanelGroup").GetComponentsInChildren<Toggle>();
            System.Enum.TryParse(toggle.name, out Spawner.Topics voiceTopic);

            // Change the TextToggles to match the VoiceToggles
            foreach (Toggle i in textToggles)
            {
                System.Enum.TryParse(i.name, out Spawner.Topics result);
                if (voiceTopic.Equals(result))
                {
                    i.isOn = true;
                }
                else
                {
                    i.isOn = false;
                }
            }
            changingVoiceTextToggles = false;
        }
    }

    public void TextToggles(Toggle toggle)
    {
        // Keep the onValueChanged from infinite looping
        if (!changingVoiceTextToggles)
        {
            changingVoiceTextToggles = true;

            // Don't change anything if the VoiceToggle "Off" is on.
            if (!GameObject.FindGameObjectWithTag("TextOnToggle").GetComponent<Toggle>().isOn)
            {
                changingVoiceTextToggles = false;
                return;
            }

            Toggle[] voiceToggles = GameObject.FindGameObjectWithTag("TextPanelGroup").GetComponentsInChildren<Toggle>();
            System.Enum.TryParse(toggle.name, out Spawner.Topics textTopic);

            // Change the VoiceToggles to match the TextToggles
            foreach (Toggle i in voiceToggles)
            {
                System.Enum.TryParse(i.name, out Spawner.Topics result);
                if (textTopic.Equals(result))
                {
                    i.isOn = true;
                }
                else
                {
                    i.isOn = false;
                }
            }

            changingVoiceTextToggles = false;
        }
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

    // Because of physics weirdness, have to set the position to a flat number before imparting velocity on the ScrollRect
    public void ScrollArrowUpButton()
    {
        GameObject.FindGameObjectWithTag("SFXSource").GetComponent<Audio>().PopSound();
        ScrollArrowUp.SetActive(true);
        ScrollArrowDown.SetActive(false);
        SettingsCanvasScrollRect.verticalNormalizedPosition = 0.05f;
        SettingsCanvasScrollRect.StopMovement();
        SettingsCanvasScrollRect.velocity = Vector2.zero;
        SettingsCanvasScrollRect.velocity = new Vector2(0, -2500f);
    }
    
    // Because of physics weirdness, have to set the position to a flat number before imparting velocity on the ScrollRect
    public void ScrollArrowDownButton()
    {
        GameObject.FindGameObjectWithTag("SFXSource").GetComponent<Audio>().PopSound();
        ScrollArrowUp.SetActive(false);
        ScrollArrowDown.SetActive(true);
        SettingsCanvasScrollRect.verticalNormalizedPosition = 0.95f;
        SettingsCanvasScrollRect.StopMovement();
        SettingsCanvasScrollRect.velocity = Vector2.zero;
        SettingsCanvasScrollRect.velocity = new Vector2(0, 2500f);
    }
}
