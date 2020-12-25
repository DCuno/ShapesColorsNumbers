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

    //private float velNeg = -0.04f;
    //private float velPos = 0.04f;

    public Color FillColor = Color.white;

    // Start and end vertices (in absolute coordinates)
    private readonly List<Vector2> _vertices = new List<Vector2>();

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
        _lineRenderer.startWidth = 0.2f;
        _lineRenderer.endWidth = 0.2f;
        _lineRenderer.numCapVertices = 10;
        _lineRenderer.numCornerVertices = 10;
        //_rigidbody2D.useAutoMass = true;                // Mass is automatic
    }

    public void AddVertex(Vector2 vertex)
    {
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
        _meshFilter.mesh = MeshGen(_vertices.ToArray(), FillColor);

        // Update the shape's outline
        _lineRenderer.positionCount = _meshFilter.mesh.vertices.Length;
        _lineRenderer.SetPositions(_meshFilter.mesh.vertices);

        // Update the collider
        Vector2[] vector2vertices = ToVector2(_meshFilter.mesh.vertices);
        _polyCollider2D.points = vector2vertices;
    }

    /// <summary>
    /// Creates and returns a triangle mesh given three vertices 
    /// and fills it with the given color. 
    /// </summary>
    private static Mesh MeshGen(Vector2[] v, Color fillColor)
    {
        // Find all the triangles in the shape
        var triangles = new Triangulator(v).Triangulate();

        // Assign each vertex the fill color
        var colors = Enumerable.Repeat(fillColor, v.Length).ToArray();

        var mesh = new Mesh
        {
            name = "Shape",
            vertices = ToVector3(v),
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
/*        Vector2[] vertices2D = { 0,1,2,3,4 };

        AddVertex(vertices2D[0]);
        AddVertex(vertices2D[1]);
        AddVertex(vertices2D[2]);
        AddVertex(vertices2D[3]);
        AddVertex(vertices2D[4]);

        // Collider
        _polyCollider2D.points = vertices2D;*/

        // Fix pointy triangle ends
        /*        _polyCollider2D.points = new Vector2[] {
                    new Vector2(0,0.1f),
                    new Vector2(0,2),
                    new Vector2(1.9f,2),
                };
                _meshFilter.mesh.vertices = new Vector3[] {
                    new Vector3(0,0.1f,0),
                    new Vector3(0,2,0),
                    new Vector3(1.9f,2,0),
                };*/

        //_rigidbody2D.velocity = new Vector2(Random.Range(-2.0f, 2.0f), Random.Range(1.0f, 2.5f));
    }

    // Update is called once per frame
    void Update()
    {
        // Push slow shapes to speed them back up
        /*if (_rigidbody2D.velocity.x <= velPos && _rigidbody2D.velocity.x >= velNeg || _rigidbody2D.velocity.y <= velPos && _rigidbody2D.velocity.y >= velNeg)
        {
            float randomAngularVelocity = Random.Range(-150f, 150f);
            Vector2 randomVelocity = new Vector2(Random.Range(-2.0f, 2.0f), Random.Range(-2.0f, 2.0f));

            _rigidbody2D.velocity = randomVelocity;
            _rigidbody2D.angularVelocity = randomAngularVelocity;
            Debug.Log("pushing " + this.name + " - angular velocity: " + randomAngularVelocity + " velocity: " + randomVelocity);
        }*/
    }

    // Tap pop shapes!
    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {

            print("POP");
            Destroy(this.gameObject);
        }
    }

    // Collecting all vertices of a line renderer
    public List<Vector3> GetVertices(LineRenderer line)
    {
        List<Vector3> list = new List<Vector3>();

        for(int i = 0; i < line.positionCount; i++)
        {
            list.Add(line.GetPosition(i));
            print(line.GetPosition(i));
        }

        return list;
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