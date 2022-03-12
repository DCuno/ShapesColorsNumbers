using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LessonsManager : MonoBehaviour
{
    List<GameObject> ShapesList;
    List<GameObject> ColorsList;
    List<GameObject> NumbersList;
    GameObject spawner;
    private static AudioSource _audioSource;
    private static Audio _audio;

    void Start()
    {
        ShapesList = new List<GameObject>(6);
        ColorsList = new List<GameObject>(6);
        NumbersList = new List<GameObject>(10);
        spawner = GameObject.FindGameObjectWithTag("spawner");

        _audioSource = FindObjectOfType<AudioSource>();
        _audio = _audioSource.GetComponent<Audio>();

        ShapesList.Add(GameObject.FindGameObjectWithTag("TriangleShape"));
        ShapesList.Add(GameObject.FindGameObjectWithTag("SquareShape"));
        ShapesList.Add(GameObject.FindGameObjectWithTag("PentagonShape"));
        ShapesList.Add(GameObject.FindGameObjectWithTag("HexagonShape"));
        ShapesList.Add(GameObject.FindGameObjectWithTag("CircleShape"));
        ShapesList.Add(GameObject.FindGameObjectWithTag("StarShape"));

        ColorsList.Add(GameObject.FindGameObjectWithTag("RedColor"));
        ColorsList.Add(GameObject.FindGameObjectWithTag("OrangeColor"));
        ColorsList.Add(GameObject.FindGameObjectWithTag("YellowColor"));
        ColorsList.Add(GameObject.FindGameObjectWithTag("GreenColor"));
        ColorsList.Add(GameObject.FindGameObjectWithTag("BlueColor"));
        ColorsList.Add(GameObject.FindGameObjectWithTag("PurpleColor"));

        NumbersList.Add(GameObject.FindGameObjectWithTag("1Number"));
        NumbersList.Add(GameObject.FindGameObjectWithTag("2Number"));
        NumbersList.Add(GameObject.FindGameObjectWithTag("3Number"));
        NumbersList.Add(GameObject.FindGameObjectWithTag("4Number"));
        NumbersList.Add(GameObject.FindGameObjectWithTag("5Number"));
        NumbersList.Add(GameObject.FindGameObjectWithTag("6Number"));
        NumbersList.Add(GameObject.FindGameObjectWithTag("7Number"));
        NumbersList.Add(GameObject.FindGameObjectWithTag("8Number"));
        NumbersList.Add(GameObject.FindGameObjectWithTag("9Number"));
        NumbersList.Add(GameObject.FindGameObjectWithTag("10Number"));

        foreach (GameObject i in ShapesList)
        {
            i.gameObject.SetActive(false);
        }

        foreach (GameObject i in ColorsList)
        {
            i.gameObject.SetActive(false);
        }

        foreach (GameObject i in NumbersList)
        {
            i.gameObject.SetActive(false);
        }

        StartCoroutine(Lessons());
    }

    IEnumerator Lessons()
    {
        int idx = 0;
        List<Polygon.Shape> _allShapes = new List<Polygon.Shape>{ Polygon.Shape.Triangle, Polygon.Shape.Square, Polygon.Shape.Pentagon, Polygon.Shape.Hexagon, Polygon.Shape.Circle, Polygon.Shape.Star };
        List<Spawner.Colors> _allColors = new List<Spawner.Colors> { Spawner.Colors.Red, Spawner.Colors.Orange, Spawner.Colors.Yellow, Spawner.Colors.Green, Spawner.Colors.Blue, Spawner.Colors.Purple };

        foreach (GameObject i in ShapesList)
        {
            LessonsManager.VoiceSound(Spawner.Topics.Shapes, (Polygon.Shape)idx, (Spawner.Colors)0, 0);
            i.gameObject.SetActive(true);
            yield return new WaitForSeconds(3);
            i.gameObject.SetActive(false);

            spawner.GetComponent<Spawner>().SettingsSetup(new List<Polygon.Shape> { (Polygon.Shape)idx }, new List<Spawner.Colors> { Spawner.Colors.White }, 5, 1, true, false, Spawner.Topics.Shapes, Spawner.Topics.Shapes);
            spawner.GetComponent<Spawner>().started = true;

            yield return new WaitWhile(() => spawner.GetComponent<Spawner>().started);

            spawner.GetComponent<Spawner>().SettingsSetup(new List<Polygon.Shape> { (Polygon.Shape)idx }, new List<Spawner.Colors> { Spawner.Colors.White }, 5, 7, true, false, Spawner.Topics.Shapes, Spawner.Topics.Shapes);
            spawner.GetComponent<Spawner>().started = true;

            yield return new WaitWhile(() => spawner.GetComponent<Spawner>().started);
            idx++;
        }

        idx = 0;
        foreach (GameObject i in ColorsList)
        {
            LessonsManager.VoiceSound(Spawner.Topics.Colors, (Polygon.Shape)0, (Spawner.Colors)idx, 0);
            i.gameObject.SetActive(true);
            yield return new WaitForSeconds(3);
            i.gameObject.SetActive(false);

            spawner.GetComponent<Spawner>().SettingsSetup(_allShapes, new List<Spawner.Colors> { (Spawner.Colors)idx }, 5, 1, true, false, Spawner.Topics.Colors, Spawner.Topics.Colors);
            spawner.GetComponent<Spawner>().started = true;

            yield return new WaitWhile(() => spawner.GetComponent<Spawner>().started);

            spawner.GetComponent<Spawner>().SettingsSetup(_allShapes, new List<Spawner.Colors> { (Spawner.Colors)idx }, 5, 7, true, false, Spawner.Topics.Colors, Spawner.Topics.Colors);
            spawner.GetComponent<Spawner>().started = true;

            yield return new WaitWhile(() => spawner.GetComponent<Spawner>().started);
            idx++;
        }

        idx = 0;
        foreach (GameObject i in NumbersList)
        {
            LessonsManager.VoiceSound(Spawner.Topics.Numbers, (Polygon.Shape)0, (Spawner.Colors)0, idx);
            i.gameObject.SetActive(true);
            yield return new WaitForSeconds(3);
            i.gameObject.SetActive(false);

            spawner.GetComponent<Spawner>().SettingsSetup(_allShapes, _allColors, 5, idx + 1, true, false, Spawner.Topics.Numbers, Spawner.Topics.Numbers);
            spawner.GetComponent<Spawner>().started = true;

            yield return new WaitWhile(() => spawner.GetComponent<Spawner>().started);
            idx++;
        }

        SceneManager.LoadScene(sceneName: "TitleScene");
        yield return null;
    }

    IEnumerator TitleShowTest()
    {
        foreach (GameObject i in ShapesList)
        {
            i.gameObject.SetActive(true);
            yield return new WaitForSeconds(3);
            i.gameObject.SetActive(false);
        }

        foreach (GameObject i in ColorsList)
        {
            i.gameObject.SetActive(true);
            yield return new WaitForSeconds(3);
            i.gameObject.SetActive(false);
        }

        foreach (GameObject i in NumbersList)
        {
            i.gameObject.SetActive(true);
            yield return new WaitForSeconds(3);
            i.gameObject.SetActive(false);
        }
    }

    void Update()
    {

    }

    private static void VoiceSound(Spawner.Topics _voice, Polygon.Shape _shape, Spawner.Colors _color, int _number)
    {
        if (_voice == Spawner.Topics.Shapes)
        {
            _audioSource.PlayOneShot(_audio.voices_shapes[(int)_shape]);
        }
        else if (_voice == Spawner.Topics.Colors)
        {
            _audioSource.PlayOneShot(_audio.voices_colors[(int)_color]);
        }
        else if (_voice == Spawner.Topics.Numbers)
        {
            _audioSource.PlayOneShot(_audio.voices_numbers[_number]);
        }
        else
        {
            // Who goes there?!
        }
    }
}
