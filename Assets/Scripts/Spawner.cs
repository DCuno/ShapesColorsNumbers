using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public List<GameObject> shapesList = new List<GameObject>();
    public int count = 0;
    [Range(0, 100)]
    public int iterations;
    [Range(0.0f, 1.0f)]
    public float sizeSlider;
    public GameObject shape;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("Spawn");
    }

    IEnumerator Spawn()
    {
        for (int i = 0; i < iterations; i++)
        {
            yield return new WaitForSeconds(0.05f);
            shapesList.Add(Instantiate(shape, this.gameObject.transform.position, Quaternion.identity, this.gameObject.transform));
            shapesList[i].GetComponent<Polygon>().Creation(RandomShape(), RandomColor(), sizeSlider);
            yield return new WaitForSeconds(0.08f);
        }
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
        
    }
}
