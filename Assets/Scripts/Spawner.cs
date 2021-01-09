using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Spawner : MonoBehaviour
{
    private GameObject canvas;

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

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("Spawn");
    }

    IEnumerator Spawn()
    {
        // GET CANVAS VARIABLES HERE

        for (int i = 0; i < iterations; i++)
        {
            yield return new WaitForSeconds(0.05f);
            shapesList.Add(Instantiate(shape, this.gameObject.transform.position, Quaternion.identity, this.gameObject.transform));
            shapesList[i].GetComponent<Polygon>().Creation(RandomShape(), RandomColor(), sizeSlider, true);
            yield return new WaitForSeconds(0.08f);
        }
    }

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

    Color RandomColor()
    {
        int i = Random.Range(0, 6);

        switch(i)
        {
            case 0:
                return Color.red;//return Color.Lerp(Color.red, Color.black, 0.2f); // Darkens red by 20%
            case 1:
                return Color.green;
            case 2:
                return Color.blue;
            case 3:
                return Color.yellow;
            case 4:
                return new Color(1.0f, 0.64f, 0.0f); // Orange
            case 5:
                return new Color(0.5f, 0f, 0.5f); // Purple
            case 6:
                return Color.white;
            default:
                return Color.white;
        }
    }

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
    
    // Update is called once per frame
    void Update()
    {
        finishedCheck += Time.deltaTime;

        // After 1 second of game running time, start checking if all the shapes are gone.
        if (finishedCheck >= 1.0f)
        {
            // Shapes have been popped, pull down menu. (BACK BUTTON FOR NOW)
            if (this.transform.childCount == 0)
            {
                finished = true;
            }
        }
    }
}
