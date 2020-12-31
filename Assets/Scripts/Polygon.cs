using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Polygon : MonoBehaviour
{


    private Rigidbody2D _rigidbody2D;
    private PolygonCollider2D _polyCollider2D;
    private LineRenderer _lineRenderer;
    private MeshFilter _meshFilter;
    private MeshRenderer _meshRenderer;
    private AudioSource _audioSource;
    public bool solid = false;

    private float initVMin = 5.0f;
    private float initVMax = 8.0f;
    private float pushVMin = 4.0f;
    private float pushVMax = 6.0f;
    private float pushAngV = 250.0f;
    private float slowestV = 0.03f;
    private float relV;

    private AudioClip pop1;
    private AudioClip pop2;
    private AudioClip pop3;

    // Start and end vertices (in absolute coordinates)
    private readonly List<Vector2> _vertices = new List<Vector2>();

    public enum Shape { Triangle, Square, Pentagon, Hexagon, Star, Circle }
    public Vector2[] vertices;
    public Color color;
    public int[] sizes;
    public Vector2 speed;
    public Vector2 rotSpeed;

    public void Creation(Shape s, Color c, float size)
    {
        relV = (size - 1.0f) == 0 ? 1 : Mathf.Abs(size - 1.0f); // Change velocity relative to the size of the shapes

        switch(s)
        {
            case Shape.Triangle:
                vertices = new Vector2[] {
                    new Vector2(0,0)*size,
                    new Vector2(0,2.75f)*size,
                    new Vector2(2.75f,2.75f)*size,
                };
                color = c;
                break;
            case Shape.Square:
                vertices = new Vector2[] {
                    new Vector2(0,0)*size,
                    new Vector2(0,2.5f)*size,
                    new Vector2(2.5f,2.5f)*size,
                    new Vector2(2.5f,0)*size,
                };
                color = c;
                break;
            case Shape.Pentagon:
                vertices = new Vector2[] { // size 2 pentagon
                    new Vector2(-1f, 0)*size,
                    new Vector2(-1.618f, 1.902f)*size,
                    new Vector2(0, 3.077f)*size,
                    new Vector2(1.618f, 1.902f)*size,
                    new Vector2(1f, 0)*size,        
                };
                color = c;
                break;
            case Shape.Hexagon:
                color = c;
                break;
            case Shape.Star:
                color = c;
                break;
            case Shape.Circle:
                color = c;
                break;
        }

        for (int i = 0; i < vertices.Length; i++)
        {
            AddVertex(vertices[i]);
        }

        // Fix pointy triangle ends
        if (s == Shape.Triangle)
        {
            _polyCollider2D.points = TriangleFix(_polyCollider2D.points);
            _meshFilter.mesh.vertices = ToVector3(TriangleFix(ToVector2(_meshFilter.mesh.vertices)));
        }

        // Initial x and y velocities. Randomize if the x velocity is negative or positive.
        float randomX = Random.Range(0.5f, 1f);
        float randomY = Random.Range(-initVMin * relV, -initVMax * relV);
        int randomFlip = Random.Range(0, 2);

        if (randomFlip == 0)
            _rigidbody2D.velocity = new Vector2(randomX, randomY);
        else
            _rigidbody2D.velocity = new Vector2(randomX * -1, randomY);

    }

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
        _polyCollider2D = GetComponent<PolygonCollider2D>();
        _meshFilter = GetComponent<MeshFilter>();
        _meshRenderer = GetComponent<MeshRenderer>();
        _lineRenderer = GetComponent<LineRenderer>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _audioSource = FindObjectOfType<AudioSource>();

        // lineRenderer Properties
        _lineRenderer.loop = true;
        _lineRenderer.startWidth = 0.3f;
        _lineRenderer.endWidth = 0.3f;
        _lineRenderer.numCapVertices = 10;
        _lineRenderer.numCornerVertices = 10;

        // rigidbody2D Properties
        _rigidbody2D.angularDrag = 0.2f;

        // Ignoring collisions between other shapes and the edge of the screen until entering the ShapesCollideON collider
        Physics2D.IgnoreLayerCollision(3, 3, true);
        Physics2D.IgnoreCollision(_polyCollider2D, GameObject.FindGameObjectWithTag("edge").GetComponent<EdgeCollider2D>(), true);

        // Audio
        pop1 = Resources.Load("Audio/pop1") as AudioClip;
        pop2 = Resources.Load("Audio/pop2") as AudioClip;
        pop3 = Resources.Load("Audio/pop3") as AudioClip;
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

        // Update the mesh relative to the transform
        _meshFilter.mesh = MeshGen(_vertices.ToArray(), this.color);

        // Update the shape's outline
        _lineRenderer.positionCount = _meshFilter.mesh.vertices.Length;
        _lineRenderer.SetPositions(_meshFilter.mesh.vertices);
        _lineRenderer.startColor = this.color;
        _lineRenderer.endColor = this.color;

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

    }

    // Update is called once per frame
    void Update()
    {
        // Push slow shapes to speed them back up
        if (_rigidbody2D.velocity.x <= slowestV * relV && _rigidbody2D.velocity.x >= -slowestV * relV || _rigidbody2D.velocity.y <= slowestV * relV && _rigidbody2D.velocity.y >= -slowestV * relV)
        {
            float randomAngularVelocity = Random.Range(-pushAngV * relV, pushAngV * relV);
            Vector2 randomVelocity = new Vector2(Random.Range(pushVMin * relV, pushVMax * relV), Random.Range(pushVMin * relV, pushVMax * relV));

            _rigidbody2D.velocity = randomVelocity;
            _rigidbody2D.angularVelocity = randomAngularVelocity;
            Debug.Log("pushing " + this.name + " - angular velocity: " + randomAngularVelocity + " velocity: " + randomVelocity);
        }
    }

    // Tap pop shapes!
    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            int x = Random.Range(0, 2);

            switch (x)
            {
                case 0:
                    _audioSource.PlayOneShot(pop1);
                    break;
                case 1:
                    _audioSource.PlayOneShot(pop2);
                    break;
                case 2:
                    _audioSource.PlayOneShot(pop3);
                    break;
            }


            print("POP");
            Destroy(this.gameObject);
        }
    }

    // Fix triangle pointy ends
    public static Vector2[] TriangleFix(Vector2[] v)
    {
        Vector2[] temp = new Vector2[v.Length];
        for (int i = 0; i < v.Length; i++)
        {
            if (i == 0) // move first vertice's y up 0.1
            {
                temp[i] = v[i];
                temp[i].y += 0.1f;
                continue;
            }

            if (i == 2) // move last vertice's x down 0.1
            {
                temp[i] = v[i];
                temp[i].x -= 0.1f;
                continue;
            }

            temp[i] = v[i];
        }

        return temp;
    }

    // Extension that converts an array of Vector2 to an array of Vector3
    public static Vector3[] ToVector3(Vector2[] vectors)
    {
        return System.Array.ConvertAll<Vector2, Vector3>(vectors, v => v);
    }

    // Extension that converts an array of Vector3 to an array of Vector2
    public static Vector2[] ToVector2(Vector3[] vectors)
    {
        return System.Array.ConvertAll<Vector3, Vector2>(vectors, v => v);
    }

    /// Extension that, given a collection of vectors, returns a centroid 
    /// (i.e., an average of all vectors) 
    public static Vector2 Centroid(ICollection<Vector2> vectors)
    {
        return vectors.Aggregate((agg, next) => agg + next) / vectors.Count();
    }

    /// Extension returning the absolute value of a vector
    public static Vector2 Abs(Vector2 vector)
    {
        return new Vector2(Mathf.Abs(vector.x), Mathf.Abs(vector.y));
    }
}


