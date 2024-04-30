using UnityEngine;
using System.Collections;
using TMPro;

public class Fps : MonoBehaviour
{
    private float count;
    Animator _anim;
    GameBackButton _gbb;
    TextMeshProUGUI tmp;
    private IEnumerator Start()
    {
        tmp = gameObject.GetComponent<TextMeshProUGUI>();
        GUI.depth = 2;
        while (true)
        {
            count = 1f / Time.unscaledDeltaTime;
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void OnGUI()
    {
        tmp.text = "FPS:" + Mathf.Round(count) + "\n";
        
        if (_anim != null)
        {
            tmp.text += "Press Bool: " + _anim.GetBool("Press 0") + "\n";
        }
        else
        {
            _anim = GameObject.FindGameObjectWithTag("GameBackButton").GetComponent<Animator>();
            tmp.text += "Press Bool: " + _anim.GetBool("Press 0") + "\n";
        }
/*
        if (_gbb != null)
        {
            tmp.text += "Hit Collider: " + _gbb.hit.collider + "\n";
        }
        else
        {
            _gbb = GameObject.FindGameObjectWithTag("GameBackButton").GetComponent<GameBackButton>();
            tmp.text += "Hit Collider: " + _gbb.hit.collider + "\n";
        }*/
    }
}
