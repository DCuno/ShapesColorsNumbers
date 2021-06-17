using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LessonsManager : MonoBehaviour
{
    List<GameObject> ShapesList;
    List<GameObject> ColorsList;
    List<GameObject> NumbersList;
    void Start()
    {
        ShapesList = new List<GameObject>(6);
        ColorsList = new List<GameObject>(6);
        NumbersList = new List<GameObject>(10);

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

        StartCoroutine(TitleShowTest());
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
}
