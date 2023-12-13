using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Image>().alphaHitTestMinimumThreshold = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
