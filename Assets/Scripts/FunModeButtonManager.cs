using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FunModeButtonManager : MonoBehaviour
{
    GameObject spawner;
    GameObject settingsPanel;
    List<Polygon.Shape> shapes;
    List<Spawner.Colors> colors;
    float size;
    float amount;
    bool edges;
    bool tilt;
    Spawner.Topics voice;
    Spawner.Topics text;

    private void Awake()
    {

    }

    public void FunModeButtonManagerConstructor(Spawner.settingsStruct settingsStruct)
    {
        shapes = settingsStruct.shapes;
        colors = settingsStruct.colors;
        size = settingsStruct.size;
        amount = settingsStruct.amount;
        edges = settingsStruct.edges;
        tilt = settingsStruct.tilt;
        voice = settingsStruct.voice;
        text = settingsStruct.text;

        Toggle[] tempToggles;

        //shapes = new List<Polygon.Shape>();

        tempToggles = GameObject.FindGameObjectWithTag("ShapesPanelGroup").GetComponentsInChildren<Toggle>();
        foreach (Toggle i in tempToggles)
        {
            System.Enum.TryParse(i.name, out Polygon.Shape result);
            if (shapes.Contains(result))
            {
                i.isOn = true;
            }
            else
            {
                i.isOn = false;
            }
        }

        /*if (shapes.Count == 0)
            shapes.Add(Polygon.Shape.Circle);*/

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

        /*if (colors.Count == 0)
            colors.Add(Spawner.Colors.White);*/

        GameObject.FindGameObjectWithTag("SizePanelGroup").GetComponentInChildren<Slider>().value = size;
        GameObject.FindGameObjectWithTag("AmountPanelGroup").GetComponentInChildren<Slider>().value = amount;
        GameObject.FindGameObjectWithTag("EdgesToggleOn").GetComponent<Toggle>().isOn = edges;
        GameObject.FindGameObjectWithTag("TiltToggleOn").GetComponent<Toggle>().isOn = tilt;

        tempToggles = GameObject.FindGameObjectWithTag("EdgesPanelGroup").GetComponentsInChildren<Toggle>();
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
        }

        tempToggles = GameObject.FindGameObjectWithTag("TiltPanelGroup").GetComponentsInChildren<Toggle>();
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
        }

        tempToggles = GameObject.FindGameObjectWithTag("VoicePanelGroup").GetComponentsInChildren<Toggle>();
        foreach (Toggle i in tempToggles)
        {
            System.Enum.TryParse(i.name, out Spawner.Topics result);
            if (voice.Equals(result))
            {
                i.isOn = true;
            }
            else
            {
                i.isOn = false;
            }
        }

        tempToggles = GameObject.FindGameObjectWithTag("TextPanelGroup").GetComponentsInChildren<Toggle>();
        foreach (Toggle i in tempToggles)
        {
            System.Enum.TryParse(i.name, out Spawner.Topics result);
            if (text.Equals(result))
            {
                i.isOn = true;
            }
            else
            {
                i.isOn = false;
            }
        }
    }

    public void PlayButton()
    {
        Toggle[] tempToggles;

        spawner = GameObject.FindGameObjectWithTag("spawner");
        settingsPanel = GameObject.FindGameObjectWithTag("SettingsPanel");

        shapes = new List<Polygon.Shape>();

        tempToggles = GameObject.FindGameObjectWithTag("ShapesPanelGroup").GetComponentsInChildren<Toggle>();
        foreach (Toggle i in tempToggles)
        {
            if (i.isOn)
                if (System.Enum.TryParse(i.name, out Polygon.Shape result))
                    shapes.Add(result);

        }

        if (shapes.Count == 0)
            shapes.Add(Polygon.Shape.Circle);

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
        edges = GameObject.FindGameObjectWithTag("EdgesToggleOn").GetComponent<Toggle>().isOn;
        tilt = GameObject.FindGameObjectWithTag("TiltToggleOn").GetComponent<Toggle>().isOn;

        voice = Spawner.Topics.Off;

        tempToggles = GameObject.FindGameObjectWithTag("VoicePanelGroup").GetComponentsInChildren<Toggle>();
        foreach (Toggle i in tempToggles)
        {
            if (i.isOn)
                if (System.Enum.TryParse(i.name, out Spawner.Topics result))
                {
                    voice = result;
                    break;
                }
        }

        text = Spawner.Topics.Off;

        tempToggles = GameObject.FindGameObjectWithTag("TextPanelGroup").GetComponentsInChildren<Toggle>();
        foreach (Toggle i in tempToggles)
        {
            if (i.isOn)
                if (System.Enum.TryParse(i.name, out Spawner.Topics result))
                {
                    text = result;
                    break;
                }
        }

        spawner.GetComponent<Spawner>().SettingsSetup(shapes, colors, size, amount, edges, tilt, voice, text);
        spawner.GetComponent<Spawner>().started = true;
        Destroy(GameObject.FindGameObjectWithTag("SettingsCanvas"));
    }

    public void BackButton()
    {

    }

    public void RandomButton()
    {

    }

    public void SizeSliderAmount()
    {
        Slider sizeSlider = GameObject.FindGameObjectWithTag("SizePanelGroup").GetComponent<Slider>();
        Slider amountSlider = GameObject.FindGameObjectWithTag("AmountPanelGroup").GetComponent<Slider>();
        GameObject.FindGameObjectWithTag("SizeSliderCounter").GetComponent<TextMeshProUGUI>().text = sizeSlider.value.ToString();

        if (sizeSlider.value >= 9)
        {
            if (amountSlider.value > 4)
                amountSlider.value = 4;
            
            amountSlider.maxValue = 4;
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
    }

    public void AmountSliderAmount()
    {
        Slider amountSlider = GameObject.FindGameObjectWithTag("AmountPanelGroup").GetComponent<Slider>();
        GameObject.FindGameObjectWithTag("AmountSliderCounter").GetComponent<TextMeshProUGUI>().text = amountSlider.value.ToString();
    }
}
