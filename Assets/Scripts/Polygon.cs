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
    private Audio _audio;

    // Effects
    public GameObject[] PopParticles;
    public GameObject TextObj;

    // Game Mode Variables
    private bool _tiltOn;
    public bool EdgesOn;
    private Spawner.Topics _voice;
    private Spawner.Topics _text;
    static private float s_gravityScale = 30f;
    static private float s_gravityLerpTimer = 0.0f;
    static private float s_gravityLerpTimeTotal = 500.0f;
    private float _lerpPercent;
    static private float s_gravityStopMargin = 0.05f;
    private PhysicsMaterial2D _gravityOnMaterial;
    private PhysicsMaterial2D _gravityOffMaterial;

    // Physics
    private float _initYV = 10.0f;
    static private float s_initXVMin = 0.06f;
    static private float s_initXVMax = 2.0f;
    static private float s_initAngV = 300.0f;
    static private float s_pushVMin = 5.0f;
    static private float s_pushVMax = 8.0f;
    static private float s_pushAngV = 300.0f;
    static private float s_slowestV = 0.05f;
    static private float s_maxVx = 20.0f;
    static private float s_maxVy = 20.0f;
    static private float s_maxGravity = 1.5f;
    static private float s_smallestSizeSlider = 1;
    static private float s_largestSizeSlider = 10;
    static private float s_smallestRealSize = 0.1f;
    static private float s_largestRealSize = 0.7f;
    private float _normV;
    private float _shapeMapSize;
    private GameObject _popMapSize;
    private float _numberTextMapSize;
    private float _shapeColorTextMapSize;

    // Collider updater variables
    private List<Vector2> _points = new List<Vector2>();
    private List<Vector2> _simplifiedPoints = new List<Vector2>();

    // Update timer
    private float _gravityWaitTimer = 0f;

    // Polygon variables
    public enum Shape { Triangle, Square, Pentagon, Hexagon, Circle, Star }
    public Sprite[] PolygonSprites;
    private Shape _shape;
    public Spawner.Colors Color;
    public bool IsSolid = false;
    public bool IsPopped = false;
    [SerializeField] public int ID;
    private GameObject _shadowObj;
  
    public void Creation(Shape shape, Color unityColor, Spawner.Colors color, float size, bool edges, bool tilt, Spawner.Topics voice, Spawner.Topics text)
    {
        _normV = 1; // Change velocity relative to the size of the shapes. Default Size: 0.33f

        // Maps s_smallestSizeSlider(Default: 1) through s_largestSizeSlider(Default: 10) to s_smallestRealSize(Default: 0.1f) through s_largestRealSize(Default: 0.7f)
        _shapeMapSize = ((size - s_smallestSizeSlider) /(s_largestSizeSlider - s_smallestSizeSlider) * (s_largestRealSize - s_smallestRealSize)) + s_smallestRealSize;
        _numberTextMapSize = ((size - s_smallestSizeSlider) /(s_largestSizeSlider - s_smallestSizeSlider) * (1.0f - 0.35f)) + 0.35f;
        _shapeColorTextMapSize = ((size - s_smallestSizeSlider) /(s_largestSizeSlider - s_smallestSizeSlider) * (0.5f - 0.3f)) + 0.3f;

        if (size >= 8)
            _popMapSize = PopParticles[0];
        else if (size >= 4)
            _popMapSize = PopParticles[1];
        else
            _popMapSize = PopParticles[2];

        _shape = shape;
        this.Color = color;
        _tiltOn = tilt;
        EdgesOn = edges;
        _voice = voice;
        _text = text;

        // Gravity mode on or off
        if (_tiltOn)
        {
            // normal gravity at the start, then scale up for tilt controls in update()
            _rigidbody2D.gravityScale = 1f;
            _rigidbody2D.sharedMaterial = _gravityOnMaterial;
            _initYV /= 100f;
        }
        else
        {
            _rigidbody2D.gravityScale = 0;
            _rigidbody2D.sharedMaterial = _gravityOffMaterial;
        }

        switch(shape)
        {
            case Shape.Triangle:
                _spriteRenderer.sprite = PolygonSprites[0];
                break;
            case Shape.Square:
                _spriteRenderer.sprite = PolygonSprites[1];
                break;
            case Shape.Pentagon:
                _spriteRenderer.sprite = PolygonSprites[2];
                break;
            case Shape.Hexagon:
                _spriteRenderer.sprite = PolygonSprites[3];
                break;
            case Shape.Circle:
                _spriteRenderer.sprite = PolygonSprites[4];
                break;
            case Shape.Star:
                _spriteRenderer.sprite = PolygonSprites[5];
                break;
        }

        UpdatePolygonCollider2D();
        this.gameObject.transform.localScale = new Vector3(_shapeMapSize, _shapeMapSize, 0);    
        _spriteRenderer.color = unityColor;

        // Initial x and y velocities. Randomize if the x velocity is negative or positive.
        float randomX = Random.Range(s_initXVMin, s_initXVMax);
        float randomY = Random.Range(-_initYV * _normV, -_initYV * _normV);
        float randomAngularVel = Random.Range(-s_initAngV, s_initAngV);
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

        CreateShadow();
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
        _audio = _audioSource.GetComponent<Audio>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        this.ID = PlayerPrefs.GetInt("PolygonID") + 1;
        PlayerPrefs.SetInt("PolygonID", this.ID);

        _gravityOffMaterial = Resources.Load<PhysicsMaterial2D>("Physics/GravityOffMaterial");
        _gravityOnMaterial = Resources.Load<PhysicsMaterial2D>("Physics/GravityOnMaterial");

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
    }

    public void CreateShadow()
    {
        // Create an empty gameobject to be the shadow
        _shadowObj = new GameObject("Shadow");
        _shadowObj.transform.parent = this.gameObject.transform;

        // Attach a sprite renderer component and set its color to black
        SpriteRenderer shadow_sr = _shadowObj.AddComponent<SpriteRenderer>();
        shadow_sr.sprite = _spriteRenderer.sprite;
        shadow_sr.color = UnityEngine.Color.black;
        shadow_sr.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
        shadow_sr.sortingOrder = 0;

        // For actual shadows
        _shadowObj.transform.localScale = new Vector3(1f, 1f, 1f);
        _shadowObj.transform.position = new Vector3(this.gameObject.transform.position.x+0.2f, this.gameObject.transform.position.y+0.2f, this.gameObject.transform.position.z + 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        var dist = 0.5f;
        _shadowObj.transform.position = new Vector3(this.gameObject.transform.position.x + dist, this.gameObject.transform.position.y + dist, this.gameObject.transform.position.z + 0.5f);

        PolygonVelocityLimiter();

        if (_tiltOn)
        {
            _gravityWaitTimer += Time.deltaTime;

            if (_gravityWaitTimer >= 3.0f)
            {
                _rigidbody2D.gravityScale = s_gravityScale;

                // Within pre-determined margin, the shape will slow to a halt instead of moving continuously in one direction.
                if ((Input.acceleration.x <= s_gravityStopMargin && Input.acceleration.x >= -s_gravityStopMargin) 
                    && (Input.acceleration.y <= s_gravityStopMargin && Input.acceleration.y >= -s_gravityStopMargin))
                {
                    Physics2D.gravity = new Vector2(0f, 0f);
                    s_gravityLerpTimer += Time.deltaTime;
                    if (s_gravityLerpTimer > s_gravityLerpTimeTotal)
                    {
                        s_gravityLerpTimer = s_gravityLerpTimeTotal;
                    }

                    _lerpPercent = s_gravityLerpTimer / s_gravityLerpTimeTotal;
                    _rigidbody2D.velocity = Vector3.Lerp(_rigidbody2D.velocity, Vector3.zero, _lerpPercent);
                    _rigidbody2D.angularVelocity = Mathf.Lerp(_rigidbody2D.angularVelocity, 0f, _lerpPercent);
                }
                else
                {
                    GravityLimiter();
                }
            }
        }
        else
        {
            PushSlowShapes();            
        }

        if (!EdgesOn)
        {
            if (gameObject.transform.position.x > (Screen.width / Camera.main.orthographicSize) / 4 || gameObject.transform.position.x < -(Screen.width / Camera.main.orthographicSize) / 4)
            {
                gameObject.transform.position = new Vector2(-gameObject.transform.position.x, gameObject.transform.position.y);
            }

            if (gameObject.transform.position.y > (Screen.height / Camera.main.orthographicSize) / 4 || gameObject.transform.position.y < -(Screen.height / Camera.main.orthographicSize) / 4)
            {
                gameObject.transform.position = new Vector2(gameObject.transform.position.x, -gameObject.transform.position.y);
            }
        }

        if (gameObject.transform.position.x > (Screen.width/Camera.main.orthographicSize) || gameObject.transform.position.y > (Screen.height/Camera.main.orthographicSize)
                || gameObject.transform.position.x < -(Screen.width / Camera.main.orthographicSize) || gameObject.transform.position.y < -(Screen.height / Camera.main.orthographicSize))
        {
            TeleportSound();
            gameObject.transform.position = Vector2.zero;
        }

    }
    
    // Shapes slow down and stop eventually. This keeps them always moving.
    private void PushSlowShapes()
    {
        if (_rigidbody2D.velocity.x <= s_slowestV * _normV && _rigidbody2D.velocity.x >= -s_slowestV * _normV
                || _rigidbody2D.velocity.y <= s_slowestV * _normV && _rigidbody2D.velocity.y >= -s_slowestV * _normV)
        {
            float randomAngularVelocity = Random.Range(-s_pushAngV * _normV, s_pushAngV * _normV);
            Vector2 randomVelocity = new Vector2(Random.Range(s_pushVMin * _normV, s_pushVMax * _normV), Random.Range(s_pushVMin * _normV, s_pushVMax * _normV));

            _rigidbody2D.velocity = randomVelocity;
            _rigidbody2D.angularVelocity = randomAngularVelocity;
            //Debug.Log("pushing " + this.name + " - angular velocity: " + randomAngularVelocity + " velocity: " + randomVelocity);
        }
    }

    private Vector3 mOffset;
    private float mZCoord;
    public float mouseSpeed;
    public float mouseXNorm;
    public float mouseYNorm;
    public Vector3 mouseStartPosition;
    public Vector3 mouseEndPosition;

    private void Pop()
    {
        IsPopped = true;
        PopSound();
        VoiceSound();
        Instantiate(_popMapSize, this.gameObject.GetComponent<Renderer>().bounds.center, Quaternion.identity, this.gameObject.transform.parent);
        SpawnPopText();
        Destroy(this.gameObject);
    }

    // Shape pop
    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Pop();
        }
    }

    // Collider method
    public void UpdatePolygonCollider2D(float tolerance = 0.05f)
    {
        _polyCollider2D.pathCount = _spriteRenderer.sprite.GetPhysicsShapeCount();
        for (int i = 0; i < _polyCollider2D.pathCount; i++)
        {
            _spriteRenderer.sprite.GetPhysicsShape(i, _points);
            LineUtility.Simplify(_points, tolerance, _simplifiedPoints);
            _polyCollider2D.SetPath(i, _simplifiedPoints);
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

    // Extension that limits the gravity
    public void GravityLimiter()
    {
        if (Physics2D.gravity.x > s_maxGravity)
        {
            Physics2D.gravity = new Vector2(s_maxGravity, Physics2D.gravity.y);
        }
        else if (Physics2D.gravity.x < -s_maxGravity)
        {
            Physics2D.gravity = new Vector2(-s_maxGravity, Physics2D.gravity.y);
        }
        else if (Physics2D.gravity.y > s_maxGravity)
        {
            Physics2D.gravity = new Vector2(Physics2D.gravity.x, s_maxGravity);
        }
        else if (Physics2D.gravity.y < -s_maxGravity)
        {
            Physics2D.gravity = new Vector2(Physics2D.gravity.x, -s_maxGravity);
        }
        else
        {
            Physics2D.gravity = new Vector2(Input.acceleration.x * 1.5f, Input.acceleration.y * 1.5f);
        }
    }

    // Extension that limits polygon velocity to maximum
    public void PolygonVelocityLimiter()
    {
        if (_rigidbody2D.velocity.x > s_maxVx || _rigidbody2D.velocity.x < -s_maxVx)
            _rigidbody2D.velocity = new Vector2(s_maxVx * Mathf.Sign(_rigidbody2D.velocity.x), _rigidbody2D.velocity.y);

        if (_rigidbody2D.velocity.y > s_maxVy || _rigidbody2D.velocity.y < -s_maxVy)
            _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, s_maxVy * Mathf.Sign(_rigidbody2D.velocity.y));
    }

    public void TeleportSound()
    {
        int x = Random.Range(0, _audio.teleports.Length);
        _audioSource.PlayOneShot(_audio.teleports[x]);
    }

    public void PopSound()
    {
        int x = Random.Range(0, _audio.pops.Length);
        _audioSource.PlayOneShot(_audio.pops[x]);
    }

    public void VoiceSound()
    {
        if (_voice == Spawner.Topics.Shapes)
        {
            _audioSource.PlayOneShot(_audio.voices_shapes[(int)_shape]);
        }
        else if (_voice == Spawner.Topics.Colors)
        {
            _audioSource.PlayOneShot(_audio.voices_colors[(int)Color]);
        }
        else if (_voice == Spawner.Topics.Numbers)
        {
            // Need to increment the polygon count if the pop text isn't on. Because it increments in pop text otherwise.
            if (_text != Spawner.Topics.Numbers)
                _audioSource.PlayOneShot(_audio.voices_numbers[GetComponentInParent<Spawner>().count++]);
            else
                _audioSource.PlayOneShot(_audio.voices_numbers[GetComponentInParent<Spawner>().count]);
        }
        else
        {
            // Who goes there?!
        }
    }

    public void SpawnPopText()
    {
        if (_text == Spawner.Topics.Shapes)
        {
            GameObject tempTextObj = Instantiate(TextObj, this.gameObject.GetComponent<Renderer>().bounds.center, Quaternion.identity, this.gameObject.transform.parent);
            tempTextObj.transform.localScale = new Vector3(_shapeColorTextMapSize, _shapeColorTextMapSize, 0);
            TextMeshPro tempTextObjTMP = tempTextObj.GetComponent<TextMeshPro>();
            tempTextObjTMP.fontSize = 40;
            tempTextObjTMP.text = _shape.ToString();
            tempTextObjTMP.font = Resources.Load<TMP_FontAsset>("Font/Vanillaextract-Unshaded SDF");
            tempTextObj.GetComponent<BoxCollider2D>().size = new Vector2(tempTextObjTMP.preferredWidth, 4);
        }
        else if (_text == Spawner.Topics.Colors)
        {
            GameObject tempTextObj = Instantiate(TextObj, this.gameObject.GetComponent<Renderer>().bounds.center, Quaternion.identity, this.gameObject.transform.parent);
            tempTextObj.transform.localScale = new Vector3(_shapeColorTextMapSize, _shapeColorTextMapSize, 0);
            TextMeshPro tempTextObjTMP = tempTextObj.GetComponent<TextMeshPro>();
            tempTextObjTMP.fontSize = 40;
            tempTextObjTMP.text = Color.ToString();
            tempTextObjTMP.font = Resources.Load<TMP_FontAsset>("Font/Vanillaextract-Unshaded SDF");
            tempTextObj.GetComponent<BoxCollider2D>().size = new Vector2(tempTextObjTMP.preferredWidth, 4);
        }
        else if (_text == Spawner.Topics.Numbers)
        {
            GameObject tempTextObj = Instantiate(TextObj, this.gameObject.GetComponent<Renderer>().bounds.center, Quaternion.identity, this.gameObject.transform.parent);
            tempTextObj.transform.localScale = new Vector3(_numberTextMapSize, _numberTextMapSize, 0);
            int newCount = ++GetComponentInParent<Spawner>().count;
            TextMeshPro tempTextObjTMP = tempTextObj.GetComponent<TextMeshPro>();
            tempTextObjTMP.text = newCount.ToString();
            tempTextObj.GetComponent<BoxCollider2D>().size = new Vector2(tempTextObjTMP.preferredWidth, 4);
        }
        else // Off
        {
            // Dance in the emptiness
        }
    }
}


