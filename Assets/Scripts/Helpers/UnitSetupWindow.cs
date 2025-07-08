#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class UnitSetupWindow : EditorWindow
{
    private UnitDataSetup unitSetup;
    private Vector2 scrollPos;
    
    [MenuItem("Tools/Unit Setup Assistant")]
    public static void ShowWindow()
    {
        GetWindow<UnitSetupWindow>("Unit Setup Assistant");
    }
    
    void OnGUI()
    {
        GUILayout.Label("🎮 Unit Setup Assistant", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        if (unitSetup == null)
        {
            unitSetup = FindFirstObjectByType<UnitDataSetup>();
        }
        
        if (unitSetup == null)
        {
            GUILayout.Label("No UnitDataSetup found in scene", EditorStyles.helpBox);
            
            if (GUILayout.Button("Create UnitDataSetup GameObject"))
            {
                GameObject obj = new GameObject("UnitDataSetup");
                unitSetup = obj.AddComponent<UnitDataSetup>();
                Selection.activeGameObject = obj;
            }
            return;
        }
        
        GUILayout.Label("✅ UnitDataSetup found!", EditorStyles.helpBox);
        
        if (GUILayout.Button("Select UnitDataSetup in Hierarchy"))
        {
            Selection.activeGameObject = unitSetup.gameObject;
        }
        
        GUILayout.Space(10);
        
        // Show unit model assignments
        GUILayout.Label("Unit Model Assignments:", EditorStyles.boldLabel);
        
        scrollPos = GUILayout.BeginScrollView(scrollPos);
        
        for (int i = 0; i < unitSetup.unitModels.Length; i++)
        {
            var unit = unitSetup.unitModels[i];
            
            GUILayout.BeginHorizontal();
            GUILayout.Label($"{unit.unitName}:", GUILayout.Width(80));
            
            if (unit.modelPrefab != null)
            {
                GUILayout.Label("✅", GUILayout.Width(20));
                GUILayout.Label(unit.modelPrefab.name, EditorStyles.miniLabel);
            }
            else
            {
                GUILayout.Label("❌", GUILayout.Width(20));
                GUILayout.Label("No model assigned", EditorStyles.miniLabel);
            }
            
            GUILayout.EndHorizontal();
        }
        
        GUILayout.EndScrollView();
        
        GUILayout.Space(10);
        
        // Action buttons
        GUILayout.Label("Actions:", EditorStyles.boldLabel);
        
        if (GUILayout.Button("📁 Open Models Folder"))
        {
            EditorUtility.OpenFilePanel("Select Model", "Assets/models/units", "glb");
        }
        
        if (GUILayout.Button("🔧 Create All Unit Data"))
        {
            unitSetup.CreateAllUnitData();
        }
        
        if (GUILayout.Button("🔄 Update Model Assignments"))
        {
            unitSetup.UpdateModelAssignments();
        }
        
        GUILayout.Space(10);
        
        // Status
        int assignedCount = 0;
        foreach (var unit in unitSetup.unitModels)
        {
            if (unit.modelPrefab != null) assignedCount++;
        }
        
        GUILayout.Label($"Progress: {assignedCount}/{unitSetup.unitModels.Length} models assigned", EditorStyles.helpBox);
        
        if (assignedCount == unitSetup.unitModels.Length)
        {
            GUILayout.Label("🎉 All models assigned! Ready to create unit data!", EditorStyles.helpBox);
        }
        
        GUILayout.Space(10);
        
        // Instructions
        GUILayout.Label("Quick Instructions:", EditorStyles.boldLabel);
        GUILayout.Label("1. Select UnitDataSetup in Hierarchy");
        GUILayout.Label("2. Drag GLB models to 'Unit Models' slots");
        GUILayout.Label("3. Click 'Create All Unit Data'");
        GUILayout.Label("4. Assign created data to GameManager");
    }
}
#endif 