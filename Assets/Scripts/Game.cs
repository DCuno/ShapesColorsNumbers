using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Game : MonoBehaviour
{


    private Rigidbody2D _rigidbody2D;
    private BoxCollider2D _boxCollider2D;
    private LineRenderer _lineRenderer;
    private MeshFilter _meshFilter;
    
    private bool _simulating;
    public bool SimulatingPhysics
    {
        get { return _simulating; }
        set
        {
            _simulating = value;
            _rigidbody2D.bodyType = value ?
                    RigidbodyType2D.Dynamic : RigidbodyType2D.Static;
        }
    }

    private void Awake()
    {
        _meshFilter = GetComponent<MeshFilter>();
        _lineRenderer = GetComponent<LineRenderer>();
        _boxCollider2D = GetComponent<BoxCollider2D>();
        _lineRenderer = GetComponent<LineRenderer>();

        _rigidbody2D = GetComponent<Rigidbody2D>();
        _rigidbody2D.useAutoMass = true;
    }

    public Color FillColor = Color.white;

    // Start and end vertices (in absolute coordinates)
    private readonly List<Vector2> _vertices = new List<Vector2>(2);

    public bool ShapeFinished { get { return _vertices.Count >= 2; } }

    public void AddVertex(Vector2 vertex)
    {
        if (ShapeFinished)
        {
            return;
        }

        _vertices.Add(vertex);
        UpdateShape(vertex);
    }

    public void UpdateShape(Vector2 newVertex)
    {
        if (_vertices.Count < 2)
        {
            return;
        }

        _vertices[_vertices.Count - 1] = newVertex;

        // Set the gameobject's position to be the center of mass
        var center = Centroid(_vertices);
        transform.position = center;

        // Update the mesh relative to the transform
        var relativeVertices = _vertices.Select(v => v - center).ToArray();
        _meshFilter.mesh = RectangleMesh(relativeVertices[0], relativeVertices[1], FillColor);

        // Update the shape's outline
        _lineRenderer.positionCount = _meshFilter.mesh.vertices.Length;
        _lineRenderer.SetPositions(_meshFilter.mesh.vertices);

        // Update the collider
        var dimensions = Abs((_vertices[1] - _vertices[0]));
        _boxCollider2D.size = dimensions;
    }

    /// <summary>
    /// Creates and returns a rectangle mesh given two vertices on its 
    /// opposite corners and fills it with the given color. 
    /// </summary>
    private static Mesh RectangleMesh(Vector2 v0, Vector2 v1, Color fillColor)
    {
        // Calculate implied verticies from corner vertices
        // Note: vertices must be adjacent to each other for Triangulator to work properly
        var v2 = new Vector2(v0.x, v1.y);
        var v3 = new Vector2(v1.x, v0.y);
        var rectangleVertices = new[] { v0, v2, v1, v3 };

        // Find all the triangles in the shape
        var triangles = new Triangulator(rectangleVertices).Triangulate();

        // Assign each vertex the fill color
        var colors = Enumerable.Repeat(fillColor, rectangleVertices.Length).ToArray();

        var mesh = new Mesh
        {
            name = "Rectangle",
            vertices = ToVector3(rectangleVertices), 
            triangles = triangles,
            colors = colors
        };

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();

        return mesh;
    }
    // Start is called before the first frame update
    void Start()
    {
        // Create Vector2 vertices
        var vertices2D = new Vector2[] {
            new Vector2(0,0),
            new Vector2(0,1),
            new Vector2(1,1),
        };

        var vertices3D = System.Array.ConvertAll<Vector2, Vector3>(vertices2D, v => v);

        // Use the triangulator to get indices for creating triangles
        var triangulator = new Triangulator(vertices2D);
        var indices = triangulator.Triangulate();

        // Generate a color for each vertex
        Color[] colors = new Color[3];
        colors[0] = Color.white;
        colors[1] = Color.white;
        colors[2] = Color.white;
            //Enumerable.Range(0, vertices3D.Length).Select(i => Random.ColorHSV()).ToArray();

        // Create the mesh
        var mesh = new Mesh
        {
            vertices = vertices3D,
            triangles = indices,
            colors = colors
        };

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        // Set up game object with mesh;
        var meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = new Material(Shader.Find("Sprites/Default"));

        var filter = gameObject.AddComponent<MeshFilter>();
        filter.mesh = mesh;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Extension that converts an array of Vector2 to an array of Vector3
    /// </summary>
    public static Vector3[] ToVector3(Vector2[] vectors)
    {
        return System.Array.ConvertAll<Vector2, Vector3>(vectors, v => v);
    }

    /// <summary>
    /// Extension that, given a collection of vectors, returns a centroid 
    /// (i.e., an average of all vectors) 
    /// </summary>
    public static Vector2 Centroid(ICollection<Vector2> vectors)
    {
        return vectors.Aggregate((agg, next) => agg + next) / vectors.Count();
    }

    /// <summary>
    /// Extension returning the absolute value of a vector
    /// </summary>
    public static Vector2 Abs(Vector2 vector)
    {
        return new Vector2(Mathf.Abs(vector.x), Mathf.Abs(vector.y));
    }
}