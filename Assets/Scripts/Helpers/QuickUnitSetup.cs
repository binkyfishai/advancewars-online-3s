using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class QuickUnitSetup : MonoBehaviour
{
    [Header("Quick Setup")]
    [SerializeField] private bool setupComplete = false;
    
    [ContextMenu("Setup Unit System")]
    public void SetupUnitSystem()
    {
        #if UNITY_EDITOR
        // Find or create UnitDataSetup GameObject
        UnitDataSetup setupComponent = FindObjectOfType<UnitDataSetup>();
        
        if (setupComponent == null)
        {
            GameObject setupObject = new GameObject("UnitDataSetup");
            setupComponent = setupObject.AddComponent<UnitDataSetup>();
            
            Debug.Log("Created UnitDataSetup GameObject!");
            Debug.Log("Now you can:");
            Debug.Log("1. Select the UnitDataSetup GameObject");
            Debug.Log("2. Drag your GLB models from Assets/models/units/ into the 'Unit Models' slots");
            Debug.Log("3. Right-click the component and choose 'Create All Unit Data'");
        }
        else
        {
            Debug.Log("UnitDataSetup already exists in the scene!");
        }
        
        // Select the setup component for convenience
        Selection.activeGameObject = setupComponent.gameObject;
        setupComplete = true;
        #endif
    }
    
    void Start()
    {
        if (!setupComplete)
        {
            SetupUnitSystem();
        }
    }
} 