using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))] // Ensure world space objects have a collider
public class DragToExit : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Exit Settings")]
    [SerializeField] private float exitThreshold = 0.1f; // How close to top-right corner to trigger exit (0-1)
    [SerializeField] private float snapBackSpeed = 10f;
    [SerializeField] private bool requireHoldInZone = false; // If true, must hold in exit zone briefly
    [SerializeField] private float holdTime = 0.3f; // Time to hold in exit zone

    [Header("Visual Feedback")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color draggingColor = Color.yellow;
    [SerializeField] private Color exitZoneColor = Color.green;
    [SerializeField] private bool showTargetIndicator = true;
    [SerializeField] private Color targetIndicatorColor = Color.white;
    [SerializeField] private float targetIndicatorAlpha = 0.5f;

    [Header("Ghost Guide Settings")]
    [SerializeField] private bool showGhostGuide = true;
    [SerializeField] private Color ghostColor = Color.white;
    [SerializeField] private float ghostAlpha = 0.3f;
    [SerializeField] private float ghostSpeed = 0.5f; // Speed of ghost animation (reduced to 1/4)
    [SerializeField] private int maxGhostLoops = 3; // Maximum number of loops

    // Target indicator
    private GameObject targetIndicator;
    private SpriteRenderer targetSpriteRenderer;
    private UnityEngine.UI.Image targetUIImage;

    // Ghost guide
    private GameObject ghostGuide;
    private SpriteRenderer ghostSpriteRenderer;
    private UnityEngine.UI.Image ghostUIImage;
    private bool isGhostAnimating = false;
    private float ghostAnimationTimer = 0f;
    private int currentGhostLoops = 0;

    private Vector3 startPosition;
    private Vector3 targetExitPosition;
    private bool isDragging = false;
    private bool isSnappingBack = false;
    private float holdTimer = 0f;
    private bool inExitZone = false;

    private Camera mainCamera;
    private RectTransform rectTransform;
    private SpriteRenderer spriteRenderer;
    private UnityEngine.UI.Image uiImage;

    GameBackButton _gameBackButton;
    Spawner _spawner;

    // Events
    public System.Action OnExitTriggered;

    void Start()
    {
        mainCamera = Camera.main;
        spriteRenderer = GetComponent<SpriteRenderer>();
        uiImage = GetComponent<UnityEngine.UI.Image>();

        SetupPositions();
        CreateTargetIndicator();
        CreateGhostGuide();
        SetColor(normalColor);

        _gameBackButton = GameObject.FindGameObjectWithTag("GameBackButton").GetComponent<GameBackButton>();
        _spawner = GameObject.FindGameObjectWithTag("spawner").GetComponent<Spawner>();
        _spawner.OnShapesPopped.AddListener(HideExitButton);
        OnExitTriggered += QuitLesson;
    }

    private void OnDestroy()
    {
        OnExitTriggered -= QuitLesson;
        _spawner.OnShapesPopped.RemoveListener(HideExitButton);
    }

    private void QuitLesson()
    {
        if (SceneManager.GetActiveScene().name == "FunModeGameScene2")
        {
            ResetPosition();
            _spawner.ResetFunMode();

        }
        else if (SceneManager.GetActiveScene().name == "LessonsScene")
        {
            ResetPosition();
            _spawner.LeaveLessons();
        }
    }

    void ShowTargetIndicator(bool show)
    {
        if (targetIndicator != null)
        {
            targetIndicator.SetActive(show);
        }
    }

    void ShowGhostGuide(bool show)
    {
        if (ghostGuide != null)
        {
            // If we're showing the guide and it was previously hidden, reset everything
            bool wasHidden = !ghostGuide.activeSelf;

            ghostGuide.SetActive(show);
            isGhostAnimating = show;

            if (show && wasHidden)
            {
                // Only reset when we're truly starting fresh (guide was hidden)
                ghostAnimationTimer = 0f;
                currentGhostLoops = 0;
                ghostGuide.transform.position = startPosition;
            }
            else if (show && currentGhostLoops < maxGhostLoops)
            {
                // If guide was already showing but hasn't completed max loops, just reset timer
                ghostAnimationTimer = 0f;
                ghostGuide.transform.position = startPosition;
            }
        }
    }

    // Update target indicator position if screen size changes
    void UpdateTargetIndicatorPosition()
    {
        if (targetIndicator != null)
        {
            targetIndicator.transform.position = targetExitPosition;

            // Update UI RectTransform position if it's a UI element
            if (targetUIImage != null)
            {
                RectTransform targetRect = targetIndicator.GetComponent<RectTransform>();
                if (targetRect != null)
                {
                    targetRect.anchoredPosition = targetExitPosition;
                }
            }
        }
    }

    // Call this to change target indicator appearance
    public void SetTargetIndicatorColor(Color color, float alpha = -1f)
    {
        if (alpha >= 0f) targetIndicatorAlpha = alpha;

        Color targetColor = color;
        targetColor.a = targetIndicatorAlpha;

        if (targetSpriteRenderer != null)
        {
            targetSpriteRenderer.color = targetColor;
        }
        else if (targetUIImage != null)
        {
            targetUIImage.color = targetColor;
        }
    }

    void SetupPositions()
    {
        // Use a consistent Z distance from camera for screen-to-world conversion
        float zDistance = Mathf.Abs(transform.position.z - mainCamera.transform.position.z);
        if (zDistance < 0.1f) zDistance = 10f; // Default distance if too close

        Vector3 topLeft = mainCamera.ScreenToWorldPoint(new Vector3(0, Screen.height, zDistance));
        Vector3 topRight = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, zDistance));

        // Get object bounds to offset from edges
        Bounds objectBounds = GetObjectBounds();
        float halfWidth = objectBounds.size.x / 2f;
        float halfHeight = objectBounds.size.y / 2f;

        // Add margin from screen edges (adjust this value as needed)
        float margin = 0.5f; // World units - adjust based on your camera setup

        // Offset positions so object appears fully on screen with margin
        startPosition = new Vector3(topLeft.x + halfWidth + margin, topLeft.y - halfHeight - margin, transform.position.z);
        targetExitPosition = new Vector3(topRight.x - halfWidth - margin, topRight.y - halfHeight - margin, transform.position.z);

        transform.position = startPosition;
    }

    Bounds GetObjectBounds()
    {
        if (spriteRenderer != null)
        {
            return spriteRenderer.bounds;
        }
        else if (uiImage != null)
        {
            RectTransform rect = GetComponent<RectTransform>();
            Vector3 size = rect.rect.size;
            size = Vector3.Scale(size, rect.lossyScale);
            return new Bounds(rect.position, size);
        }

        // Fallback if no renderer found
        return new Bounds(transform.position, Vector3.one);
    }

    void CreateTargetIndicator()
    {
        if (!showTargetIndicator) return;

        // Create target indicator as child object
        targetIndicator = new GameObject("TargetIndicator");
        targetIndicator.transform.SetParent(transform.parent); // Same parent as draggable object
        targetIndicator.transform.position = targetExitPosition;

        // Copy visual components
        if (spriteRenderer != null)
        {
            // Copy SpriteRenderer
            targetSpriteRenderer = targetIndicator.AddComponent<SpriteRenderer>();
            targetSpriteRenderer.sprite = spriteRenderer.sprite;
            targetSpriteRenderer.sortingLayerName = spriteRenderer.sortingLayerName;
            targetSpriteRenderer.sortingOrder = spriteRenderer.sortingOrder - 1; // Behind the draggable

            // Set target color with alpha
            Color targetColor = targetIndicatorColor;
            targetColor.a = targetIndicatorAlpha;
            targetSpriteRenderer.color = targetColor;

            // Match scale and rotation
            targetIndicator.transform.localScale = transform.localScale;
            targetIndicator.transform.rotation = transform.rotation;
        }
        else if (uiImage != null)
        {
            // Copy UI Image
            targetUIImage = targetIndicator.AddComponent<UnityEngine.UI.Image>();
            targetUIImage.sprite = uiImage.sprite;
            targetUIImage.type = uiImage.type;
            targetUIImage.preserveAspect = uiImage.preserveAspect;

            // Set target color with alpha
            Color targetColor = targetIndicatorColor;
            targetColor.a = targetIndicatorAlpha;
            targetUIImage.color = targetColor;

            // Copy RectTransform properties if UI
            RectTransform targetRect = targetIndicator.GetComponent<RectTransform>();
            RectTransform sourceRect = GetComponent<RectTransform>();
            if (sourceRect != null && targetRect != null)
            {
                targetRect.sizeDelta = sourceRect.sizeDelta;
                targetRect.anchoredPosition = targetExitPosition;
                targetRect.localScale = sourceRect.localScale;
                targetRect.rotation = sourceRect.rotation;
            }
        }

        ShowTargetIndicator(false);
    }

    void CreateGhostGuide()
    {
        if (!showGhostGuide) return;

        // Create ghost guide as child object
        ghostGuide = new GameObject("GhostGuide");
        ghostGuide.transform.SetParent(transform.parent); // Same parent as draggable object
        ghostGuide.transform.position = startPosition;

        // Copy visual components
        if (spriteRenderer != null)
        {
            // Copy SpriteRenderer
            ghostSpriteRenderer = ghostGuide.AddComponent<SpriteRenderer>();
            ghostSpriteRenderer.sprite = spriteRenderer.sprite;
            ghostSpriteRenderer.sortingLayerName = spriteRenderer.sortingLayerName;
            ghostSpriteRenderer.sortingOrder = spriteRenderer.sortingOrder - 2; // Behind both target and draggable

            // Set ghost color with alpha
            Color ghostColorWithAlpha = ghostColor;
            ghostColorWithAlpha.a = ghostAlpha;
            ghostSpriteRenderer.color = ghostColorWithAlpha;

            // Match scale and rotation
            ghostGuide.transform.localScale = transform.localScale;
            ghostGuide.transform.rotation = transform.rotation;
        }
        else if (uiImage != null)
        {
            // Copy UI Image
            ghostUIImage = ghostGuide.AddComponent<UnityEngine.UI.Image>();
            ghostUIImage.sprite = uiImage.sprite;
            ghostUIImage.type = uiImage.type;
            ghostUIImage.preserveAspect = uiImage.preserveAspect;

            // Set ghost color with alpha
            Color ghostColorWithAlpha = ghostColor;
            ghostColorWithAlpha.a = ghostAlpha;
            ghostUIImage.color = ghostColorWithAlpha;

            // Copy RectTransform properties if UI
            RectTransform ghostRect = ghostGuide.GetComponent<RectTransform>();
            RectTransform sourceRect = GetComponent<RectTransform>();
            if (sourceRect != null && ghostRect != null)
            {
                ghostRect.sizeDelta = sourceRect.sizeDelta;
                ghostRect.anchoredPosition = startPosition;
                ghostRect.localScale = sourceRect.localScale;
                ghostRect.rotation = sourceRect.rotation;
            }
        }

        ShowGhostGuide(false);
    }

    void Update()
    {
        if (isSnappingBack)
        {
            SnapBackToStart();
        }

        if (isDragging && requireHoldInZone && inExitZone)
        {
            holdTimer += Time.deltaTime;
            if (holdTimer >= holdTime)
            {
                TriggerExit();
            }
        }

        // Animate ghost guide
        if (isGhostAnimating && ghostGuide != null)
        {
            AnimateGhostGuide();
        }
    }

    void AnimateGhostGuide()
    {
        // Stop animation after max loops
        if (currentGhostLoops >= maxGhostLoops)
        {
            ShowGhostGuide(false);
            return;
        }

        ghostAnimationTimer += Time.deltaTime * ghostSpeed;

        // Calculate position along the path from start to target
        float t = ghostAnimationTimer % 1f; // Use modulo to create a sawtooth wave (0 to 1, then jump back to 0)

        // Check if we completed a loop by seeing if we've passed each whole number
        int completedLoops = Mathf.FloorToInt(ghostAnimationTimer);
        if (completedLoops > currentGhostLoops)
        {
            currentGhostLoops = completedLoops;
        }

        Vector3 currentGhostPos = Vector3.Lerp(startPosition, targetExitPosition, t);

        ghostGuide.transform.position = currentGhostPos;

        // Update UI RectTransform position if it's a UI element
        if (ghostUIImage != null)
        {
            RectTransform ghostRect = ghostGuide.GetComponent<RectTransform>();
            if (ghostRect != null)
            {
                ghostRect.anchoredPosition = currentGhostPos;
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Show target indicator and ghost guide when user touches/clicks
        SetColor(draggingColor);
        ShowTargetIndicator(true);
        ShowGhostGuide(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Hide guides if user lifts finger without dragging
        if (!isDragging)
        {
            SetColor(normalColor);
            ShowTargetIndicator(false);
            ShowGhostGuide(false);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        isSnappingBack = false;
        holdTimer = 0f;
        inExitZone = false;
        SetColor(draggingColor);

        // Guides are already shown from OnPointerDown, just ensure they're visible
        // Don't restart ghost animation if it already completed max loops
        ShowTargetIndicator(true);
        if (currentGhostLoops < maxGhostLoops)
        {
            ShowGhostGuide(true);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        // Use the same Z distance calculation as SetupPositions
        float zDistance = Mathf.Abs(transform.position.z - mainCamera.transform.position.z);
        if (zDistance < 0.1f) zDistance = 10f;

        Vector3 screenPosition = new Vector3(eventData.position.x, eventData.position.y, zDistance);
        Vector3 newPosition = mainCamera.ScreenToWorldPoint(screenPosition);

        // Lock vertical movement - keep Y position same as start position
        newPosition.y = startPosition.y;
        newPosition.z = transform.position.z; // Keep original Z position

        transform.position = newPosition;

        // Check if in exit zone
        CheckExitZone();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;

        if (IsInExitZone())
        {
            if (!requireHoldInZone)
            {
                TriggerExit();
            }
            else if (holdTimer >= holdTime)
            {
                TriggerExit();
            }
            else
            {
                StartSnapBack();
            }
        }
        else
        {
            StartSnapBack();
        }
    }

    void CheckExitZone()
    {
        bool wasInZone = inExitZone;
        inExitZone = IsInExitZone();

        if (inExitZone && !wasInZone)
        {
            holdTimer = 0f;
            SetColor(exitZoneColor);
        }
        else if (!inExitZone && wasInZone)
        {
            holdTimer = 0f;
            SetColor(draggingColor);
        }
    }

    bool IsInExitZone()
    {
        Vector3 currentPos = transform.position;
        float distance = Vector3.Distance(currentPos, targetExitPosition);

        // Calculate threshold distance based on screen size
        float maxDistance = Vector3.Distance(startPosition, targetExitPosition);
        float thresholdDistance = maxDistance * exitThreshold;

        return distance <= thresholdDistance;
    }

    void StartSnapBack()
    {
        isSnappingBack = true;
        SetColor(normalColor);
        holdTimer = 0f;
        inExitZone = false;

        // Hide target indicator and ghost guide when snapping back
        ShowTargetIndicator(false);
        ShowGhostGuide(false);
    }

    void SnapBackToStart()
    {
        Vector3 currentPos = transform.position;
        Vector3 newPos = Vector3.MoveTowards(currentPos, startPosition, snapBackSpeed * Time.deltaTime * Vector3.Distance(startPosition, targetExitPosition));

        transform.position = newPos;

        if (Vector3.Distance(newPos, startPosition) < 0.01f)
        {
            transform.position = startPosition;
            isSnappingBack = false;
        }
    }

    void TriggerExit()
    {
        // Hide target indicator and ghost guide on successful exit
        ResetPositionInstant();
        ShowTargetIndicator(false);
        ShowGhostGuide(false);
        gameObject.SetActive(false);

        OnExitTriggered?.Invoke();
        // You can add scene transition logic here
        //Debug.Log("Exit triggered!");

        // Example: Load previous scene or trigger your existing exit system
        // SceneManager.LoadScene("PreviousScene");
    }

    private void HideExitButton()
    {
        ResetPositionInstant();
        ShowTargetIndicator(false);
        ShowGhostGuide(false);
        gameObject.SetActive(false);
    }

    void SetColor(Color color)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = color;
        }
        else if (uiImage != null)
        {
            uiImage.color = color;
        }
    }

    // Call this if screen orientation changes
    public void OnScreenSizeChanged()
    {
        SetupPositions();
        UpdateTargetIndicatorPosition();
    }

    // Reset to start position (useful for scene resets)
    public void ResetPosition()
    {
        isDragging = false;
        isSnappingBack = false;
        holdTimer = 0f;
        inExitZone = false;

        transform.position = startPosition;

        SetColor(normalColor);
    }

    // Instantly reset position without animation - call this before setting active false
    public void ResetPositionInstant()
    {
        isDragging = false;
        isSnappingBack = false;
        holdTimer = 0f;
        inExitZone = false;

        transform.position = startPosition;
        SetColor(normalColor);

        // Hide target indicator and ghost guide
        ShowTargetIndicator(false);
        ShowGhostGuide(false);
    }
}