using UnityEngine;

public class PrefabCreator : MonoBehaviour
{
    [Header("Create Basic Prefabs for Testing")]
    [SerializeField] private bool createPrefabs = false;
    
    void Start()
    {
        if (createPrefabs)
        {
            CreateBasicTilePrefabs();
            CreateBasicMaterials();
            createPrefabs = false;
        }
    }
    
    void CreateBasicTilePrefabs()
    {
        // Create Plain Tile
        CreateTilePrefab("PlainTile", Color.green, Vector3.zero);
        
        // Create Mountain Tile  
        CreateTilePrefab("MountainTile", Color.gray, new Vector3(0, 0.5f, 0));
        
        Debug.Log("Basic tile prefabs created!");
    }
    
    void CreateTilePrefab(string name, Color color, Vector3 offset)
    {
        // Create cube
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = name;
        cube.transform.position = offset;
        cube.transform.localScale = new Vector3(0.9f, 0.2f, 0.9f);
        
        // Set color
        Renderer renderer = cube.GetComponent<Renderer>();
        Material mat = new Material(Shader.Find("Standard"));
        mat.color = color;
        renderer.material = mat;
        
        // Add GridTile component
        GridTile gridTile = cube.AddComponent<GridTile>();
        
        // Set layer for raycasting
        cube.layer = 8; // Default layer for tiles
        
        // Save as prefab (this would need to be done manually in the editor)
        Debug.Log($"Created {name} - save this as a prefab in Assets/Prefabs/");
    }
    
    void CreateBasicMaterials()
    {
        // This would create highlight materials - for now just log instructions
        Debug.Log("Create materials manually:");
        Debug.Log("1. Create 'HighlightMaterial' - Standard shader, yellow color, emission enabled");
        Debug.Log("2. Create 'MovementRangeMaterial' - Standard shader, blue color, emission enabled");
        Debug.Log("3. Create 'AttackRangeMaterial' - Standard shader, red color, emission enabled");
    }
} 