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
    public List<SpawnerSettings> SpawnerSettings;
    private int spawnerSettingsIndex;
    private static AudioSource _SFXSource;
    private static AudioSource _MusicSource;
    private static Audio _SFXAudio;
    private static Audio _MusicAudio;

    private void Awake()
    {
        
    }

    void Start()
    {
        ShapesList = new List<GameObject>(6);
        ColorsList = new List<GameObject>(6);
        NumbersList = new List<GameObject>(10);
        spawner = GameObject.FindGameObjectWithTag("spawner");

        _SFXSource = GameObject.FindGameObjectWithTag("SFXSource").GetComponent<AudioSource>();
        _SFXAudio = _SFXSource.GetComponent<Audio>();

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
        List<Spawner.Shape> _allShapes = new List<Spawner.Shape> { Spawner.Shape.Triangle, Spawner.Shape.Square, Spawner.Shape.Pentagon, Spawner.Shape.Hexagon, Spawner.Shape.Circle, Spawner.Shape.Star };
        List<Spawner.Colors> _allColors = new List<Spawner.Colors> { Spawner.Colors.Red, Spawner.Colors.Orange, Spawner.Colors.Yellow, Spawner.Colors.Green, Spawner.Colors.Blue, Spawner.Colors.Purple };
        string curChapter = PlayerPrefs.GetString("Chapter");

        int idx = 0;
        spawnerSettingsIndex = 0;
        if (curChapter.Equals("All") || curChapter.Equals("Shapes"))
        {
            foreach (GameObject i in ShapesList)
            {
                LessonsManager.VoiceSound(Spawner.Topics.Shapes, (Spawner.Shape)idx, (Spawner.Colors)0, 0);
                i.gameObject.SetActive(true);
                yield return new WaitForSeconds(3);
                i.gameObject.SetActive(false);

                //spawner.GetComponent<Spawner>().SettingsSetup(new List<Spawner.Shape> { (Spawner.Shape)idx }, new List<Spawner.Colors> { Spawner.Colors.White }, 5, 1, true, false, Spawner.Topics.Shapes, Spawner.Topics.Shapes);
                //spawner.GetComponent<Spawner>().Started = true;
                spawner.GetComponent<Spawner>().SettingsSetup(SpawnerSettings[spawnerSettingsIndex++]);

                yield return new WaitWhile(() => spawner.GetComponent<Spawner>().Started);

                //spawner.GetComponent<Spawner>().SettingsSetup(new List<Spawner.Shape> { (Spawner.Shape)idx }, new List<Spawner.Colors> { Spawner.Colors.White }, 5, 7, true, false, Spawner.Topics.Shapes, Spawner.Topics.Shapes);
                //spawner.GetComponent<Spawner>().Started = true;
                spawner.GetComponent<Spawner>().SettingsSetup(SpawnerSettings[spawnerSettingsIndex++]);

                yield return new WaitWhile(() => spawner.GetComponent<Spawner>().Started);

                idx++;
            }

            if (curChapter.Equals("Shapes"))
                yield return null;
        }

        idx = 0;
        //spawnerSettingsIndex = 0;
        if (curChapter.Equals("All") || curChapter.Equals("Colors"))
        {
            foreach (GameObject i in ColorsList)
            {
                LessonsManager.VoiceSound(Spawner.Topics.Colors, (Spawner.Shape)0, (Spawner.Colors)idx, 0);
                i.gameObject.SetActive(true);
                yield return new WaitForSeconds(3);
                i.gameObject.SetActive(false);

                spawner.GetComponent<Spawner>().SettingsSetup(_allShapes, new List<Spawner.Colors> { (Spawner.Colors)idx }, 5, 1, true, false, Spawner.Topics.Colors, Spawner.Topics.Colors);
                spawner.GetComponent<Spawner>().Started = true;

                yield return new WaitWhile(() => spawner.GetComponent<Spawner>().Started);

                spawner.GetComponent<Spawner>().SettingsSetup(_allShapes, new List<Spawner.Colors> { (Spawner.Colors)idx }, 5, 7, true, false, Spawner.Topics.Colors, Spawner.Topics.Colors);
                spawner.GetComponent<Spawner>().Started = true;

                yield return new WaitWhile(() => spawner.GetComponent<Spawner>().Started);
                idx++;
            }

            if (curChapter.Equals("Colors"))
                yield return null;
        }

        idx = 0;
        if (curChapter.Equals("All") || curChapter.Equals("Numbers"))
        {
            foreach (GameObject i in NumbersList)
            {
                LessonsManager.VoiceSound(Spawner.Topics.Numbers, (Spawner.Shape)0, (Spawner.Colors)0, idx);
                i.gameObject.SetActive(true);
                yield return new WaitForSeconds(3);
                i.gameObject.SetActive(false);

                spawner.GetComponent<Spawner>().SettingsSetup(_allShapes, _allColors, 5, idx + 1, true, false, Spawner.Topics.Numbers, Spawner.Topics.Numbers);
                spawner.GetComponent<Spawner>().Started = true;

                yield return new WaitWhile(() => spawner.GetComponent<Spawner>().Started);
                idx++;
            }
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

    private static void VoiceSound(Spawner.Topics voice, Spawner.Shape shape, Spawner.Colors color, int number)
    {
        if (voice == Spawner.Topics.Shapes)
        {
            _SFXSource.PlayOneShot(_SFXAudio.voices_shapes[(int)shape]);
        }
        else if (voice == Spawner.Topics.Colors)
        {
            _SFXSource.PlayOneShot(_SFXAudio.voices_colors[(int)color]);
        }
        else if (voice == Spawner.Topics.Numbers)
        {
            _SFXSource.PlayOneShot(_SFXAudio.voices_numbers[number]);
        }
        else
        {
            // Who goes there?!
        }
    }
}
