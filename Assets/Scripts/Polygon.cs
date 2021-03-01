using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class Polygon : MonoBehaviour
{
    // Components
    private Rigidbody2D _rigidbody2D;
    private PolygonCollider2D _polyCollider2D;
    private SpriteRenderer _spriteRenderer;
    private AudioSource _audioSource;

    // Effects
    public GameObject[] popParticles;
    public GameObject textObj;

    // Game Mode Variables
    private bool tiltOn;
    public bool edgesOn;
    private Spawner.Topics voice;
    private Spawner.Topics text;
    private float gravityScale = 30f;
    private float gravityLerpTimer = 0.0f;
    private float gravityLerpTimeTotal = 500.0f;
    private float lerpPercent;
    private float gravityStopMargin = 0.05f;
    private PhysicsMaterial2D gravityOnMaterial;
    private PhysicsMaterial2D gravityOffMaterial;
    private float shakeForceScale = 1f;
    public float shakeDetectionThreshold;
    public float minShakeInterval;
    private float sqrShakeDetectionThreshold;
    private float timeSinceLastShake;

    // Physics
    private float initYV = 10.0f;
    private float initXVMin = 0.06f;
    private float initXVMax = 2.0f;
    private float initAngV = 300.0f;
    private float pushVMin = 5.0f;
    private float pushVMax = 8.0f;
    private float pushAngV = 300.0f;
    private float slowestV = 0.05f;
    private float smallestSizeSlider = 1;
    private float largestSizeSlider = 10;
    private float smallestRealSize = 0.1f;
    private float largestRealSize = 0.7f;
    private float normV;
    private float shapeMapSize;
    private GameObject popMapSize;
    private float numberTextMapSize;
    private float shapeColorTextMapSize;

    // Sound
    private AudioClip[] pops = new AudioClip[3];
    private AudioClip[] teleports = new AudioClip[3];

    // Collider updater variables
    private List<Vector2> points = new List<Vector2>();
    private List<Vector2> simplifiedPoints = new List<Vector2>();

    // Update timer
    private float gravityWaitTimer = 0f;

    // Polygon variables
    public enum Shape { Triangle, Square, Pentagon, Hexagon, Circle, Star }
    public Sprite[] polygonSprites;
    private Polygon.Shape shape;
    public Spawner.Colors color;
    public bool solid = false;
    public bool popped = false;

    public void Creation(Shape shape, Color unityColor, Spawner.Colors color, float size, bool edges, bool tilt, Spawner.Topics voice, Spawner.Topics text)
    {
        normV = 1; // Change velocity relative to the size of the shapes. Default Size: 0.33f

        // Maps smallestSizeSlider(Default: 1) through largestSizeslider(Default: 10) to smallestRealSize(Default: 0.1f) through largestRealSize(Default: 0.7f)
        shapeMapSize = ((size - smallestSizeSlider) /(largestSizeSlider - smallestSizeSlider) * (largestRealSize - smallestRealSize)) + smallestRealSize;
        numberTextMapSize = ((size - smallestSizeSlider) /(largestSizeSlider - smallestSizeSlider) * (1.0f - 0.35f)) + 0.35f;
        shapeColorTextMapSize = ((size - smallestSizeSlider) /(largestSizeSlider - smallestSizeSlider) * (0.5f - 0.3f)) + 0.3f;

        if (size >= 8)
            popMapSize = popParticles[0];
        else if (size >= 4)
            popMapSize = popParticles[1];
        else
            popMapSize = popParticles[2];

        this.shape = shape;
        this.color = color;
        this.tiltOn = tilt;
        this.edgesOn = edges;
        this.voice = voice;
        this.text = text;

        // Gravity mode on or off
        if (tiltOn)
        {
            // normal gravity at the start, then scale up for tilt controls in update()
            _rigidbody2D.gravityScale = 1f;
            _rigidbody2D.sharedMaterial = gravityOnMaterial;
            initYV /= 100f;
        }
        else
        {
            _rigidbody2D.gravityScale = 0;
            _rigidbody2D.sharedMaterial = gravityOffMaterial;
        }

        switch(shape)
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
        this.gameObject.transform.localScale = new Vector3(shapeMapSize, shapeMapSize, 0);    
        _spriteRenderer.color = unityColor;

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

        gravityOffMaterial = Resources.Load<PhysicsMaterial2D>("Physics/GravityOffMaterial");
        gravityOnMaterial = Resources.Load<PhysicsMaterial2D>("Physics/GravityOnMaterial");
        sqrShakeDetectionThreshold = Mathf.Pow(shakeDetectionThreshold, 2);

        // rigidbody2D Properties
        _rigidbody2D.angularDrag = 0.5f;

        // Ignoring collisions between other shapes and the edge of the screen until entering the ShapesCollideON collider. Also ignore text colliders layer 6.
        Physics2D.IgnoreLayerCollision(3, 3, true);
        Physics2D.IgnoreLayerCollision(3, 6, true);
        Physics2D.IgnoreLayerCollision(6, 2, true); // Pop shapes through text colliders
        GameObject[] screenEdges = GameObject.FindGameObjectsWithTag("edge");
        foreach (GameObject screenEdge in screenEdges)
        {
            Physics2D.IgnoreCollision(_polyCollider2D, screenEdge.GetComponent<BoxCollider2D>(), true);
        }

        // Audio
        pops[0] = Resources.Load<AudioClip>("Audio/pop1");
        pops[1] = Resources.Load<AudioClip>("Audio/pop2");
        pops[2] = Resources.Load<AudioClip>("Audio/pop3");
        teleports[0] = Resources.Load<AudioClip>("Audio/teleport1");
        teleports[1] = Resources.Load<AudioClip>("Audio/teleport2");
        teleports[2] = Resources.Load<AudioClip>("Audio/teleport3");
    }

    // Update is called once per frame
    void Update()
    {
        if (tiltOn)
        {
            gravityWaitTimer += Time.deltaTime;

            if (gravityWaitTimer >= 3.0f)
            {
                _rigidbody2D.gravityScale = gravityScale;

                // Within -0.1f and 0.1f, the shape will slow to a halt instead of floating in zero G.
                if ((Input.acceleration.x <= gravityStopMargin && Input.acceleration.x >= -gravityStopMargin) 
                    && (Input.acceleration.y <= gravityStopMargin && Input.acceleration.y >= -gravityStopMargin))
                {
                    Physics2D.gravity = new Vector2(0f, 0f);
                    //Physics2D.gravity = new Vector2(Mathf.Lerp(Physics2D.gravity.x, 0, gravityLerpTime), Mathf.Lerp(Physics2D.gravity.y, 0, gravityLerpTime));
                    gravityLerpTimer += Time.deltaTime;
                    if (gravityLerpTimer > gravityLerpTimeTotal)
                    {
                        gravityLerpTimer = gravityLerpTimeTotal;
                    }

                    lerpPercent = gravityLerpTimer / gravityLerpTimeTotal;
                    _rigidbody2D.velocity = Vector3.Lerp(_rigidbody2D.velocity, Vector3.zero, lerpPercent);
                    _rigidbody2D.angularVelocity = Mathf.Lerp(_rigidbody2D.angularVelocity, 0f, lerpPercent);

                    if (Input.acceleration.sqrMagnitude >= sqrShakeDetectionThreshold
                           && Time.unscaledTime >= timeSinceLastShake + minShakeInterval)
                    {
                        _rigidbody2D.AddForce(Input.acceleration * shakeForceScale, ForceMode2D.Impulse);
                        timeSinceLastShake = Time.unscaledTime;
                    }
                }
                else
                {
                    Physics2D.gravity = new Vector2(Input.acceleration.x * 1.5f, Input.acceleration.y * 1.5f);

                    if (Input.acceleration.sqrMagnitude >= sqrShakeDetectionThreshold
                           && Time.unscaledTime >= timeSinceLastShake + minShakeInterval)
                    {
                        _rigidbody2D.AddForce(Input.acceleration * shakeForceScale, ForceMode2D.Impulse);
                        timeSinceLastShake = Time.unscaledTime;
                    }
                }
            }
        }
        else
        {
            // Push slow shapes to speed them back up
            if (_rigidbody2D.velocity.x <= slowestV * normV && _rigidbody2D.velocity.x >= -slowestV * normV 
                || _rigidbody2D.velocity.y <= slowestV * normV && _rigidbody2D.velocity.y >= -slowestV * normV)
            {
                float randomAngularVelocity = Random.Range(-pushAngV * normV, pushAngV * normV);
                Vector2 randomVelocity = new Vector2(Random.Range(pushVMin * normV, pushVMax * normV), Random.Range(pushVMin * normV, pushVMax * normV));

                _rigidbody2D.velocity = randomVelocity;
                _rigidbody2D.angularVelocity = randomAngularVelocity;
                Debug.Log("pushing " + this.name + " - angular velocity: " + randomAngularVelocity + " velocity: " + randomVelocity);
            }
        }
    }

    // Tap pop shapes!
    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            popped = true;
            PopSound();


            GameObject pop = Instantiate(popMapSize, this.gameObject.GetComponent<Renderer>().bounds.center, Quaternion.identity, this.gameObject.transform.parent);

            if (text == Spawner.Topics.Shapes)
            {
                GameObject tempTextObj = Instantiate(textObj, this.gameObject.GetComponent<Renderer>().bounds.center, Quaternion.identity, this.gameObject.transform.parent);
                tempTextObj.transform.localScale = new Vector3(shapeColorTextMapSize, shapeColorTextMapSize, 0);
                TextMeshPro tempTextObjTMP = tempTextObj.GetComponent<TextMeshPro>();
                tempTextObjTMP.fontSize = 40;
                tempTextObjTMP.text = shape.ToString();
                tempTextObjTMP.font = Resources.Load<TMP_FontAsset>("Font/Vanillaextract-Unshaded SDF");
                tempTextObj.GetComponent<BoxCollider2D>().size = new Vector2(tempTextObjTMP.preferredWidth, 4);
            }
            else if (text == Spawner.Topics.Colors)
            {
                GameObject tempTextObj = Instantiate(textObj, this.gameObject.GetComponent<Renderer>().bounds.center, Quaternion.identity, this.gameObject.transform.parent);
                tempTextObj.transform.localScale = new Vector3(shapeColorTextMapSize, shapeColorTextMapSize, 0);
                TextMeshPro tempTextObjTMP = tempTextObj.GetComponent<TextMeshPro>();
                tempTextObjTMP.fontSize = 40;
                tempTextObjTMP.text = color.ToString();
                tempTextObjTMP.font = Resources.Load<TMP_FontAsset>("Font/Vanillaextract-Unshaded SDF");
                tempTextObj.GetComponent<BoxCollider2D>().size = new Vector2(tempTextObjTMP.preferredWidth, 4);
            }
            else if (text == Spawner.Topics.Numbers)
            {
                GameObject tempTextObj = Instantiate(textObj, this.gameObject.GetComponent<Renderer>().bounds.center, Quaternion.identity, this.gameObject.transform.parent);
                tempTextObj.transform.localScale = new Vector3(numberTextMapSize, numberTextMapSize, 0);
                int newCount = ++GetComponentInParent<Spawner>().count;
                TextMeshPro tempTextObjTMP = tempTextObj.GetComponent<TextMeshPro>();
                tempTextObjTMP.text = newCount.ToString();
                tempTextObj.GetComponent<BoxCollider2D>().size = new Vector2(tempTextObjTMP.preferredWidth, 4);
            }
            else // Off
            {
                // Dance in the emptiness
            }

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

    public void TeleportSound()
    {
        int x = Random.Range(0, teleports.Length);
        _audioSource.PlayOneShot(teleports[x]);
    }

    public void PopSound()
    {
        int x = Random.Range(0, pops.Length);
        _audioSource.PlayOneShot(pops[x]);
    }
}


