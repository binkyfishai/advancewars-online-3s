using UnityEngine;

[System.Serializable]
public class AIManager : MonoBehaviour
{
    [Header("AI Configuration")]
    [SerializeField] private AISettings aiSettings;
    [SerializeField] private bool useCustomSettings = false;
    
    [Header("Custom Settings (if not using AISettings asset)")]
    [SerializeField] private int[] customAIPlayerIDs = { 1 };
    [SerializeField] private float customActionDelay = 1.0f;
    [SerializeField] private float customTurnEndDelay = 2.0f;
    [SerializeField] private float customAggressiveness = 0.7f;
    [SerializeField] private float customExploreFactor = 0.3f;
    
    // AI Controller component
    private AIController aiController;
    
    void Awake()
    {
        // Get or add AIController component
        aiController = GetComponent<AIController>();
        if (aiController == null)
        {
            aiController = gameObject.AddComponent<AIController>();
        }
    }
    
    void Start()
    {
        SetupAI();
    }
    
    void SetupAI()
    {
        if (aiController == null)
        {
            Debug.LogError("AIManager: No AIController found!");
            return;
        }
        
        // Apply settings to AI Controller
        if (useCustomSettings)
        {
            ApplyCustomSettings();
        }
        else if (aiSettings != null)
        {
            ApplyAISettings();
        }
        else
        {
            Debug.LogWarning("AIManager: No AI settings configured, using defaults");
        }
        
        Debug.Log("🤖 AI Manager setup complete");
    }
    
    void ApplyAISettings()
    {
        if (aiSettings == null) return;
        
        // Set AI players
        foreach (int playerId in aiSettings.aiPlayerIDs)
        {
            aiController.AddAIPlayer(playerId);
        }
        
        // Apply difficulty-based settings
        switch (aiSettings.difficulty)
        {
            case AIDifficulty.Easy:
                aiController.SetAggressiveness(aiSettings.aggressiveness * 0.6f);
                break;
            case AIDifficulty.Normal:
                aiController.SetAggressiveness(aiSettings.aggressiveness);
                break;
            case AIDifficulty.Hard:
                aiController.SetAggressiveness(aiSettings.aggressiveness * 1.2f);
                break;
            case AIDifficulty.Expert:
                aiController.SetAggressiveness(aiSettings.aggressiveness * 1.4f);
                break;
        }
        
        Debug.Log($"🤖 Applied AI settings: Difficulty={aiSettings.difficulty}, Players={string.Join(",", aiSettings.aiPlayerIDs)}");
    }
    
    void ApplyCustomSettings()
    {
        // Set AI players
        foreach (int playerId in customAIPlayerIDs)
        {
            aiController.AddAIPlayer(playerId);
        }
        
        // Apply custom settings
        aiController.SetAggressiveness(customAggressiveness);
        
        Debug.Log($"🤖 Applied custom AI settings: Players={string.Join(",", customAIPlayerIDs)}, Aggressiveness={customAggressiveness}");
    }
    
    // Public methods for runtime control
    public void SetAIDifficulty(AIDifficulty difficulty)
    {
        if (aiSettings != null)
        {
            aiSettings.difficulty = difficulty;
            ApplyAISettings();
        }
    }
    
    public void SetAIAggressiveness(float aggressiveness)
    {
        if (aiController != null)
        {
            aiController.SetAggressiveness(aggressiveness);
        }
    }
    
    public void AddAIPlayer(int playerId)
    {
        if (aiController != null)
        {
            aiController.AddAIPlayer(playerId);
        }
    }
    
    public void RemoveAIPlayer(int playerId)
    {
        if (aiController != null)
        {
            aiController.RemoveAIPlayer(playerId);
        }
    }
    
    // Editor helper methods
    [ContextMenu("Create Default AI Settings")]
    void CreateDefaultAISettings()
    {
        #if UNITY_EDITOR
        if (aiSettings == null)
        {
            string path = "Assets/ScriptableObjects/DefaultAISettings.asset";
            
            // Create directory if it doesn't exist
            string directory = System.IO.Path.GetDirectoryName(path);
            if (!System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
            }
            
            // Create the asset
            AISettings newSettings = CreateInstance<AISettings>();
            UnityEditor.AssetDatabase.CreateAsset(newSettings, path);
            UnityEditor.AssetDatabase.SaveAssets();
            
            aiSettings = newSettings;
            
            Debug.Log($"Created default AI settings at: {path}");
        }
        #endif
    }
} 