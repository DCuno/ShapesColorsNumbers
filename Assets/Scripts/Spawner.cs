using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    List<GameObject> shapesList = new List<GameObject>();
    public GameObject shape;

    // Start is called before the first frame update
    void Start()
    {
        shapesList.Add(Instantiate(shape, this.gameObject.transform.position, Quaternion.identity, this.gameObject.transform));
        shapesList.Add(Instantiate(shape, this.gameObject.transform.position, Quaternion.identity, this.gameObject.transform));
        shapesList.Add(Instantiate(shape, this.gameObject.transform.position, Quaternion.identity, this.gameObject.transform));
        shapesList.Add(Instantiate(shape, this.gameObject.transform.position, Quaternion.identity, this.gameObject.transform));
        shapesList.Add(Instantiate(shape, this.gameObject.transform.position, Quaternion.identity, this.gameObject.transform));
        shapesList.Add(Instantiate(shape, this.gameObject.transform.position, Quaternion.identity, this.gameObject.transform));
        shapesList.Add(Instantiate(shape, this.gameObject.transform.position, Quaternion.identity, this.gameObject.transform));
        shapesList.Add(Instantiate(shape, this.gameObject.transform.position, Quaternion.identity, this.gameObject.transform));
        shapesList.Add(Instantiate(shape, this.gameObject.transform.position, Quaternion.identity, this.gameObject.transform));

        foreach (GameObject g in shapesList)
        {
            g.GetComponent<Polygon>().Creation(RandomShape(), RandomColor());
        }
    }

    Color RandomColor()
    {
        int i = Random.Range(0, 6);

        switch(i)
        {
            case 0:
                return Color.red;
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
        int i = Random.Range(0, 3); // increase when we make new shapes

        switch (i)
        {
            case 0:
                return Polygon.Shape.Triangle;
            case 1:
                return Polygon.Shape.Square;
            case 2:
                return Polygon.Shape.Pentagon;
            /*case 3:
                return Color.blue;
            case 4:
                return new Color(255, 165, 0); // Orange
            case 5:
                return Color.yellow;*/
            default:
                return Polygon.Shape.Triangle;
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
