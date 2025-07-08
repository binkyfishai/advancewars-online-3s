using UnityEngine;

[CreateAssetMenu(fileName = "New AI Settings", menuName = "Advanced Wars/AI Settings")]
public class AISettings : ScriptableObject
{
    [Header("AI Players")]
    [Tooltip("Which player IDs should be controlled by AI (0 = Orange Star, 1 = Blue Moon, etc.)")]
    public int[] aiPlayerIDs = { 1 };
    
    [Header("AI Timing")]
    [Range(0.1f, 3.0f)]
    [Tooltip("Delay between AI actions for visual clarity")]
    public float actionDelay = 1.0f;
    
    [Range(0.5f, 5.0f)]
    [Tooltip("Delay before AI ends its turn")]
    public float turnEndDelay = 2.0f;
    
    [Header("AI Personality")]
    [Range(0.0f, 1.0f)]
    [Tooltip("How aggressive the AI is (0 = defensive, 1 = very aggressive)")]
    public float aggressiveness = 0.7f;
    
    [Range(0.0f, 1.0f)]
    [Tooltip("How much AI explores vs focuses on enemies")]
    public float exploreFactor = 0.3f;
    
    [Header("AI Difficulty")]
    [Tooltip("AI difficulty preset")]
    public AIDifficulty difficulty = AIDifficulty.Normal;
    
    [Header("Debug")]
    [Tooltip("Enable detailed AI logging")]
    public bool enableDebugLogging = true;
    
    [Tooltip("Show AI thinking process in console")]
    public bool showThinkingProcess = false;
}

public enum AIDifficulty
{
    Easy,       // Lower aggressiveness, slower actions
    Normal,     // Balanced behavior
    Hard,       // Higher aggressiveness, smarter targeting
    Expert      // Optimal play, fastest actions
} 