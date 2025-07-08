using System.Collections.Generic;
using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    [Header("Terrain Settings")]
    public TerrainSet currentTerrainSet = TerrainSet.Normal;
    public float terrainScale = 1f;
    
    [Header("Terrain Models")]
    public TerrainModelLibrary terrainLibrary;
    
    [Header("Generation Settings")]
    public bool generateRandomTerrain = false;
    public int randomSeed = 12345;
    
    // Terrain data
    private Dictionary<Vector2Int, TerrainTile> terrainTiles = new Dictionary<Vector2Int, TerrainTile>();
    private Dictionary<TerrainType, List<GameObject>> terrainPrefabs = new Dictionary<TerrainType, List<GameObject>>();
    
    // Components
    private GridManager gridManager;
    
    void Awake()
    {
        gridManager = FindObjectOfType<GridManager>();
        LoadTerrainModels();
    }
    
    void LoadTerrainModels()
    {
        if (terrainLibrary == null)
        {
            Debug.LogWarning("TerrainManager: No terrain library assigned!");
            return;
        }
        
        // Load terrain models from Resources or assigned prefabs
        LoadTerrainPrefabs();
    }
    
    void LoadTerrainPrefabs()
    {
        // Initialize terrain prefab dictionary
        terrainPrefabs.Clear();
        
        // Load terrain models from the terrain library
        foreach (var terrainGroup in terrainLibrary.terrainGroups)
        {
            if (!terrainPrefabs.ContainsKey(terrainGroup.terrainType))
            {
                terrainPrefabs[terrainGroup.terrainType] = new List<GameObject>();
            }
            
            foreach (var prefab in terrainGroup.terrainPrefabs)
            {
                if (prefab != null)
                {
                    terrainPrefabs[terrainGroup.terrainType].Add(prefab);
                }
            }
        }
        
        // Load default terrain models from Resources if library is empty
        if (terrainPrefabs.Count == 0)
        {
            LoadDefaultTerrainModels();
        }
    }
    
    void LoadDefaultTerrainModels()
    {
        // Load terrain models from Resources folder
        string basePath = GetTerrainPath();
        
        // Load basic terrain types
        LoadTerrainType(TerrainType.Plain, basePath + "/Plain.Textured.Grid");
        LoadTerrainType(TerrainType.Forest, basePath + "/Forest");
        LoadTerrainType(TerrainType.Mountain, basePath + "/MountainA");
        LoadTerrainType(TerrainType.River, basePath + "/River.NormalRiver.Plane");
        LoadTerrainType(TerrainType.Sea, basePath + "/Sea.DeepSea.Plane");
        LoadTerrainType(TerrainType.Road, basePath + "/Road.Horizontal.A");
        LoadTerrainType(TerrainType.Bridge, basePath + "/RiverBridge.RoadVertical");
        LoadTerrainType(TerrainType.Reef, basePath + "/Reef");
        LoadTerrainType(TerrainType.Pipe, basePath + "/Pipe.Horizontal.TipA");
        LoadTerrainType(TerrainType.Waterfall, basePath + "/Waterfall.A");
        LoadTerrainType(TerrainType.Shore, basePath + "/Shore.Horizontal.A");
    }
    
    void LoadTerrainType(TerrainType type, string resourcePath)
    {
        GameObject terrainModel = Resources.Load<GameObject>(resourcePath);
        if (terrainModel != null)
        {
            if (!terrainPrefabs.ContainsKey(type))
            {
                terrainPrefabs[type] = new List<GameObject>();
            }
            terrainPrefabs[type].Add(terrainModel);
        }
        else
        {
            Debug.LogWarning($"TerrainManager: Could not load terrain model at {resourcePath}");
        }
    }
    
    string GetTerrainPath()
    {
        switch (currentTerrainSet)
        {
            case TerrainSet.Normal:
                return "terrain/tiles/Normal";
            case TerrainSet.Desert:
                return "terrain/tiles/Desert";
            case TerrainSet.Snow:
                return "terrain/tiles/Snow";
            case TerrainSet.Wasteland:
                return "terrain/tiles/Wasteland";
            default:
                return "terrain/tiles/Normal";
        }
    }
    
    public void GenerateTerrain(Vector2Int gridSize)
    {
        if (generateRandomTerrain)
        {
            GenerateRandomTerrain(gridSize);
        }
        else
        {
            GenerateDefaultTerrain(gridSize);
        }
    }
    
    public void GenerateDefaultTerrain(Vector2Int gridSize)
    {
        // Generate a simple default terrain layout
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector2Int gridPos = new Vector2Int(x, y);
                TerrainType terrainType = GetDefaultTerrainType(gridPos, gridSize);
                CreateTerrainTile(gridPos, terrainType);
            }
        }
    }
    
    public void GenerateRandomTerrain(Vector2Int gridSize)
    {
        Random.InitState(randomSeed);
        
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector2Int gridPos = new Vector2Int(x, y);
                TerrainType terrainType = GetRandomTerrainType(gridPos, gridSize);
                CreateTerrainTile(gridPos, terrainType);
            }
        }
    }
    
    TerrainType GetDefaultTerrainType(Vector2Int gridPos, Vector2Int gridSize)
    {
        // Create a simple terrain pattern
        int x = gridPos.x;
        int y = gridPos.y;
        
        // Add some forests
        if ((x + y) % 5 == 0 && x > 2 && y > 2 && x < gridSize.x - 3 && y < gridSize.y - 3)
        {
            return TerrainType.Forest;
        }
        
        // Add some mountains
        if ((x % 7 == 0 && y % 7 == 0) && x > 0 && y > 0 && x < gridSize.x - 1 && y < gridSize.y - 1)
        {
            return TerrainType.Mountain;
        }
        
        // Add a river
        if (x == gridSize.x / 2 && y > 3 && y < gridSize.y - 3)
        {
            return TerrainType.River;
        }
        
        // Default to plain
        return TerrainType.Plain;
    }
    
    TerrainType GetRandomTerrainType(Vector2Int gridPos, Vector2Int gridSize)
    {
        float random = Random.Range(0f, 1f);
        
        if (random < 0.6f)
            return TerrainType.Plain;
        else if (random < 0.75f)
            return TerrainType.Forest;
        else if (random < 0.85f)
            return TerrainType.Mountain;
        else if (random < 0.95f)
            return TerrainType.River;
        else
            return TerrainType.Road;
    }
    
    public void CreateTerrainTile(Vector2Int gridPos, TerrainType terrainType)
    {
        // Get the corresponding GridTile
        if (gridManager == null)
        {
            Debug.LogWarning("TerrainManager: No GridManager found!");
            return;
        }
        
        GridTile gridTile = gridManager.GetTile(gridPos);
        if (gridTile == null)
        {
            Debug.LogWarning($"TerrainManager: No GridTile found at {gridPos}");
            return;
        }
        
        // Create or update terrain tile component
        TerrainTile terrainTile = gridTile.GetComponent<TerrainTile>();
        if (terrainTile == null)
        {
            terrainTile = gridTile.gameObject.AddComponent<TerrainTile>();
        }
        
        // Set terrain type
        terrainTile.SetTerrainType(terrainType);
        
        // Set terrain model
        GameObject terrainModel = GetTerrainModel(terrainType);
        if (terrainModel != null)
        {
            GameObject instantiatedModel = Instantiate(terrainModel, gridTile.transform);
            instantiatedModel.transform.localPosition = Vector3.zero;
            instantiatedModel.transform.localRotation = Quaternion.identity;
            instantiatedModel.transform.localScale = Vector3.one * terrainScale;
            
            terrainTile.SetTerrainModel(instantiatedModel);
        }
        
        // Store in dictionary
        terrainTiles[gridPos] = terrainTile;
    }
    
    GameObject GetTerrainModel(TerrainType terrainType)
    {
        if (terrainPrefabs.ContainsKey(terrainType) && terrainPrefabs[terrainType].Count > 0)
        {
            // Return a random variant if multiple models exist
            List<GameObject> models = terrainPrefabs[terrainType];
            return models[Random.Range(0, models.Count)];
        }
        
        return null;
    }
    
    public TerrainTile GetTerrainTile(Vector2Int gridPos)
    {
        return terrainTiles.TryGetValue(gridPos, out TerrainTile tile) ? tile : null;
    }
    
    public TerrainType GetTerrainType(Vector2Int gridPos)
    {
        TerrainTile tile = GetTerrainTile(gridPos);
        return tile != null ? tile.terrainType : TerrainType.Plain;
    }
    
    public void SetTerrainSet(TerrainSet terrainSet)
    {
        currentTerrainSet = terrainSet;
        LoadTerrainModels();
    }
    
    public void ClearTerrain()
    {
        foreach (var terrainTile in terrainTiles.Values)
        {
            if (terrainTile != null)
            {
                DestroyImmediate(terrainTile.gameObject);
            }
        }
        terrainTiles.Clear();
    }
    
    public void RefreshTerrain()
    {
        Vector2Int gridSize = gridManager.gridSize;
        ClearTerrain();
        GenerateTerrain(gridSize);
    }
}

[System.Serializable]
public class TerrainModelLibrary
{
    public TerrainGroup[] terrainGroups;
}

[System.Serializable]
public class TerrainGroup
{
    public TerrainType terrainType;
    public GameObject[] terrainPrefabs;
}

[System.Serializable]
public enum TerrainSet
{
    Normal,
    Desert,
    Snow,
    Wasteland
} 