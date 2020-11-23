using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shape : MonoBehaviour
{
    public enum ShapeColors { Red, Blue, Yellow, Purple, Green, Orange, Black, White};
    public int edges;
    public int vertices;
    public ShapeColors[] colors;
    public int[] sizes;
    public int speed;
    public int rotSpeed;
    public int acceleration;
}
