using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triangle : Shape
{
    public Triangle ()
    {
        edges = 3;
        vertices = 3;
        colors = new ShapeColors[edges];
        colors[0] = Shape.ShapeColors.White;
        colors[1] = Shape.ShapeColors.White;
        colors[2] = Shape.ShapeColors.White;
        sizes = new int[] { 1, 1, 1 };
        speed = 1;
        rotSpeed = 0;
        acceleration = 1;
    }
}
