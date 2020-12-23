using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Game : MonoBehaviour
{


    private Rigidbody2D _rigidbody2D;
    private PolygonCollider2D _polyCollider2D;
    private LineRenderer _lineRenderer;
    private MeshFilter _meshFilter;
    private MeshRenderer _meshRenderer;

    private float velNeg = -0.02f;
    private float velPos = 0.02f;

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
        _meshRenderer = GetComponent<MeshRenderer>();
        _lineRenderer = GetComponent<LineRenderer>();
        _polyCollider2D = GetComponent<PolygonCollider2D>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _lineRenderer.loop = true;
        _lineRenderer.startWidth = 0.4f;
        _lineRenderer.endWidth = 0.4f;
        _lineRenderer.numCapVertices = 10;
        _lineRenderer.numCornerVertices = 10;
        //_rigidbody2D.useAutoMass = true;                // Mass is automatic
    }

    public Color FillColor = Color.white;

    // Start and end vertices (in absolute coordinates)
    private readonly List<Vector2> _vertices = new List<Vector2>(3);

    public bool ShapeFinished { get { return _vertices.Count >= 3; } }

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
        if (_vertices.Count < 3)
        {
            return;
        }

        _vertices[_vertices.Count - 1] = newVertex;

        // Set the gameobject's position to be the center of mass
        var center = Centroid(_vertices);
        transform.position = center;

        // Update the mesh relative to the transform
        var relativeVertices = _vertices.ToArray();
        _meshFilter.mesh = TriangleMesh(relativeVertices[0], relativeVertices[1], relativeVertices[2], FillColor);

        // Update the shape's outline
        _lineRenderer.positionCount = _meshFilter.mesh.vertices.Length;
        _lineRenderer.SetPositions(_meshFilter.mesh.vertices);

        // Update the collider
        Vector2[] vector2vertices = ToVector2(_meshFilter.mesh.vertices);
        _polyCollider2D.points = vector2vertices;
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

    /// <summary>
    /// Creates and returns a triangle mesh given three vertices 
    /// and fills it with the given color. 
    /// </summary>
    private static Mesh TriangleMesh(Vector2 v0, Vector2 v1, Vector2 v2, Color fillColor)
    {
        var triangleVertices = new[] { v0, v1, v2 };

        // Find all the triangles in the shape
        var triangles = new Triangulator(triangleVertices).Triangulate();

        // Assign each vertex the fill color
        var colors = Enumerable.Repeat(fillColor, triangleVertices.Length).ToArray();

        var mesh = new Mesh
        {
            name = "Triangle",
            vertices = ToVector3(triangleVertices),
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
            new Vector2(0,2),
            new Vector2(2,2),
        };

        AddVertex(vertices2D[0]);
        AddVertex(vertices2D[1]);
        AddVertex(vertices2D[2]);

        // Collider
        _polyCollider2D.points = vertices2D;

        _rigidbody2D.velocity = new Vector2(2,2);
    }

    // Update is called once per frame
    void Update()
    {
        if (_rigidbody2D.velocity.x <= velPos && _rigidbody2D.velocity.x >= velNeg || _rigidbody2D.velocity.y <= velPos && _rigidbody2D.velocity.y >= velNeg)
        {
            float randomAngularVelocity = Random.Range(-100f, 100f);
            Vector2 randomVelocity = new Vector2(Random.Range(0.5f, 2.0f), Random.Range(0.5f, 2.0f));

            _rigidbody2D.velocity = randomVelocity;
            _rigidbody2D.angularVelocity = randomAngularVelocity;
            Debug.Log("pushing " + this.name + " - angular velocity: " + randomAngularVelocity + " velocity: " + randomVelocity);
        }
    }

    /// <summary>
    /// Extension that converts an array of Vector2 to an array of Vector3
    /// </summary>
    public static Vector3[] ToVector3(Vector2[] vectors)
    {
        return System.Array.ConvertAll<Vector2, Vector3>(vectors, v => v);
    }

    /// <summary>
    /// Extension that converts an array of Vector3 to an array of Vector2
    /// </summary>
    public static Vector2[] ToVector2(Vector3[] vectors)
    {
        return System.Array.ConvertAll<Vector3, Vector2>(vectors, v => v);
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