using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [Header("Input Settings")]
    public LayerMask tileLayer = 1 << 8; // GridTile layer
    public LayerMask unitLayer = 1 << 9; // Unit layer
    public LayerMask buildingLayer = 1 << 10; // Building layer
    
    [Header("Raycasting")]
    public float maxRayDistance = 100f;
    
    // Input Actions
    private InputAction leftClickAction;
    private InputAction rightClickAction;
    private InputAction mousePositionAction;
    private InputAction endTurnAction;
    private InputAction escapeAction;
    
    // Components
    private Camera mainCamera;
    private GameManager gameManager;
    private GridManager gridManager;
    private UnitManager unitManager;
    
    // Mouse state
    private Vector2 mousePosition;
    private GridTile hoveredTile;
    private GridTile lastHoveredTile;
    
    void Awake()
    {
        // Get references
        mainCamera = Camera.main;
        gameManager = FindFirstObjectByType<GameManager>();
        gridManager = FindFirstObjectByType<GridManager>();
        unitManager = FindFirstObjectByType<UnitManager>();
        
        // Setup input actions
        SetupInputActions();
    }
    
    void SetupInputActions()
    {
        // Create input actions
        leftClickAction = new InputAction("LeftClick", InputActionType.Button, binding: "<Mouse>/leftButton");
        rightClickAction = new InputAction("RightClick", InputActionType.Button, binding: "<Mouse>/rightButton");
        mousePositionAction = new InputAction("MousePosition", InputActionType.Value, binding: "<Mouse>/position");
        endTurnAction = new InputAction("EndTurn", InputActionType.Button, binding: "<Keyboard>/space");
        escapeAction = new InputAction("Escape", InputActionType.Button, binding: "<Keyboard>/escape");
        
        // Enable actions
        leftClickAction.Enable();
        rightClickAction.Enable();
        mousePositionAction.Enable();
        endTurnAction.Enable();
        escapeAction.Enable();
        
        // Setup callbacks
        leftClickAction.performed += OnLeftClick;
        rightClickAction.performed += OnRightClick;
        mousePositionAction.performed += OnMouseMove;
        endTurnAction.performed += OnEndTurn;
        escapeAction.performed += OnEscape;
    }
    
    void Update()
    {
        HandleMouseHover();
    }
    
    void HandleMouseHover()
    {
        if (mainCamera == null) return;
        
        // Get mouse position
        mousePosition = mousePositionAction.ReadValue<Vector2>();
        
        // Create ray from camera
        Ray ray = mainCamera.ScreenPointToRay(mousePosition);
        
        // Raycast to find tiles
        if (Physics.Raycast(ray, out RaycastHit hit, maxRayDistance, tileLayer))
        {
            GridTile tile = hit.collider.GetComponent<GridTile>();
            if (tile != null)
            {
                // Update hovered tile
                if (hoveredTile != tile)
                {
                    // Clear previous hover
                    if (hoveredTile != null)
                    {
                        hoveredTile.SetHovered(false);
                    }
                    
                    // Set new hover
                    hoveredTile = tile;
                    hoveredTile.SetHovered(true);
                    
                    // Update UI with tile info
                    if (gameManager != null && gameManager.uiManager != null)
                    {
                        gameManager.uiManager.UpdateTileInfo(tile);
                    }
                }
            }
        }
        else
        {
            // No tile hovered
            if (hoveredTile != null)
            {
                hoveredTile.SetHovered(false);
                hoveredTile = null;
            }
        }
    }
    
        void OnLeftClick(InputAction.CallbackContext context)
    {
        if (mainCamera == null || gameManager == null) return;

        Debug.Log("=== LEFT CLICK DETECTED ===");

        // Get mouse position
        mousePosition = mousePositionAction.ReadValue<Vector2>();
        Debug.Log($"Mouse position: {mousePosition}");

        // Create ray from camera
        Ray ray = mainCamera.ScreenPointToRay(mousePosition);
        Debug.Log($"Ray: Origin={ray.origin}, Direction={ray.direction}");

        // Try to hit a unit first
        if (Physics.Raycast(ray, out RaycastHit unitHit, maxRayDistance, unitLayer))
        {
            Debug.Log($"Unit hit: {unitHit.collider.name} on layer {unitHit.collider.gameObject.layer}");
            Unit unit = unitHit.collider.GetComponent<Unit>();
            if (unit != null)
            {
                Debug.Log($"✓ Unit found: {unit.name}");
                gameManager.SelectUnit(unit);
                return;
            }
        }

        // Try to hit a building
        if (Physics.Raycast(ray, out RaycastHit buildingHit, maxRayDistance, buildingLayer))
        {
            Debug.Log($"Building hit: {buildingHit.collider.name} on layer {buildingHit.collider.gameObject.layer}");
            // Handle building selection
            Debug.Log("Building clicked: " + buildingHit.collider.name);
            return;
        }

        // Try to hit a tile
        Debug.Log($"Trying tile raycast with layer mask: {tileLayer.value} (layer 8 bit: {1 << 8})");
        if (Physics.Raycast(ray, out RaycastHit tileHit, maxRayDistance, tileLayer))
        {
            Debug.Log($"Tile hit: {tileHit.collider.name} on layer {tileHit.collider.gameObject.layer}");
            GridTile tile = tileHit.collider.GetComponent<GridTile>();
            if (tile != null)
            {
                Debug.Log($"✓ GridTile found: {tile.GetGridPosition()}");
                gameManager.SelectTile(tile);
                return;
            }
            else
            {
                Debug.LogWarning("Hit object has no GridTile component!");
            }
        }
        else
        {
            Debug.LogWarning("No tile hit detected");
        }

        // No valid target, clear selection
        Debug.Log("No valid target - clearing selections");
        gameManager.ClearSelections();
    }
    
    void OnRightClick(InputAction.CallbackContext context)
    {
        if (gameManager == null) return;
        
        // Right click cancels current selection
        gameManager.ClearSelections();
    }
    
    void OnMouseMove(InputAction.CallbackContext context)
    {
        mousePosition = context.ReadValue<Vector2>();
    }
    
    void OnEndTurn(InputAction.CallbackContext context)
    {
        if (gameManager == null) return;
        
        // End current player's turn
        gameManager.EndTurn();
    }
    
    void OnEscape(InputAction.CallbackContext context)
    {
        if (gameManager == null) return;
        
        // Clear selections or open pause menu
        gameManager.ClearSelections();
        
        // TODO: Open pause menu
    }
    
    public GridTile GetTileAtMousePosition()
    {
        if (mainCamera == null) return null;
        
        mousePosition = mousePositionAction.ReadValue<Vector2>();
        Ray ray = mainCamera.ScreenPointToRay(mousePosition);
        
        if (Physics.Raycast(ray, out RaycastHit hit, maxRayDistance, tileLayer))
        {
            return hit.collider.GetComponent<GridTile>();
        }
        
        return null;
    }
    
    public Unit GetUnitAtMousePosition()
    {
        if (mainCamera == null) return null;
        
        mousePosition = mousePositionAction.ReadValue<Vector2>();
        Ray ray = mainCamera.ScreenPointToRay(mousePosition);
        
        if (Physics.Raycast(ray, out RaycastHit hit, maxRayDistance, unitLayer))
        {
            return hit.collider.GetComponent<Unit>();
        }
        
        return null;
    }
    
    public Vector3 GetWorldPositionAtMouse()
    {
        if (mainCamera == null) return Vector3.zero;
        
        mousePosition = mousePositionAction.ReadValue<Vector2>();
        Ray ray = mainCamera.ScreenPointToRay(mousePosition);
        
        if (Physics.Raycast(ray, out RaycastHit hit, maxRayDistance))
        {
            return hit.point;
        }
        
        return Vector3.zero;
    }
    
    public void SetCursor(Texture2D cursorTexture, Vector2 hotspot)
    {
        Cursor.SetCursor(cursorTexture, hotspot, CursorMode.Auto);
    }
    
    public void ResetCursor()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
    
    void OnDestroy()
    {
        // Clean up input actions
        leftClickAction?.Disable();
        rightClickAction?.Disable();
        mousePositionAction?.Disable();
        endTurnAction?.Disable();
        escapeAction?.Disable();
    }
    
    public GridTile GetTileUnderCursor()
    {
        if (gridManager == null) return null;
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, tileLayer))
        {
            GridTile tile = hit.collider.GetComponent<GridTile>();
            return tile;
        }
        
        return null;
    }
    

} 