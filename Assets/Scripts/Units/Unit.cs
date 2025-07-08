using System.Collections;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [Header("Unit Data")]
    public UnitData unitData;
    
    [Header("Current State")]
    public int owner;
    public int currentHP;
    public int currentFuel;
    public int currentAmmo;
    public bool hasMoved = false;
    public bool hasAttacked = false;
    
    [Header("Position")]
    public Vector2Int gridPosition;
    public GridTile currentTile;
    
    [Header("Visual")]
    public GameObject unitModel;
    public Animator animator;
    public AudioSource audioSource;
    
    // Movement
    private bool isMoving = false;
    private Vector3 targetPosition;
    private float moveSpeed = 5f;
    
    // References
    private GridManager gridManager;
    private UnitManager unitManager;
    
    // Events
    public System.Action<Unit> OnUnitDestroyed;
    public System.Action<Unit> OnUnitDamaged;
    public System.Action<Unit> OnUnitMoved;
    
    void Awake()
    {
        // Get references
        gridManager = GridManager.Instance;
        unitManager = UnitManager.Instance;
        
        // Get components
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
        
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
        
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }
    
    void Start()
    {
        // Initialize unit stats
        InitializeUnit();
    }
    
    void Update()
    {
        // Handle movement animation
        if (isMoving)
        {
            UpdateMovement();
        }
    }
    
    public void InitializeUnit()
    {
        if (unitData == null)
        {
            Debug.LogError($"Unit {name} has no UnitData assigned!");
            return;
        }
        
        // Initialize stats
        currentHP = unitData.maxHP;
        currentFuel = unitData.fuel;
        currentAmmo = unitData.ammo;
        
                // Set up animator
        if (animator != null && unitData.animatorController != null)
        {
            animator.runtimeAnimatorController = unitData.animatorController;
        }

        // Find unit model (it should already be loaded by UnitManager)
        if (unitModel == null)
        {
            unitModel = GetComponentInChildren<Transform>().gameObject;
            if (unitModel == transform.gameObject)
            {
                unitModel = null; // Don't use the root gameobject as the model
            }
        }
        
        // Set name
        name = $"{unitData.unitName}_{owner}";
        
        Debug.Log($"Unit initialized: {name} - HP: {currentHP}, Fuel: {currentFuel}, Ammo: {currentAmmo}");
    }
    
    // Movement
    public void SetGridPosition(Vector2Int newPosition)
    {
        Debug.Log($"=== SET GRID POSITION: {name} ===");
        Debug.Log($"Old position: {gridPosition}");
        Debug.Log($"New position: {newPosition}");
        Debug.Log($"GridManager: {(gridManager != null ? "Found" : "NULL")}");
        
        gridPosition = newPosition;
        
        // Update tile references
        if (currentTile != null)
        {
            Debug.Log($"Clearing old tile occupancy at {currentTile.GetGridPosition()}");
            currentTile.ClearOccupyingUnit();
        }
        
        if (gridManager != null)
        {
            currentTile = gridManager.GetTile(gridPosition);
            Debug.Log($"Got tile at {gridPosition}: {(currentTile != null ? "Found" : "NULL")}");
            if (currentTile != null)
            {
                Debug.Log($"Setting occupying unit on tile {currentTile.GetGridPosition()}");
                currentTile.SetOccupyingUnit(this);
                Debug.Log($"✓ Tile occupancy set. Verify: {(currentTile.occupyingUnit != null ? currentTile.occupyingUnit.name : "NULL")}");
            }
            else
            {
                Debug.LogError($"Could not find tile at position {gridPosition}!");
            }
        }
        else
        {
            Debug.LogError($"GridManager is null for unit {name}!");
        }
    }
    
    public void SetWorldPosition(Vector3 worldPosition)
    {
        transform.position = worldPosition;
        
        // Update grid position
        if (gridManager != null)
        {
            gridPosition = gridManager.WorldToGrid(worldPosition);
            SetGridPosition(gridPosition);
        }
    }
    
    public IEnumerator MoveTo(Vector2Int targetGrid)
    {
        Debug.Log($"=== UNIT MOVE TO COROUTINE START ===");
        Debug.Log($"Unit: {name}");
        Debug.Log($"Current pos: {gridPosition}");
        Debug.Log($"Target pos: {targetGrid}");
        Debug.Log($"GridManager: {(gridManager != null ? "Found" : "NULL")}");
        
        if (gridManager == null) 
        {
            Debug.LogError($"GridManager is null for unit {name}!");
            yield break;
        }
        
        // Calculate world position
        Vector3 worldTarget = gridManager.GridToWorld(targetGrid);
        Debug.Log($"World target: {worldTarget}");
        
        // Start movement animation
        Debug.Log($"Starting movement animation...");
        yield return StartCoroutine(MoveToWorldPosition(worldTarget));
        Debug.Log($"Movement animation completed");
        
        // Update grid position
        Debug.Log($"Updating grid position from {gridPosition} to {targetGrid}");
        SetGridPosition(targetGrid);
        
        // Mark as moved
        hasMoved = true;
        Debug.Log($"Unit marked as moved");
        
        // Consume fuel
        if (currentTile != null)
        {
            TerrainTile terrainTile = currentTile.GetComponent<TerrainTile>();
            int fuelCost = terrainTile != null ? 
                terrainTile.GetMovementCost(this) : 
                unitData.GetMovementCost(TerrainType.Plain);
            currentFuel = Mathf.Max(0, currentFuel - fuelCost);
            Debug.Log($"Fuel consumed: {fuelCost}, remaining: {currentFuel}");
        }
        
        // Play move sound
        if (audioSource != null && unitData.moveSound != null)
        {
            audioSource.PlayOneShot(unitData.moveSound);
        }
        
        // Trigger event
        OnUnitMoved?.Invoke(this);
        
        Debug.Log($"✓ {name} successfully moved to {targetGrid}");
    }
    
    public IEnumerator MoveToWorldPosition(Vector3 worldTarget)
    {
        isMoving = true;
        targetPosition = worldTarget;
        
        Vector3 startPos = transform.position;
        float distance = Vector3.Distance(startPos, targetPosition);
        float duration = distance / moveSpeed;
        float elapsed = 0f;
        
        // Set animation state
        if (animator != null)
        {
            animator.SetBool("IsMoving", true);
        }
        
        // Animate movement
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            // Smooth movement curve
            t = Mathf.SmoothStep(0f, 1f, t);
            
            transform.position = Vector3.Lerp(startPos, targetPosition, t);
            
            yield return null;
        }
        
        // Ensure final position
        transform.position = targetPosition;
        
        // Stop animation
        if (animator != null)
        {
            animator.SetBool("IsMoving", false);
        }
        
        isMoving = false;
    }
    
    void UpdateMovement()
    {
        if (!isMoving) return;
        
        // This is handled by the coroutine, but we keep this for any additional logic
        // during movement if needed
    }
    
    // Combat
    public void TakeDamage(int damage)
    {
        currentHP = Mathf.Max(0, currentHP - damage);
        
        // Play damage animation
        if (animator != null)
        {
            animator.SetTrigger("TakeDamage");
        }
        
        // Trigger event
        OnUnitDamaged?.Invoke(this);
        
        Debug.Log($"{name} took {damage} damage. HP: {currentHP}/{unitData.maxHP}");
        
        // Check if destroyed
        if (currentHP <= 0)
        {
            DestroyUnit();
        }
    }
    
    public void AttackUnit(Unit target)
    {
        if (target == null) return;
        
        // Calculate damage
        int damage = CombatCalculator.CalculateDamage(this, target);
        
        // Play attack animation
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }
        
        // Play attack sound
        if (audioSource != null && unitData.attackSound != null)
        {
            audioSource.PlayOneShot(unitData.attackSound);
        }
        
        // Apply damage
        target.TakeDamage(damage);
        
        // Consume ammo
        if (unitData.GetAttackDamage(target.unitData.unitType) > 0)
        {
            currentAmmo = Mathf.Max(0, currentAmmo - 1);
        }
        
        // Mark as attacked
        hasAttacked = true;
        
        Debug.Log($"{name} attacked {target.name} for {damage} damage");
    }
    
    public void DestroyUnit()
    {
        // Play destroy sound
        if (audioSource != null && unitData.destroySound != null)
        {
            audioSource.PlayOneShot(unitData.destroySound);
        }
        
        // Play destroy animation
        if (animator != null)
        {
            animator.SetTrigger("Destroy");
        }
        
        // Clear tile occupancy
        if (currentTile != null)
        {
            currentTile.ClearOccupyingUnit();
        }
        
        // Trigger event
        OnUnitDestroyed?.Invoke(this);
        
        Debug.Log($"{name} destroyed!");
        
        // Remove from unit manager
        if (unitManager != null)
        {
            unitManager.RemoveUnit(this);
        }
        
        // Destroy game object (with delay for animation)
        Destroy(gameObject, 2f);
    }
    
    // Turn management
    public void ResetTurn()
    {
        hasMoved = false;
        hasAttacked = false;
        
        // Reset visual state
        SetGrayout(false);
        
        Debug.Log($"{name} turn reset");
    }
    
    public void SetGrayout(bool grayout)
    {
        // Make unit appear grayed out when done for the turn
        if (unitModel != null)
        {
            MeshRenderer[] renderers = unitModel.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer renderer in renderers)
            {
                if (grayout)
                {
                    renderer.material.color = Color.gray;
                }
                else
                {
                    renderer.material.color = Color.white;
                }
            }
        }
    }
    
    // Unit capabilities
    public bool CanMove()
    {
        return !hasMoved && currentFuel > 0;
    }
    
    public bool CanAttack()
    {
        return !hasAttacked && currentAmmo > 0;
    }
    
    public bool CanCapture()
    {
        return unitData.canCapture && currentTile != null && currentTile.building != null;
    }
    
    public bool IsActionComplete()
    {
        return hasMoved && hasAttacked;
    }
    
    // Utility
    public float GetHealthPercentage()
    {
        return (float)currentHP / unitData.maxHP;
    }
    
    public float GetFuelPercentage()
    {
        return (float)currentFuel / unitData.fuel;
    }
    
    public float GetAmmoPercentage()
    {
        return (float)currentAmmo / unitData.ammo;
    }
    
    public override string ToString()
    {
        return $"{unitData.unitName} (Player {owner}) - HP: {currentHP}/{unitData.maxHP} - Pos: {gridPosition}";
    }
    
    // Gizmos for debugging
    void OnDrawGizmos()
    {
        // Draw unit position
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
        
        // Draw health bar
        if (unitData != null && currentHP > 0)
        {
            float healthPercent = GetHealthPercentage();
            Vector3 healthBarPos = transform.position + Vector3.up * 2f;
            
            Gizmos.color = Color.red;
            Gizmos.DrawLine(healthBarPos, healthBarPos + Vector3.right * (1f - healthPercent));
            
            Gizmos.color = Color.green;
            Gizmos.DrawLine(healthBarPos, healthBarPos + Vector3.right * healthPercent);
        }
    }
} 