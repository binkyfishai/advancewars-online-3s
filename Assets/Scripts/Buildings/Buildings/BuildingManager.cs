using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    [Header("Building Settings")]
    public int incomePerBuilding = 1000;
    
    // All buildings in the game
    private List<Building> allBuildings = new List<Building>();
    
    // Singleton
    public static BuildingManager Instance { get; private set; }
    
    // References
    private GridManager gridManager;
    private GameManager gameManager;
    
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
        gridManager = GridManager.Instance;
        gameManager = GameManager.Instance;
    }
    
    public void InitializeBuildings()
    {
        // TODO: Create buildings from map data or prefabs
        Debug.Log("Buildings initialized");
    }
    
    public void GenerateIncome(int playerId)
    {
        // TODO: Calculate income from player's buildings
        int income = GetIncomeForPlayer(playerId);
        
        if (gameManager != null && playerId < gameManager.players.Count)
        {
            gameManager.players[playerId].funds += income;
            Debug.Log($"Player {playerId} earned {income} from buildings. Total funds: {gameManager.players[playerId].funds}");
        }
    }
    
    public int GetIncomeForPlayer(int playerId)
    {
        // TODO: Count buildings owned by player and calculate income
        int buildingCount = GetBuildingCountForPlayer(playerId);
        return buildingCount * incomePerBuilding;
    }
    
    public int GetBuildingCountForPlayer(int playerId)
    {
        // TODO: Implement building counting
        return 0;
    }
    
    public List<Building> GetBuildingsForPlayer(int playerId)
    {
        // TODO: Implement building filtering
        return new List<Building>();
    }
}

// Placeholder Building class
public class Building : MonoBehaviour
{
    public int owner = -1; // -1 = neutral
    public BuildingType buildingType;
    public GridTile currentTile;
    
    // TODO: Implement building functionality
}

public enum BuildingType
{
    HQ,
    City,
    Factory,
    Airport,
    Port,
    CommTower,
    Silo,
    TempAirport,
    TempPort
} 