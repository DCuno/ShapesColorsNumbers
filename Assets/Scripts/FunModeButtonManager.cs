using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.;

public class FunModeButtonManager : MonoBehaviour
{
    public void PlayButton()
    {
        GameObject[] tempToggles;
        GameObject spawner = GameObject.FindGameObjectWithTag("spawner");
        GameObject settingsPanel = GameObject.FindGameObjectWithTag("SettingsPanel");

        List<Polygon.Shape> shapes = new List<Polygon.Shape>();

        tempToggles = GameObject.FindGameObjectsWithTag("ShapesPanelGroup");
        foreach (GameObject i in tempToggles)
        {
            if (i.GetComponent<Toggle>().isOn)
                if (System.Enum.TryParse(i.name, out Polygon.Shape result))
                    shapes.Add(result);
        }

        List<Spawner.Colors> colors = new List<Spawner.Colors>();

        tempToggles = GameObject.FindGameObjectsWithTag("ColorsPanelGroup");
        foreach (GameObject i in tempToggles)
        {
            if (i.GetComponent<Toggle>().isOn)
                if (System.Enum.TryParse(i.name, out Spawner.Colors result))
                    colors.Add(result);
        }

        int size;
        int amount;
        bool edges;
        bool gravity;
        bool tilt;
        List<Spawner.Topics> voice;
        List<Spawner.Topics> text;

        spawner.GetComponent<Spawner>().SettingsSetup(shapes, colors, size, amount, edges, gravity, tilt, voice, text);
    }

    public void BackButton()
    {

    }

    public void RandomButton()
    {

    }

    private void 
}
