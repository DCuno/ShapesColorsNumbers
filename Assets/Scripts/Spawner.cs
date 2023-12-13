using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Spawner : MonoBehaviour
{
    public enum Colors { Red, Orange, Yellow, Green, Blue, Purple, White };
    public enum Topics { Shapes, Colors, Numbers, Off };
    public enum Shape { Triangle, Square, Pentagon, Hexagon, Circle, Star };

    public GameObject settingsCanvas;
    private GameObject spawnedSettingsCanvas;

    private const int minIter = 1, maxIter = 100, minSize = 1, maxSize = 10;
    private List<GameObject> shapesList = new List<GameObject>();
    public int Count { get; set; } = 0;
    public bool DoneSpawning = false;
    [Range(minIter, maxIter)]
    private int iterations;
    [Range(minSize, maxSize)]
    private int sizeSlider; // when size is maxSize(Default: 10) on the spawner, shape size is 0.7f. when size is minSize(Default: 1), shape size is 0.1f.
    public GameObject shape;
    private bool _tilt;

    private float finishedCheck = 0f;
    private bool finished = false;
    public bool Started { get; set; } = false;
    [SerializeField]
    private SpawnerSettings SpawnerSettings;
    private SpawnerSettingsStruct currentSettings;

    public struct SpawnerSettingsStruct
    {
        public List<Shape> shapes;
        public List<Colors> colors;
        public float size;
        public float amount;
        public bool edges;
        public bool tilt;
        public Spawner.Topics topic;
        public bool voice;
        public bool text;
    }

    /*public void OnDrawGizmos()
    {
        Camera cam = Camera.main;
        //Vector2's for the corners of the screen
        Vector2 bottomLeft = cam.ScreenToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));
        Vector2 topRight = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth, cam.pixelHeight, cam.nearClipPlane));
        Vector2 topLeft = new Vector2(bottomLeft.x, topRight.y);
        Vector2 bottomRight = new Vector2(topRight.x, bottomLeft.y);
        Vector2 _screenSize;
        _screenSize.x = Vector2.Distance(Camera.main.ScreenToWorldPoint(new Vector2(0, 0)), Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0))) * 0.5f;
        _screenSize.y = Vector2.Distance(Camera.main.ScreenToWorldPoint(new Vector2(0, 0)), Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height))) * 0.5f;

        Gizmos.color = Color.green;
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topLeft, bottomLeft);
        Gizmos.DrawLine(bottomLeft, bottomRight);
        Gizmos.DrawLine(bottomRight, topRight);
        //Gizmos.DrawIcon(new Vector2(topRight.x+10, 0), "circle.png", true);
        //Gizmos.DrawIcon(new Vector2(-topRight.x-10, 0), "circle.png", true);
        //Gizmos.DrawIcon(new Vector2(topRight.x, 0), "circle.png", true);
        //Gizmos.DrawIcon(new Vector2(0, topRight.y), "circle.png", true);
        Gizmos.DrawIcon(new Vector2(_screenSize.x, 0), "circle.png", true);
        Gizmos.DrawIcon(new Vector2(0, _screenSize.y), "circle.png", true);
    }*/

    public void SettingsSetup(List<Shape> shapes, List<Colors> colors, float size, float amount, bool edges, bool tilt, Spawner.Topics topic, bool voice, bool text)
    {
        StartCoroutine(Spawn(shapes, colors, size, amount, edges, tilt, topic, voice, text));
    }

    public void SettingsSetup(SpawnerSettings settings)
    {
        if (settings == null)
            return;

        SettingsSetup(settings.Shapes, settings.Colors, settings.Size, settings.Amount, 
                        settings.Edges, settings.Tilt, settings.Topic, settings.Voice, settings.Text);
        Started = true;
    }

    IEnumerator Spawn(List<Shape> shapes, List<Colors> colors, float size, float amount, bool edges, bool tilt, Topics topic, bool voice, bool text)
    {
        currentSettings = new SpawnerSettingsStruct() { shapes = shapes, colors = colors, size = size, amount = amount, edges = edges, tilt = tilt, topic = topic, voice = voice, text = text };
        shapesList.Clear();
        Count = 0;
        DoneSpawning = false;
        finished = false;
        _tilt = tilt;
        Polygon curPolygon;
        Destroy(GameObject.FindGameObjectWithTag("SettingsCanvasScroll"));

        // Initialize normal gravity in case tilt mode was used on a previous game
        Physics2D.gravity = new Vector2(0.0f, -9.8f);

        //float spawnSpeed = SpawnAmountRatio(amount);
        for (int i = 0; i < amount; i++)
        {
            if (!finished)
            {
                yield return new WaitForSeconds(0.05f);
                shapesList.Add(Instantiate(shape, this.gameObject.transform.position, Quaternion.identity, this.gameObject.transform));
                Color _tmpColor = RandomColor(colors);
                curPolygon = shapesList[i].GetComponent<Polygon>();
                curPolygon.Creation(RandomShape(shapes), _tmpColor, UnityColorToEnumColor(_tmpColor), size, edges, tilt, topic, voice, text);
                //yield return new WaitForSeconds(0.08f);
                
                while (curPolygon != null && !curPolygon.IsInPlayArea)
                {
                    yield return null;
                }
            }
        }

        yield return new WaitForSeconds(0.5f);

        DoneSpawning = true;

        if (_tilt)
        {
            Physics2D.gravity = new Vector2(0f, 0f);
        }

        yield break;
    }

    public void GravityLimiter()
    {
        float s_maxGravity = 1.5f;

        if (Physics2D.gravity.x > s_maxGravity)
        {
            Physics2D.gravity = new Vector2(s_maxGravity, Physics2D.gravity.y);
        }
        else if (Physics2D.gravity.x < -s_maxGravity)
        {
            Physics2D.gravity = new Vector2(-s_maxGravity, Physics2D.gravity.y);
        }
        else if (Physics2D.gravity.y > s_maxGravity)
        {
            Physics2D.gravity = new Vector2(Physics2D.gravity.x, s_maxGravity);
        }
        else if (Physics2D.gravity.y < -s_maxGravity)
        {
            Physics2D.gravity = new Vector2(Physics2D.gravity.x, -s_maxGravity);
        }
        else
        {
            Physics2D.gravity = new Vector2(Input.acceleration.x * 1.5f, Input.acceleration.y * 1.5f);
        }
    }

    private float SpawnAmountRatio(float amount)
    {
        return (float) (0.08d - ((((0.08d - 0.01d) / (100d - 1d)) * (double)(amount - 1f))));
    }

    // Updates in the editor
    private void OnValidate()
    {
        if (sizeSlider >= 9 && iterations > 6)
            iterations = 6;
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
            0 => new Color(0.784f, 0.216f, 0.216f), // Red
            1 => new Color(0.0f, 0.831f, 0.0f), // Green
            2 => new Color(0f, 0.33f, 0.831f), // Blue
            3 => new Color(1.0f, 0.8f, 0.0f), // Yellow
            4 => new Color(1.0f, 0.5f, 0.0f),// Orange
            5 => new Color(0.5f, 0f, 0.5f),// Purple
            _ => Color.white,
        };
    }

    // Random Color selected from given list
    public static Color RandomColor(List<Colors> arr) =>
        arr[Random.Range(0, arr.Count)] switch
        {
            Colors.Red => new Color(0.784f, 0.216f, 0.216f), // Red
            Colors.Green => new Color(0.0f, 0.831f, 0.0f), // Green
            Colors.Blue => new Color(0f, 0.33f, 0.831f), // Blue
            Colors.Yellow => new Color(1.0f, 0.8f, 0.0f), // Yellow
            Colors.Orange => new Color(1.0f, 0.5f, 0.0f), // Orange
            Colors.Purple => new Color(0.443f, 0.216f, 0.784f), // Purple
            Colors.White => Color.white,
            _ => Color.white,
        };

    // Old Orange
    // new Color(1.0f, 0.4f, 0.0f)

    // Random Shape returned out of all shapes
    Shape RandomShape()
    {
        int i = Random.Range(0, 6);

        switch (i)
        {
            case 0:
                return Shape.Triangle;
            case 1:
                return Shape.Square;
            case 2:
                return Shape.Pentagon;
            case 3:
                return Shape.Hexagon;
            case 4:
                return Shape.Circle;
            case 5:
                return Shape.Star;
            default:
                return Shape.Circle;
        }
    }

    // Random Shape selected from given array
    Shape RandomShape(List<Shape> arr)
    {
        return arr[Random.Range(0, arr.Count)];
    }

    private Spawner.Colors UnityColorToEnumColor(Color color)
    {
        if (color == new Color(0.784f, 0.216f, 0.216f))
            return Colors.Red;
        else if (color == new Color(0.0f, 0.831f, 0.0f))
            return Colors.Green;
        else if (color == new Color(0f, 0.33f, 0.831f))
            return Colors.Blue;
        else if (color == new Color(1.0f, 0.8f, 0.0f))
            return Colors.Yellow;
        else if (color == new Color(1.0f, 0.5f, 0.0f))
            return Colors.Orange;
        else if (color == new Color(0.443f, 0.216f, 0.784f))
            return Colors.Purple;
        else // White
            return Colors.White;
    }

    // Used in Polygon to color pop text
    public static Color EnumColortoUnityColor(Spawner.Colors color)
    {
        if (color == Colors.Red)
            return new Color(0.784f, 0.216f, 0.216f);
        else if (color == Colors.Green)
            return new Color(0.0f, 0.831f, 0.0f);
        else if (color == Colors.Blue)
            return new Color(0f, 0.33f, 0.831f);
        else if (color == Colors.Yellow)
            return new Color(1.0f, 0.8f, 0.0f);
        else if (color == Colors.Orange)
            return new Color(1.0f, 0.5f, 0.0f);
        else if (color == Colors.Purple)
            return new Color(0.443f, 0.216f, 0.784f);
        else // White
            return Color.white;
    }

    // Update is called once per frame
    void Update()
    {
        if (Started)
        {
            finishedCheck += Time.deltaTime;

            if (SceneManager.GetActiveScene().name == "FunModeGameScene2")
            {
                if (DoneSpawning)
                {
                    // Shapes have been popped, pull down menu.
                    if (this.transform.childCount == 0)
                    {
                        finished = true;
                        Started = false;
                        finishedCheck = 0f;
                        spawnedSettingsCanvas = Instantiate(settingsCanvas);
                        spawnedSettingsCanvas.GetComponentInChildren<FunModeButtonManager>().FunModeButtonManagerConstructor(currentSettings);
                    }
                }
            }
            else if (SceneManager.GetActiveScene().name == "LessonsScene")
            {
                if (DoneSpawning)
                {
                    // Shapes have been popped, pull down menu.
                    if (this.transform.childCount == 0)
                    {
                        finished = true;
                        Started = false;
                        finishedCheck = 0f;
                    }
                }
            }

            // Check if Back was pressed this frame
            if (Input.GetKeyDown(KeyCode.Escape) && SceneManager.GetActiveScene().name == "FunModeGameScene2")
            {
                ResetFunMode();
            }
            else if (Input.GetKeyDown(KeyCode.Escape) && SceneManager.GetActiveScene().name == "LessonsScene")
            {
                LeaveLessons();
            }
        }

        if (_tilt && DoneSpawning)
            GravityLimiter();
    }

    private void DeleteAllChildren()
    {
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    private void ResetFunMode()
    {
        finished = true;
        DeleteAllChildren();
        spawnedSettingsCanvas.GetComponentInChildren<FunModeButtonManager>().FunModeButtonManagerConstructor(currentSettings);
    }

    private void LeaveLessons()
    {
        finished = true;
        DeleteAllChildren();
        SceneManager.LoadScene(sceneName: "TitleScene");
    }
}
