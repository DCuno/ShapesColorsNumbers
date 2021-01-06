using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class Polygon : MonoBehaviour
{

    private Rigidbody2D _rigidbody2D;
    private PolygonCollider2D _polyCollider2D;
    private SpriteRenderer _spriteRenderer;
    private AudioSource _audioSource;
    public GameObject popParticles;
    public GameObject countNumber;
    public bool solid = false;

    private readonly float initYV = 9.0f;
    private readonly float initXVMin = 0.06f;
    private readonly float initXVMax = 2.0f;
    private readonly float initAngV = 300.0f;
    private readonly float pushVMin = 5.0f;
    private readonly float pushVMax = 8.0f;
    private readonly float pushAngV = 300.0f;
    private readonly float slowestV = 0.05f;
    private readonly float smallestSizeSlider = 1;
    private readonly float largestSizeSlider = 10;
    private readonly float smallestRealSize = 0.1f;
    private readonly float largestRealSize = 0.7f;
    private float normV;
    private float mapSize;

    private AudioClip[] pops = new AudioClip[3];

    private List<Vector2> points = new List<Vector2>();
    private List<Vector2> simplifiedPoints = new List<Vector2>();

    public enum Shape { Triangle, Square, Pentagon, Hexagon, Circle, Star }
    public Sprite[] polygonSprites;
    public Color color;

    public void Creation(Shape s, Color c, float size)
    {
        normV = 1; // Change velocity relative to the size of the shapes. Default Size: 0.33f
        // Maps smallestSizeSlider(Default: 1) through largestSizeslider(Default: 10) to smallestRealSize(Default: 0.1f) through largestRealSize(Default: 0.7f)
        mapSize = ((size - smallestSizeSlider) /(largestSizeSlider - smallestSizeSlider) * (largestRealSize - smallestRealSize)) + smallestRealSize;
        print("mapSize: " + mapSize);
        switch(s)
        {
            case Shape.Triangle:
                _spriteRenderer.sprite = polygonSprites[0];
                break;
            case Shape.Square:
                _spriteRenderer.sprite = polygonSprites[1];
                break;
            case Shape.Pentagon:
                _spriteRenderer.sprite = polygonSprites[2];
                break;
            case Shape.Hexagon:
                _spriteRenderer.sprite = polygonSprites[3];
                break;
            case Shape.Circle:
                _spriteRenderer.sprite = polygonSprites[4];
                break;
            case Shape.Star:
                _spriteRenderer.sprite = polygonSprites[5];
                break;
        }

        UpdatePolygonCollider2D();
        this.gameObject.transform.localScale = new Vector3(mapSize, mapSize, 0);    
        _spriteRenderer.color = c;

        // Initial x and y velocities. Randomize if the x velocity is negative or positive.
        float randomX = Random.Range(initXVMin, initXVMax);
        float randomY = Random.Range(-initYV * normV, -initYV * normV);
        float randomAngularVel = Random.Range(initAngV, initAngV);
        int randomFlip = Random.Range(0, 2);

        if (randomFlip == 0)
        {
            _rigidbody2D.velocity = new Vector2(randomX, randomY);
            _rigidbody2D.angularVelocity = randomAngularVel;
        }
        else
        {
            _rigidbody2D.velocity = new Vector2(randomX * -1, randomY);
            _rigidbody2D.angularVelocity = randomAngularVel;
        }

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
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _audioSource = FindObjectOfType<AudioSource>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        // rigidbody2D Properties
        _rigidbody2D.angularDrag = 0.5f;

        // Ignoring collisions between other shapes and the edge of the screen until entering the ShapesCollideON collider
        Physics2D.IgnoreLayerCollision(3, 3, true);
        Physics2D.IgnoreCollision(_polyCollider2D, GameObject.FindGameObjectWithTag("edge").GetComponent<EdgeCollider2D>(), true);

        // Audio
        pops[0] = Resources.Load<AudioClip>("Audio/pop1");
        pops[1] = Resources.Load<AudioClip>("Audio/pop2");
        pops[2] = Resources.Load<AudioClip>("Audio/pop3");
}

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(Input.acceleration);

        // Push slow shapes to speed them back up
        if (_rigidbody2D.velocity.x <= slowestV * normV && _rigidbody2D.velocity.x >= -slowestV * normV || _rigidbody2D.velocity.y <= slowestV * normV && _rigidbody2D.velocity.y >= -slowestV * normV)
        {
            float randomAngularVelocity = Random.Range(-pushAngV * normV, pushAngV * normV);
            Vector2 randomVelocity = new Vector2(Random.Range(pushVMin * normV, pushVMax * normV), Random.Range(pushVMin * normV, pushVMax * normV));

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
            int x = Random.Range(0, pops.Length);
            _audioSource.PlayOneShot(pops[x]);

            GameObject pop = Instantiate(popParticles, this.gameObject.GetComponent<Renderer>().bounds.center, Quaternion.identity, this.gameObject.transform.parent);
            GameObject num = Instantiate(countNumber, this.gameObject.GetComponent<Renderer>().bounds.center, Quaternion.identity, this.gameObject.transform.parent);
            int newCount = ++GetComponentInParent<Spawner>().count;
            num.GetComponent<TextMeshPro>().text = newCount.ToString();

            print("POP");
            Destroy(this.gameObject);
        }
    }

    // Collider method
    public void UpdatePolygonCollider2D(float tolerance = 0.05f)
    {
        _polyCollider2D.pathCount = _spriteRenderer.sprite.GetPhysicsShapeCount();
        for (int i = 0; i < _polyCollider2D.pathCount; i++)
        {
            _spriteRenderer.sprite.GetPhysicsShape(i, points);
            LineUtility.Simplify(points, tolerance, simplifiedPoints);
            _polyCollider2D.SetPath(i, simplifiedPoints);
        }
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
        int numSides = vectors.Count;

        if (numSides == 3)
        {
            return new Vector2(vectors.ElementAt(0).x + (0.66f * (vectors.ElementAt(1).x - vectors.ElementAt(0).x)),
                                vectors.ElementAt(0).y + (0.66f * (vectors.ElementAt(1).y - vectors.ElementAt(0).y)));
        }
        else if (numSides == 4)
        {
            return vectors.Aggregate((agg, next) => agg + next) / vectors.Count();
        }

        return vectors.Aggregate((agg, next) => agg + next) / vectors.Count();
    }

    /// Extension returning the absolute value of a vector
    public static Vector2 Abs(Vector2 vector)
    {
        return new Vector2(Mathf.Abs(vector.x), Mathf.Abs(vector.y));
    }
}


