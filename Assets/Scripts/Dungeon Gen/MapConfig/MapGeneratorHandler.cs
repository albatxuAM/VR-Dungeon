using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public class MapGeneratorHandler
{
    // Permite abrir la ventana al hacer doble clic en un ScriptableObject
    [OnOpenAsset()]
    public static bool OpenEditor(int instanceID, int line)
    {
        MapGeneratorConfig obj = EditorUtility.InstanceIDToObject(instanceID) as MapGeneratorConfig;

        if (obj != null)
        {
            MapGeneratorConfigWindow.OpenWindow(obj);
            return true;
        }

        return false;
    }
}

[CustomEditor(typeof(MapGeneratorConfig))]
public class MapGeneratorCustomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Open Config Editor"))
        {
            MapGeneratorConfigWindow.OpenWindow((MapGeneratorConfig)target);
        }
    }
}
