using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Number : MonoBehaviour
{
    //adjust this to change rotation speed
    public float speed = 5f;
    //adjust this to change how high it rotates
    public float rotationAmount = 0.1f;
    // adjust this to change when the number begins to fade away
    public float FadeTime = 2f;
    // adjust this to change the rate at which the number fades away
    public float FadeRate = 2f;

    private Quaternion rotation;
    private Color numberColor;
    private float fadeAmount;
    private float fadeTimer = 0f;
    static private Queue<Number> numberList = new Queue<Number>();
    private int _fadeCount = 0;

    private void Start()
    {
        rotation = transform.rotation;
        numberColor = this.GetComponent<TextMeshPro>().color;
        numberList.Enqueue(this);
    }
    void Update()
    {
        if (numberList.Count > 1)
            Pop();

        fadeTimer += Time.deltaTime;

        if (fadeTimer >= FadeTime)
        {
            fadeAmount = numberColor.a - (FadeRate * Time.deltaTime);
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

    static public void Pop()
    {
        numberList.Peek().FadeRate = 4f;
        numberList.Peek().FadeTime = 0f;
        numberList.Dequeue();
    }
}
