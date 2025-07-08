using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    [Header("AI Settings")]
    [SerializeField] private List<int> aiPlayerIDs = new List<int> { 1 }; // Player IDs controlled by AI
    [SerializeField] private float actionDelay = 1.0f; // Delay between AI actions for visual clarity
    [SerializeField] private float turnEndDelay = 2.0f; // Delay before ending AI turn
    
    [Header("AI Behavior")]
    [SerializeField] private float aggressiveness = 0.7f; // How likely to attack vs defend (0-1)
    [SerializeField] private float exploreFactor = 0.3f; // How much AI explores vs focuses on enemies
    
    // References
    private GameManager gameManager;
    private UnitManager unitManager;
    private GridManager gridManager;
    
    // AI State
    private bool isProcessingAITurn = false;
    private Queue<AIAction> actionQueue = new Queue<AIAction>();
    
    // Singleton
    public static AIController Instance { get; private set; }
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        // Get references
        gameManager = GameManager.Instance;
        unitManager = UnitManager.Instance;
        gridManager = GridManager.Instance;
        
        if (gameManager == null)
        {
            Debug.LogError("AIController: GameManager not found!");
            return;
        }
        
        Debug.Log($"AIController initialized. Controlling players: {string.Join(", ", aiPlayerIDs)}");
    }
    
    void Update()
    {
        // Check if it's an AI player's turn
        if (gameManager != null && IsAIPlayerTurn() && !isProcessingAITurn)
        {
            Debug.Log($"🤖 AI Player {gameManager.currentPlayer} turn starting...");
            StartCoroutine(ProcessAITurn());
        }
    }
    
    bool IsAIPlayerTurn()
    {
        return aiPlayerIDs.Contains(gameManager.currentPlayer);
    }
    
    IEnumerator ProcessAITurn()
    {
        isProcessingAITurn = true;
        
        Debug.Log($"🤖 AI Processing turn for Player {gameManager.currentPlayer}");
        
        // Get all AI units
        List<Unit> aiUnits = unitManager.GetUnitsForPlayer(gameManager.currentPlayer);
        Debug.Log($"🤖 AI has {aiUnits.Count} units to command");
        
        // Plan actions for all units
        PlanUnitActions(aiUnits);
        
        // Execute all planned actions
        while (actionQueue.Count > 0)
        {
            AIAction action = actionQueue.Dequeue();
            yield return StartCoroutine(ExecuteAction(action));
            yield return new WaitForSeconds(actionDelay);
        }
        
        // End AI turn after delay
        yield return new WaitForSeconds(turnEndDelay);
        
        Debug.Log($"🤖 AI Player {gameManager.currentPlayer} ending turn");
        gameManager.EndTurn();
        
        isProcessingAITurn = false;
    }
    
    void PlanUnitActions(List<Unit> aiUnits)
    {
        Debug.Log($"🤖 Planning actions for {aiUnits.Count} AI units");
        
        foreach (Unit unit in aiUnits)
        {
            if (unit == null) continue;
            
            AIAction bestAction = DecideBestAction(unit);
            if (bestAction != null)
            {
                actionQueue.Enqueue(bestAction);
                Debug.Log($"🤖 Planned action for {unit.name}: {bestAction.actionType}");
            }
        }
        
        Debug.Log($"🤖 Total planned actions: {actionQueue.Count}");
    }
    
    AIAction DecideBestAction(Unit unit)
    {
        if (unit == null) return null;
        
        Debug.Log($"🤖 Deciding action for {unit.name} at {unit.gridPosition}");
        
        // Find all enemy units
        List<Unit> enemyUnits = GetEnemyUnits(unit);
        
        // Priority 1: Attack if possible
        if (unit.CanAttack())
        {
            Unit targetToAttack = FindBestAttackTarget(unit, enemyUnits);
            if (targetToAttack != null)
            {
                Debug.Log($"🤖 {unit.name} will attack {targetToAttack.name}");
                return new AIAction(AIActionType.Attack, unit, targetToAttack.currentTile, targetToAttack);
            }
        }
        
        // Priority 2: Move to attack position
        if (unit.CanMove())
        {
            GridTile bestMoveTarget = FindBestMoveTarget(unit, enemyUnits);
            if (bestMoveTarget != null && bestMoveTarget != unit.currentTile)
            {
                Debug.Log($"🤖 {unit.name} will move to {bestMoveTarget.GetGridPosition()}");
                return new AIAction(AIActionType.Move, unit, bestMoveTarget);
            }
        }
        
        // Priority 3: Wait/Defend
        Debug.Log($"🤖 {unit.name} will wait");
        return new AIAction(AIActionType.Wait, unit, unit.currentTile);
    }
    
    Unit FindBestAttackTarget(Unit attacker, List<Unit> enemyUnits)
    {
        Unit bestTarget = null;
        float bestScore = -1f;
        
        foreach (Unit enemy in enemyUnits)
        {
            if (enemy == null) continue;
            
            // Check if enemy is in attack range
            if (CombatCalculator.IsInAttackRange(attacker, enemy))
            {
                float score = EvaluateAttackTarget(attacker, enemy);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestTarget = enemy;
                }
            }
        }
        
        return bestTarget;
    }
    
    float EvaluateAttackTarget(Unit attacker, Unit target)
    {
        // Calculate potential damage
        CombatResult combatResult = CombatCalculator.CalculateCombatResult(attacker, target);
        
        float score = 0f;
        
        // Prefer targets that can be destroyed
        if (combatResult.defenderFinalHP <= 0)
        {
            score += 100f; // High priority for kills
        }
        
        // Prefer high damage attacks
        score += combatResult.attackerDamage * 2f;
        
        // Avoid attacks where we take heavy counter-damage
        if (combatResult.defenderDamage > 0)
        {
            score -= combatResult.defenderDamage * 1.5f;
        }
        
        // Prefer attacking weaker units
        score += (target.unitData.maxHP - target.currentHP) * 0.5f;
        
        // Prefer attacking valuable units (higher cost units)
        score += target.unitData.cost * 0.01f;
        
        return score;
    }
    
    GridTile FindBestMoveTarget(Unit unit, List<Unit> enemyUnits)
    {
        List<GridTile> movementRange = unitManager.GetMovementRange(unit);
        
        GridTile bestTile = null;
        float bestScore = -1f;
        
        foreach (GridTile tile in movementRange)
        {
            if (tile == null || tile.occupyingUnit != null) continue;
            
            float score = EvaluateMoveTarget(unit, tile, enemyUnits);
            if (score > bestScore)
            {
                bestScore = score;
                bestTile = tile;
            }
        }
        
        return bestTile;
    }
    
    float EvaluateMoveTarget(Unit unit, GridTile targetTile, List<Unit> enemyUnits)
    {
        float score = 0f;
        Vector2Int targetPos = targetTile.GetGridPosition();
        
        // Find closest enemy
        Unit closestEnemy = FindClosestEnemy(unit, enemyUnits);
        if (closestEnemy != null)
        {
            float distanceToEnemy = Vector2Int.Distance(targetPos, closestEnemy.gridPosition);
            
            // Prefer tiles closer to enemies (aggressive behavior)
            score += (20f - distanceToEnemy) * aggressiveness;
            
            // Check if we can attack from this position
            float attackRange = unit.unitData.maxRange;
            if (distanceToEnemy <= attackRange)
            {
                score += 50f; // High bonus for attack positions
            }
        }
        
        // Prefer defensive terrain
        score += targetTile.defenseBonus * 0.5f;
        
        // Avoid edges of map (simple heuristic)
        Vector2Int mapCenter = new Vector2Int(10, 10); // Assuming 20x20 map
        float distanceFromCenter = Vector2Int.Distance(targetPos, mapCenter);
        score += (10f - distanceFromCenter) * exploreFactor;
        
        return score;
    }
    
    Unit FindClosestEnemy(Unit unit, List<Unit> enemyUnits)
    {
        Unit closestEnemy = null;
        float closestDistance = float.MaxValue;
        
        foreach (Unit enemy in enemyUnits)
        {
            if (enemy == null) continue;
            
            float distance = Vector2Int.Distance(unit.gridPosition, enemy.gridPosition);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }
        
        return closestEnemy;
    }
    
    List<Unit> GetEnemyUnits(Unit unit)
    {
        List<Unit> allUnits = unitManager.GetAllUnits();
        List<Unit> enemyUnits = new List<Unit>();
        
        foreach (Unit otherUnit in allUnits)
        {
            if (otherUnit != null && otherUnit.owner != unit.owner)
            {
                enemyUnits.Add(otherUnit);
            }
        }
        
        return enemyUnits;
    }
    
    IEnumerator ExecuteAction(AIAction action)
    {
        if (action == null || action.unit == null)
        {
            Debug.LogWarning("🤖 AI tried to execute null action");
            yield break;
        }
        
        Debug.Log($"🤖 Executing {action.actionType} for {action.unit.name}");
        
        switch (action.actionType)
        {
            case AIActionType.Move:
                yield return StartCoroutine(ExecuteMove(action));
                break;
                
            case AIActionType.Attack:
                yield return StartCoroutine(ExecuteAttack(action));
                break;
                
            case AIActionType.Wait:
                // Just mark the unit as done
                action.unit.hasMoved = true;
                action.unit.hasAttacked = true;
                break;
        }
    }
    
    IEnumerator ExecuteMove(AIAction action)
    {
        if (action.targetTile == null)
        {
            Debug.LogWarning($"🤖 AI move action has null target tile for {action.unit.name}");
            yield break;
        }
        
        Debug.Log($"🤖 Moving {action.unit.name} to {action.targetTile.GetGridPosition()}");
        
        // Clear any previous selections
        gameManager.ClearSelections();
        
        // Select the unit
        gameManager.selectedUnit = action.unit;
        gameManager.selectedTile = action.unit.currentTile;
        
        // Move the unit
        gameManager.MoveSelectedUnit(action.targetTile);
        
        yield return new WaitForSeconds(0.5f); // Allow movement to complete
    }
    
    IEnumerator ExecuteAttack(AIAction action)
    {
        if (action.targetUnit == null)
        {
            Debug.LogWarning($"🤖 AI attack action has null target unit for {action.unit.name}");
            yield break;
        }
        
        Debug.Log($"🤖 {action.unit.name} attacking {action.targetUnit.name}");
        
        // Clear any previous selections
        gameManager.ClearSelections();
        
        // Select the attacking unit
        gameManager.selectedUnit = action.unit;
        gameManager.selectedTile = action.unit.currentTile;
        
        // Attack the target
        gameManager.AttackUnit(action.unit, action.targetUnit);
        
        yield return new WaitForSeconds(1.0f); // Allow attack to complete
    }
    
    // Public methods for external control
    public void SetAggressiveness(float value)
    {
        aggressiveness = Mathf.Clamp01(value);
        Debug.Log($"🤖 AI aggressiveness set to {aggressiveness}");
    }
    
    public void AddAIPlayer(int playerId)
    {
        if (!aiPlayerIDs.Contains(playerId))
        {
            aiPlayerIDs.Add(playerId);
            Debug.Log($"🤖 Added AI control for player {playerId}");
        }
    }
    
    public void RemoveAIPlayer(int playerId)
    {
        if (aiPlayerIDs.Contains(playerId))
        {
            aiPlayerIDs.Remove(playerId);
            Debug.Log($"🤖 Removed AI control for player {playerId}");
        }
    }
}

// AI Action data structure
[System.Serializable]
public class AIAction
{
    public AIActionType actionType;
    public Unit unit;
    public GridTile targetTile;
    public Unit targetUnit;
    
    public AIAction(AIActionType type, Unit unit, GridTile targetTile, Unit targetUnit = null)
    {
        this.actionType = type;
        this.unit = unit;
        this.targetTile = targetTile;
        this.targetUnit = targetUnit;
    }
}

// AI Action types
public enum AIActionType
{
    Move,
    Attack,
    Wait,
    Capture // For future expansion
} 