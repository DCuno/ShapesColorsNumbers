using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Number : MonoBehaviour
{
    //adjust this to change speed
    public float speed = 5f;
    //adjust this to change how high it goes
    public float rotationAmount = 0.25f;
    // adjust this to change when the number begins to fade away
    public float fadeTime = 2f;
    // adjust this to change the rate at which the number fades away
    public float fadeRate = 1f;

    private Quaternion rotation;
    private Color numberColor;
    private float fadeAmount;
    private float fadeTimer = 0f;

    private void Start()
    {
        rotation = transform.rotation;
        numberColor = this.GetComponent<TextMeshPro>().color;
    }
    void Update()
    {
        fadeTimer += Time.deltaTime;

        if (fadeTimer >= fadeTime)
        {
            fadeAmount = numberColor.a - (2f * Time.deltaTime);
            numberColor = new Color(numberColor.r, numberColor.g, numberColor.b, fadeAmount);
            this.GetComponent<TextMeshPro>().color = numberColor;

            if (fadeAmount <= 0)
                Destroy(this.gameObject);
        }

        //calculate what the new z position will be
        float newZ = Mathf.Sin(Time.time * speed) * rotationAmount + rotation.z;
        //set the object's z to the new calculated z
        transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.y, newZ, transform.rotation.w);
    }
}
