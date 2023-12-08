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
    [Tooltip("Select 3 sizes of particle objects for when the shape is popped")]
    public GameObject[] PopParticles;
    [Tooltip("Select a number text object for when the shape is popped")]
    public GameObject NumberTextObj;
    [Tooltip("Select a word text object for when the shape is popped")]
    public GameObject WordTextObj;

    // Game Mode Variables
    private bool _tiltOn;
    public bool EdgesOn;
    private Spawner.Topics _topic;
    private bool _voice;
    private bool _text;
    static private float s_gravityScale = 30f;
    private float _gravityLerpTimer = 0.0f;
    static private float s_gravityLerpTimeTotal = 500.0f;
    private float _lerpPercent;
    static private float s_gravityStopMargin = 0.05f;
    private PhysicsMaterial2D _gravityOnMaterial;
    private PhysicsMaterial2D _gravityOffMaterial;

    // Physics
    private Touch touch;
    private float _initYV = 15.0f;
    static private float s_initXVMin = 0.06f;
    static private float s_initXVMax = 2.0f;
    static private float s_initAngV = 300.0f;
    static private float s_pushVMin = 2.0f;
    static private float s_pushVMax = 6.0f;
    static private float s_pushAngV = 300.0f;
    static private float s_slowestV = 0.35f;
    static private float s_maxVx = 10.0f;
    static private float s_maxVy = 10.0f;
    static private float s_maxGravity = 1.5f;
    static private float s_smallestSizeSlider = 1;
    static private float s_largestSizeSlider = 10;
    static private float s_shapeTextSmallestRealSize = 0.1f;
    static private float s_shapeTextLargestRealSize = 0.7f;
    static private float s_numberTextSmallestRealSize = 0.35f;
    static private float s_numberTextLargestRealSize = 1.0f;
    static private float s_colorTextSmallestRealSize = 0.3f;
    static private float s_colorTextLargestRealSize = 0.5f;
    static private int s_outOfBoundsRatioEdgesOn = 2;
    static private int s_outOfBoundsRatioEdgesOff = 1;
    private bool _outOfBoundsFlag = false;
    private float _normV;
    private float _shapeMapSize;
    private GameObject _popMapSize;
    private float _numberTextMapSize;
    private float _shapeColorTextMapSize;
    private Vector2 _screenSize;

    // Collider updater variables
    private List<Vector2> _points = new List<Vector2>();
    private List<Vector2> _simplifiedPoints = new List<Vector2>();

    // Update timer
    private float _gravityWaitTimer = 0f;

    // Polygon variables
    [Tooltip("Select sprites the polygon can become")]
    public Sprite[] PolygonSprites;
    public Spawner.Shape Shape;
    public Spawner.Colors Color;
    public bool IsSolid = false;
    public bool IsPopped = false;
    [SerializeField] public int ID;
    private GameObject _shadowObj;
    static private float s_shadowDist = 0.2f;
    static private Vector2 s_maxWidthVector;


    private void Awake()
    {
        _polyCollider2D = GetComponent<PolygonCollider2D>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _audioSource = GameObject.FindGameObjectWithTag("SFXSource").GetComponent<AudioSource>();
        _audio = _audioSource.GetComponent<Audio>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        ID = PlayerPrefs.GetInt("PolygonID") + 1;
        PlayerPrefs.SetInt("PolygonID", ID);

        _gravityOffMaterial = Resources.Load<PhysicsMaterial2D>("Physics/GravityOffMaterial");
        _gravityOnMaterial = Resources.Load<PhysicsMaterial2D>("Physics/GravityOnMaterial");
    }

    // Update is called once per frame
    void Update()
    {
        // Constantly update shadow position to follow the polygon
        _shadowObj.transform.position = new Vector3(gameObject.transform.position.x + s_shadowDist, 
                                                    gameObject.transform.position.y + -s_shadowDist, 
                                                    gameObject.transform.position.z + s_shadowDist);

        PolygonVelocityLimiter();

        CheckTouch();

        if (_tiltOn)
        {
            _gravityWaitTimer += Time.deltaTime;

            if (_gravityWaitTimer >= 5.0f)
            {
                _rigidbody2D.gravityScale = s_gravityScale;

                // Within pre-determined margin, the shape will slow to a halt instead of moving continuously in one direction.
                if ((Input.acceleration.x <= s_gravityStopMargin && Input.acceleration.x >= -s_gravityStopMargin) 
                    && (Input.acceleration.y <= s_gravityStopMargin && Input.acceleration.y >= -s_gravityStopMargin))
                {
                    Physics2D.gravity = new Vector2(0f, 0f);
                    _gravityLerpTimer += Time.deltaTime;

                    if (_gravityLerpTimer > s_gravityLerpTimeTotal)
                    {
                        _gravityLerpTimer = s_gravityLerpTimeTotal;
                    }

                    _lerpPercent = _gravityLerpTimer / s_gravityLerpTimeTotal;
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
            // Return shape to opposite side to loop around the screen.
            if (gameObject.transform.position.x > (Screen.width / Camera.main.orthographicSize) / 1 || gameObject.transform.position.x < -(Screen.width / Camera.main.orthographicSize) / 1)
            {
                gameObject.transform.position = new Vector2(-1 * gameObject.transform.position.x, gameObject.transform.position.y);
            }

            if (gameObject.transform.position.y > (Screen.height / Camera.main.orthographicSize) / 1 || gameObject.transform.position.y < -(Screen.height / Camera.main.orthographicSize) / 1)
            {
                gameObject.transform.position = new Vector2(gameObject.transform.position.x, -1 * gameObject.transform.position.y);
            }
        }

        if (IsSolid && !_outOfBoundsFlag)
        {
            gameObject.layer = LayerMask.NameToLayer("shapes");
            _polyCollider2D.isTrigger = false;
            _outOfBoundsFlag = true;
        }

        if (!IsSolid)
            InBoundsSolid();

        if (IsSolid)
            OutOfBoundsRecall();
    }

    public void Creation(Spawner.Shape shape, Color unityColor, Spawner.Colors color, float size, bool edges, bool tilt, Spawner.Topics topic, bool voice, bool text)
    {
        // Change velocity relative to the size of the shapes. Default Size: 0.33f
        _normV = 1;

        // Maps slider position to real size
        MapTextSliders(size);

        if (size >= 8)
            _popMapSize = PopParticles[0];
        else if (size >= 4)
            _popMapSize = PopParticles[1];
        else
            _popMapSize = PopParticles[2];

        this.Shape = shape;
        Color = color;
        _tiltOn = tilt;
        EdgesOn = edges;
        _topic = topic;
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

        switch (shape)
        {
            case Spawner.Shape.Triangle:
                _spriteRenderer.sprite = PolygonSprites[0];
                break;
            case Spawner.Shape.Square:
                _spriteRenderer.sprite = PolygonSprites[1];
                break;
            case Spawner.Shape.Pentagon:
                _spriteRenderer.sprite = PolygonSprites[2];
                break;
            case Spawner.Shape.Hexagon:
                _spriteRenderer.sprite = PolygonSprites[3];
                break;
            case Spawner.Shape.Circle:
                _spriteRenderer.sprite = PolygonSprites[4];
                break;
            case Spawner.Shape.Star:
                _spriteRenderer.sprite = PolygonSprites[5];
                break;
        }

        UpdatePolygonCollider2D();
        gameObject.transform.localScale = new Vector3(_shapeMapSize, _shapeMapSize, 0);
        _spriteRenderer.color = unityColor;

        // Getting 'radius' of shape for turning it solid when entering the screen
        s_maxWidthVector = gameObject.GetComponent<SpriteRenderer>().bounds.extents;

        _screenSize.x = Vector2.Distance(Camera.main.ScreenToWorldPoint(new Vector2(0, 0)), Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0))) * 0.5f;
        _screenSize.y = Vector2.Distance(Camera.main.ScreenToWorldPoint(new Vector2(0, 0)), Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height))) * 0.5f;

        SetInitialVelocities();

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

    public void CreateShadow()
    {
        // Create an empty gameobject to be the shadow.
        _shadowObj = new GameObject("Shadow");
        _shadowObj.transform.parent = gameObject.transform;

        // Attach a sprite renderer component and set its color to black.
        SpriteRenderer _shadow_sr = _shadowObj.AddComponent<SpriteRenderer>();
        _shadow_sr.sprite = _spriteRenderer.sprite;
        _shadow_sr.color = new UnityEngine.Color(0, 0, 0, 0.70f);
        _shadow_sr.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
        _shadow_sr.sortingOrder = 0;

        // For actual shadows
        _shadowObj.transform.localScale = new Vector3(1f, 1f, 1f);
        _shadowObj.transform.position = new Vector3(gameObject.transform.position.x + s_shadowDist, 
                                                    gameObject.transform.position.y + -s_shadowDist, 
                                                    gameObject.transform.position.z + s_shadowDist);
    }

    /// <summary>
    /// Maps the settings panel's size slider to what the real size of the text should be
    /// </summary>
    /// <param name="size">Size of polygon</param>
    private void MapTextSliders(float size)
    {
        _shapeMapSize = ((size - s_smallestSizeSlider) / (s_largestSizeSlider - s_smallestSizeSlider) * (s_shapeTextLargestRealSize - s_shapeTextSmallestRealSize)) + s_shapeTextSmallestRealSize;
        _numberTextMapSize = ((size - s_smallestSizeSlider) / (s_largestSizeSlider - s_smallestSizeSlider) * (s_numberTextLargestRealSize - s_numberTextSmallestRealSize)) + s_numberTextSmallestRealSize;
        _shapeColorTextMapSize = ((size - s_smallestSizeSlider) / (s_largestSizeSlider - s_smallestSizeSlider) * (s_colorTextLargestRealSize - s_colorTextSmallestRealSize)) + s_colorTextSmallestRealSize;
    }

    // Shapes slow down and stop eventually. This keeps them always moving.
    private void PushSlowShapes()
    {
        if (_rigidbody2D.velocity.x <= s_slowestV * _normV && _rigidbody2D.velocity.x >= -s_slowestV * _normV
                && _rigidbody2D.velocity.y <= s_slowestV * _normV && _rigidbody2D.velocity.y >= -s_slowestV * _normV)
        {
            float randomAngularVelocity = Random.Range(-s_pushAngV * _normV, s_pushAngV * _normV);
            Vector2 randomVelocity = new Vector2(Random.Range(s_pushVMin * _normV, s_pushVMax * _normV), Random.Range(s_pushVMin * _normV, s_pushVMax * _normV));

            _rigidbody2D.velocity = randomVelocity;
            _rigidbody2D.angularVelocity = randomAngularVelocity;
            //Debug.Log("pushing " + this.name + " - angular velocity: " + randomAngularVelocity + " velocity: " + randomVelocity);
        }
    }

    private void InBoundsSolid()
    {
        float shapeWidth = gameObject.transform.position.x + s_maxWidthVector.x;
        float shapeHeight = gameObject.transform.position.y + s_maxWidthVector.y;

        if (shapeWidth < _screenSize.x && shapeWidth > -_screenSize.x 
            && shapeHeight < _screenSize.y && shapeHeight > -_screenSize.y)
        {
            IsSolid = true;
        }
    }


    private void OutOfBoundsRecall()
    {
        if(EdgesOn)
        {
            if (gameObject.transform.position.x > (Screen.width / Camera.main.orthographicSize) / s_outOfBoundsRatioEdgesOn || gameObject.transform.position.y > (Screen.height / Camera.main.orthographicSize) / s_outOfBoundsRatioEdgesOn
                    || gameObject.transform.position.x < -(Screen.width / Camera.main.orthographicSize) / s_outOfBoundsRatioEdgesOn || gameObject.transform.position.y < -(Screen.height / Camera.main.orthographicSize) / s_outOfBoundsRatioEdgesOn)
            {
                TeleportSound();
                gameObject.transform.position = Vector2.zero;
            }
        }
        else
        {
            if (gameObject.transform.position.x > (Screen.width / Camera.main.orthographicSize) / s_outOfBoundsRatioEdgesOff || gameObject.transform.position.y > (Screen.height / Camera.main.orthographicSize) / s_outOfBoundsRatioEdgesOff
                    || gameObject.transform.position.x < -(Screen.width / Camera.main.orthographicSize) / s_outOfBoundsRatioEdgesOff || gameObject.transform.position.y < -(Screen.height / Camera.main.orthographicSize) / s_outOfBoundsRatioEdgesOff)
            {
                TeleportSound();
                gameObject.transform.position = Vector2.zero;
            }
        }
    }

    private void CheckTouch()
    {
        if (Input.touchCount > 0)
        {
            // Loop through all active touches
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);

                if (touch.phase == TouchPhase.Began)
                {
                    // Convert touch position to world position
                    Vector3 worldTouchPosition = Camera.main.ScreenToWorldPoint(touch.position);

                    // Perform a raycast to detect if the object is touched
                    RaycastHit2D hit = Physics2D.Raycast(worldTouchPosition, Vector2.zero);

                    // Check if the object was hit by the raycast
                    if (hit.collider != null && hit.collider.gameObject == gameObject)
                    {
                        Pop();
                    }
                }
            }
        }
    }

    private void Pop()
    {
        IsPopped = true;
        PopSound();
        VoiceSound();
        Instantiate(_popMapSize, this.gameObject.GetComponent<Renderer>().bounds.center, Quaternion.identity, this.gameObject.transform.parent);
        SpawnPopText();
        Destroy(this.gameObject);
    }

    // PC Testing. Not compiled for mobile compilation
    #if UNITY_EDITOR

        private void OnMouseOver()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Pop();
            }
        }
        
        private void DebugPop()
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                Pop();
            }
        }

    #endif

// Initialize x and y velocities. Randomize if the x velocity is negative or positive.
private void SetInitialVelocities()
    {
        float randomX = Random.Range(s_initXVMin, s_initXVMax);
        float randomY = Random.Range(-_initYV * _normV, -_initYV * _normV);
        float randomAngularVel = Random.Range(-s_initAngV, s_initAngV);
        int randomFlip = Random.Range(0, 2);

        _rigidbody2D.velocity = randomFlip == 0 ? new Vector2(randomX, randomY) : new Vector2(randomX * -1, randomY);

        _rigidbody2D.angularVelocity = randomAngularVel;
    }

    // Collider method.
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
        {
            _rigidbody2D.velocity = new Vector2(s_maxVx * Mathf.Sign(_rigidbody2D.velocity.x), _rigidbody2D.velocity.y);
        }

        if (_rigidbody2D.velocity.y > s_maxVy || _rigidbody2D.velocity.y < -s_maxVy)
        { 
            _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, s_maxVy * Mathf.Sign(_rigidbody2D.velocity.y));
        }
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
        if (_topic == Spawner.Topics.Shapes && _voice)
        {
            _audioSource.PlayOneShot(_audio.voices_shapes[(int)Shape]);
        }
        else if (_topic == Spawner.Topics.Colors && _voice)
        {
            _audioSource.PlayOneShot(_audio.voices_colors[(int)Color]);
        }
        else if (_topic == Spawner.Topics.Numbers && _voice)
        {
            // Need to increment the polygon count if the pop text isn't on. Because it increments in pop text otherwise.
            if (!_text)
                _audioSource.PlayOneShot(_audio.voices_numbers[GetComponentInParent<Spawner>().Count++]);
            else
                _audioSource.PlayOneShot(_audio.voices_numbers[GetComponentInParent<Spawner>().Count]);
        }
        else
        {
            // Voice Option Off
        }
    }

    public void SpawnPopText()
    {
        if (_topic == Spawner.Topics.Shapes && _text)
        {
            GameObject tempTextObj = Instantiate(WordTextObj, this.gameObject.GetComponent<Renderer>().bounds.center, Quaternion.identity, this.gameObject.transform.parent);
            tempTextObj.transform.localScale = new Vector3(_shapeColorTextMapSize, _shapeColorTextMapSize, 0);
            TextMeshPro tempTextObjTMP = tempTextObj.GetComponent<TextMeshPro>();
            //tempTextObjTMP.fontSize = 40;
            tempTextObjTMP.text = Shape.ToString();
            //tempTextObjTMP.font = Resources.Load<TMP_FontAsset>("Font/Vanillaextract-Unshaded SDF");
            tempTextObj.GetComponent<BoxCollider2D>().size = new Vector2(tempTextObjTMP.preferredWidth, 4);
        }
        else if (_topic == Spawner.Topics.Colors && _text)
        {
            GameObject tempTextObj = Instantiate(WordTextObj, this.gameObject.GetComponent<Renderer>().bounds.center, Quaternion.identity, this.gameObject.transform.parent);
            tempTextObj.transform.localScale = new Vector3(_shapeColorTextMapSize, _shapeColorTextMapSize, 0);
            TextMeshPro tempTextObjTMP = tempTextObj.GetComponent<TextMeshPro>();
            //tempTextObjTMP.fontSize = 40;
            tempTextObjTMP.text = Color.ToString();
            tempTextObjTMP.color = Spawner.EnumColortoUnityColor(this.Color);
            //tempTextObjTMP.font = Resources.Load<TMP_FontAsset>("Font/Vanillaextract-Unshaded SDF");
            tempTextObj.GetComponent<BoxCollider2D>().size = new Vector2(tempTextObjTMP.preferredWidth, 4);
        }
        else if (_topic == Spawner.Topics.Numbers && _text)
        {
            GameObject tempTextObj = Instantiate(NumberTextObj, this.gameObject.GetComponent<Renderer>().bounds.center, Quaternion.identity, this.gameObject.transform.parent);
            tempTextObj.transform.localScale = new Vector3(_numberTextMapSize, _numberTextMapSize, 0);
            int _newCount = ++GetComponentInParent<Spawner>().Count;
            TextMeshPro tempTextObjTMP = tempTextObj.GetComponent<TextMeshPro>();
            tempTextObjTMP.text = _newCount.ToString();
            tempTextObj.GetComponent<BoxCollider2D>().size = new Vector2(tempTextObjTMP.preferredWidth, 4);
        }
        else // Off
        {
            // Dance in the emptiness
        }
    }
}


