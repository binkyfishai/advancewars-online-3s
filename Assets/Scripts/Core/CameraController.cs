using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    public float orbitSpeed = 2f;
    public float zoomSpeed = 5f;
    public float panSpeed = 3f;
    public float smoothTime = 0.1f;
    
    [Header("Zoom Limits")]
    public float minZoom = 5f;
    public float maxZoom = 30f;
    
    [Header("Orbit Limits")]
    public float minVerticalAngle = 10f;
    public float maxVerticalAngle = 80f;
    
    [Header("Target Settings")]
    public Transform target;
    public Vector3 targetOffset = Vector3.zero;
    
    // Input
    private Vector2 orbitInput;
    private Vector2 panInput;
    private float zoomInput;
    private bool isOrbiting;
    private bool isPanning;
    
    // Camera state
    private float currentZoom;
    private float currentHorizontalAngle;
    private float currentVerticalAngle;
    private Vector3 currentPanPosition;
    
    // Smoothing
    private Vector3 velocity;
    private float zoomVelocity;
    private float angleVelocity;
    
    // Components
    private Camera cam;
    private InputAction orbitAction;
    private InputAction panAction;
    private InputAction zoomAction;
    private InputAction orbitHoldAction;
    private InputAction panHoldAction;
    
    void Awake()
    {
        cam = GetComponent<Camera>();
        
        // Initialize camera state
        currentZoom = Vector3.Distance(transform.position, target ? target.position : Vector3.zero);
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);
        
        Vector3 direction = (transform.position - (target ? target.position : Vector3.zero)).normalized;
        currentHorizontalAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        currentVerticalAngle = Mathf.Asin(direction.y) * Mathf.Rad2Deg;
        
        currentPanPosition = target ? target.position : Vector3.zero;
        
        // Setup input actions
        SetupInputActions();
    }
    
    void SetupInputActions()
    {
        var inputActions = new InputActionAsset();
        
        // Create orbit action
        orbitAction = new InputAction("Orbit", InputActionType.Value, binding: "<Mouse>/delta");
        orbitHoldAction = new InputAction("OrbitHold", InputActionType.Button, binding: "<Mouse>/rightButton");
        
        // Create pan action
        panAction = new InputAction("Pan", InputActionType.Value, binding: "<Mouse>/delta");
        panHoldAction = new InputAction("PanHold", InputActionType.Button, binding: "<Mouse>/middleButton");
        
        // Create zoom action
        zoomAction = new InputAction("Zoom", InputActionType.Value, binding: "<Mouse>/scroll/y");
        
        // Enable actions
        orbitAction.Enable();
        orbitHoldAction.Enable();
        panAction.Enable();
        panHoldAction.Enable();
        zoomAction.Enable();
        
        // Setup callbacks
        orbitAction.performed += OnOrbitInput;
        orbitAction.canceled += OnOrbitInput;
        
        panAction.performed += OnPanInput;
        panAction.canceled += OnPanInput;
        
        zoomAction.performed += OnZoomInput;
        zoomAction.canceled += OnZoomInput;
        
        orbitHoldAction.performed += OnOrbitHold;
        orbitHoldAction.canceled += OnOrbitHold;
        
        panHoldAction.performed += OnPanHold;
        panHoldAction.canceled += OnPanHold;
    }
    
    void OnOrbitInput(InputAction.CallbackContext context)
    {
        orbitInput = context.ReadValue<Vector2>();
    }
    
    void OnPanInput(InputAction.CallbackContext context)
    {
        panInput = context.ReadValue<Vector2>();
    }
    
    void OnZoomInput(InputAction.CallbackContext context)
    {
        zoomInput = context.ReadValue<float>();
    }
    
    void OnOrbitHold(InputAction.CallbackContext context)
    {
        isOrbiting = context.ReadValueAsButton();
    }
    
    void OnPanHold(InputAction.CallbackContext context)
    {
        isPanning = context.ReadValueAsButton();
    }
    
    void Update()
    {
        HandleInput();
        UpdateCamera();
    }
    
    void HandleInput()
    {
        // Handle orbit input
        if (isOrbiting && orbitInput.magnitude > 0.1f)
        {
            currentHorizontalAngle += orbitInput.x * orbitSpeed;
            currentVerticalAngle -= orbitInput.y * orbitSpeed;
            currentVerticalAngle = Mathf.Clamp(currentVerticalAngle, minVerticalAngle, maxVerticalAngle);
        }
        
        // Handle pan input
        if (isPanning && panInput.magnitude > 0.1f)
        {
            Vector3 right = transform.right;
            Vector3 up = Vector3.up;
            
            Vector3 panMovement = (-right * panInput.x + up * panInput.y) * panSpeed * Time.deltaTime;
            currentPanPosition += panMovement;
        }
        
        // Handle zoom input
        if (Mathf.Abs(zoomInput) > 0.1f)
        {
            currentZoom -= zoomInput * zoomSpeed * Time.deltaTime;
            currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);
        }
    }
    
    void UpdateCamera()
    {
        // Calculate target position
        Vector3 targetPosition = (target ? target.position : Vector3.zero) + targetOffset + currentPanPosition;
        
        // Calculate camera position based on angles and zoom
        float horizontalRadians = currentHorizontalAngle * Mathf.Deg2Rad;
        float verticalRadians = currentVerticalAngle * Mathf.Deg2Rad;
        
        Vector3 direction = new Vector3(
            Mathf.Sin(horizontalRadians) * Mathf.Cos(verticalRadians),
            Mathf.Sin(verticalRadians),
            Mathf.Cos(horizontalRadians) * Mathf.Cos(verticalRadians)
        );
        
        Vector3 desiredPosition = targetPosition + direction * currentZoom;
        
        // Smooth camera movement
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);
        
        // Look at target
        transform.LookAt(targetPosition);
    }
    
    public void FocusOnPosition(Vector3 position)
    {
        currentPanPosition = position - (target ? target.position : Vector3.zero);
    }
    
    public void FocusOnTile(GridTile tile)
    {
        if (tile != null)
        {
            FocusOnPosition(tile.worldPosition);
        }
    }
    
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        if (target != null)
        {
            currentPanPosition = Vector3.zero;
        }
    }
    
    public void ResetCamera()
    {
        currentPanPosition = Vector3.zero;
        currentZoom = (minZoom + maxZoom) / 2f;
        currentHorizontalAngle = 0f;
        currentVerticalAngle = 45f;
    }
    
    void OnDestroy()
    {
        // Clean up input actions
        orbitAction?.Disable();
        orbitHoldAction?.Disable();
        panAction?.Disable();
        panHoldAction?.Disable();
        zoomAction?.Disable();
    }
} 