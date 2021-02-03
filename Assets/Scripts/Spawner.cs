using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Spawner : MonoBehaviour
{
    public enum Colors { Red, Orange, Yellow, Green, Blue, Purple, White };
    public enum Topics { Shapes, Colors, Numbers, Off };

    public GameObject settingsCanvas;
    private GameObject spawnedSettingsCanvas;

    private const int minIter = 1, maxIter = 100, minSize = 1, maxSize = 10;
    public List<GameObject> shapesList = new List<GameObject>();
    public int count = 0;
    [Range(minIter, maxIter)]
    public int iterations;
    [Range(minSize, maxSize)]
    public int sizeSlider; // when size is maxSize(Default: 10) on the spawner, shape size is 0.7f. when size is minSize(Default: 1), shape size is 0.1f.
    public GameObject shape;

    private float finishedCheck = 0f;
    public bool finished = false;
    public bool started = false;
    public settingsStruct currentSettings;

    public struct settingsStruct
    {
        public List<Polygon.Shape> shapes;
        public List<Colors> colors;
        public float size;
        public float amount;
        public bool edges;
        public bool tilt;
        public Spawner.Topics voice;
        public Spawner.Topics text;
    }

    public void SettingsSetup(List<Polygon.Shape> shapes, List<Colors> colors, float size, float amount, bool edges, bool tilt, Topics voice, Topics text)
    {
        StartCoroutine(Spawn(shapes, colors, size, amount, edges, tilt, voice, text));
    }

    IEnumerator Spawn(List<Polygon.Shape> shapes, List<Colors> colors, float size, float amount, bool edges, bool tilt, Topics voice, Topics text)
    {
        shapesList.Clear();
        count = 0;
        finished = false;
        for (int i = 0; i < amount; i++)
        {
            yield return new WaitForSeconds(0.05f);
            shapesList.Add(Instantiate(shape, this.gameObject.transform.position, Quaternion.identity, this.gameObject.transform));
            shapesList[i].GetComponent<Polygon>().Creation(RandomShapeSelect(shapes), RandomColorSelect(colors), size, edges, tilt, voice, text);
            yield return new WaitForSeconds(0.08f);
        }

        currentSettings = new settingsStruct() { shapes = shapes, colors = colors, size = size, amount = amount, edges = edges, tilt = tilt, voice = voice, text = text };
    }

    // Updates in the editor
    private void OnValidate()
    {
        if (sizeSlider >= 9 && iterations > 4)
            iterations = 4;
        else if (sizeSlider >= 7 && iterations > 8)
            iterations = 8;
        else if (sizeSlider >= 6 && iterations > 10)
            iterations = 10;
        else if (sizeSlider >= 5 && iterations > 14)
            iterations = 14;
        else if (sizeSlider >= 4 && iterations > 24)
            iterations = 24;
        else if (sizeSlider >= 3 && iterations > 35)
            iterations = 35;
        else if (sizeSlider >= 2 && iterations > 50)
            iterations = 50;
        else if (sizeSlider >= 1 && iterations > 100)
            iterations = 100;
    }

    // Random Color returned out of all colors except white
    Color RandomColor()
    {
        int i = Random.Range(0, 6);

        return i switch
        {
            0 => Color.red,
            1 => Color.green,
            2 => Color.blue,
            3 => Color.yellow,
            4 => new Color(1.0f, 0.64f, 0.0f),// Orange
            5 => new Color(0.5f, 0f, 0.5f),// Purple
            _ => Color.white,
        };
    }

    // Random Color selected from given list
    public static Color RandomColorSelect(List<Colors> arr) =>
        arr[Random.Range(0, arr.Count)] switch
        {
            Colors.Red => Color.red,
            Colors.Green => Color.green,
            Colors.Blue => Color.blue,
            Colors.Yellow => Color.yellow,
            Colors.Orange => new Color(1.0f, 0.64f, 0.0f), // Orange
            Colors.Purple => new Color(0.5f, 0f, 0.5f), // Purple
            Colors.White => Color.white,
            _ => Color.white,
        };

    // Random Shape returned out of all shapes
    Polygon.Shape RandomShape()
    {
        int i = Random.Range(0, 6);

        switch (i)
        {
            case 0:
                return Polygon.Shape.Triangle;
            case 1:
                return Polygon.Shape.Square;
            case 2:
                return Polygon.Shape.Pentagon;
            case 3:
                return Polygon.Shape.Hexagon;
            case 4:
                return Polygon.Shape.Circle;
            case 5:
                return Polygon.Shape.Star;
            default:
                return Polygon.Shape.Circle;
        }
    }

    // Random Shape selected from given array
    Polygon.Shape RandomShapeSelect(List<Polygon.Shape> arr)
    {
        return arr[Random.Range(0, arr.Count)];
    }

    // Update is called once per frame
    void Update()
    {
        if (started)
        {
            finishedCheck += Time.deltaTime;

            // After 1 second of game running time, start checking if all the shapes are gone.
            if (finishedCheck >= 1.0f)
            {
                // Shapes have been popped, pull down menu.
                if (this.transform.childCount == 0)
                {
                    finished = true;
                    started = false;
                    finishedCheck = 0f;
                    spawnedSettingsCanvas = Instantiate(settingsCanvas);
                    spawnedSettingsCanvas.GetComponentInChildren<FunModeButtonManager>().FunModeButtonManagerConstructor(currentSettings);
                }
            }
        }

    }
}
