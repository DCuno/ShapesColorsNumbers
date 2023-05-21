using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// Start() code by Tabu from https://www.nuf.dk/making-unity-colliders-follow-the-screensize/

public class ScreenEdge : MonoBehaviour
{
    public float colThickness = 2f;
    public float zPosition = 0f;
    private Vector2 screenSize;
    public PhysicsMaterial2D physicsMaterial;
    void Start()
    {
        System.Collections.Generic.Dictionary<string, Transform> colliders = new System.Collections.Generic.Dictionary<string, Transform>();
        colliders.Add("Top", new GameObject().transform);
        colliders.Add("Bottom", new GameObject().transform);
        colliders.Add("Right", new GameObject().transform);
        colliders.Add("Left", new GameObject().transform);
        colliders.Add("TopTrigger", new GameObject().transform);
        colliders.Add("BottomTrigger", new GameObject().transform);
        colliders.Add("RightTrigger", new GameObject().transform);
        colliders.Add("LeftTrigger", new GameObject().transform);

        Vector3 cameraPos = Camera.main.transform.position;

        //Grab the world-space position values of the start and end positions of the screen, 
        //then calculate the distance between them and store it as half, since we only need 
        //half that value for distance away from the camera to the edge
        screenSize.x = Vector2.Distance(Camera.main.ScreenToWorldPoint(new Vector2(0, 0)), Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0))) * 0.5f;
        screenSize.y = Vector2.Distance(Camera.main.ScreenToWorldPoint(new Vector2(0, 0)), Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height))) * 0.5f;

        foreach (KeyValuePair<string, Transform> valPair in colliders)
        {
            valPair.Value.gameObject.AddComponent<BoxCollider2D>();
            //Set the object's name to it's "Key" name, and take on "Collider".  i.e: TopCollider
            valPair.Value.name = valPair.Key + "Collider";
            //Make the object a child of whatever object this script is on (preferably the camera)
            valPair.Value.parent = transform;
            //Scale the object to the width and height of the screen, using the world-space values calculated earlier
            if (valPair.Key == "Left" || valPair.Key == "Right" || valPair.Key == "LeftTrigger" || valPair.Key == "RightTrigger")
                valPair.Value.localScale = new Vector3(colThickness, screenSize.y * 2, colThickness);
            else
                valPair.Value.localScale = new Vector3(screenSize.x * 2, colThickness, colThickness);
            //We add the Physicsmaterial to the collider here if there is any
            //Remove the 2D from BoxCollider2D if you are working with 3D
            if (physicsMaterial)
            {
                valPair.Value.gameObject.GetComponent<BoxCollider2D>().sharedMaterial = physicsMaterial;
            }

            valPair.Value.gameObject.tag = "edge";

            if (valPair.Key == "LeftTrigger" || valPair.Key == "RightTrigger" || valPair.Key == "TopTrigger" || valPair.Key == "BottomTrigger")
            {
                valPair.Value.gameObject.AddComponent<ScreenEdgeChild>();
                valPair.Value.gameObject.GetComponent<BoxCollider2D>().isTrigger = true;
            }
        }
        //Change positions to align perfectly with outter-edge of screen, 
        //adding the world-space values of the screen we generated earlier, 
        //and adding/subtracting them with the current camera position, 
        //as well as add/subtracting half our objects size so it's not just half way off-screen
        colliders["Right"].position = new Vector3(cameraPos.x + screenSize.x + (colliders["Right"].localScale.x * 0.5f), cameraPos.y, zPosition);
        colliders["Left"].position = new Vector3(cameraPos.x - screenSize.x - (colliders["Left"].localScale.x * 0.5f), cameraPos.y, zPosition);
        colliders["Top"].position = new Vector3(cameraPos.x, cameraPos.y + screenSize.y + (colliders["Top"].localScale.y * 0.5f), zPosition);
        colliders["Bottom"].position = new Vector3(cameraPos.x, cameraPos.y - screenSize.y - (colliders["Bottom"].localScale.y * 0.5f), zPosition);
        colliders["RightTrigger"].position = new Vector3(cameraPos.x + screenSize.x + (colliders["Right"].localScale.x * 0.5f), cameraPos.y, zPosition);
        colliders["LeftTrigger"].position = new Vector3(cameraPos.x - screenSize.x - (colliders["Left"].localScale.x * 0.5f), cameraPos.y, zPosition);
        colliders["TopTrigger"].position = new Vector3(cameraPos.x, cameraPos.y + screenSize.y + (colliders["Top"].localScale.y * 0.5f), zPosition);
        colliders["BottomTrigger"].position = new Vector3(cameraPos.x, cameraPos.y - screenSize.y - (colliders["Bottom"].localScale.y * 0.5f), zPosition);
    }
}
