using UnityEditor;
using UnityEngine;

public class MapGeneratorConfigWindow : EditorWindow
{
    private MapGeneratorConfig config;

    // Variables para gestionar los apartados plegables
    private bool showGeneralSettings = true;
    private bool showMapTiles = true;
    private bool showPlayerSettings = true;

    public static void OpenWindow(MapGeneratorConfig config)
    {
        // Abre la ventana y asigna el archivo de configuración
        MapGeneratorConfigWindow window = GetWindow<MapGeneratorConfigWindow>("Map Generator Config Editor");
        window.config = config;
    }

    private void OnGUI()
    {
        if (config == null)
        {
            EditorGUILayout.HelpBox("No configuration file selected.", MessageType.Warning);
            return;
        }

        GUILayout.Label("Map Generator Config Editor", EditorStyles.boldLabel);

        EditorGUI.BeginChangeCheck();

        // *** General Settings ***
        showGeneralSettings = EditorGUILayout.Foldout(showGeneralSettings, "General Settings", true);
        if (showGeneralSettings)
        {
            EditorGUILayout.BeginVertical("box");
            config.randomSeed = EditorGUILayout.IntField("Random Seed", config.randomSeed);
            config.size = EditorGUILayout.Vector2IntField("Size", config.size);
            config.roomCount = EditorGUILayout.IntField("Room Count", config.roomCount);
            config.roomMaxSize = EditorGUILayout.Vector2IntField("Room Max Size", config.roomMaxSize);
            config.roomMinSize = EditorGUILayout.Vector2IntField("Room Min Size", config.roomMinSize);
            config.mapMultiplier = EditorGUILayout.FloatField("Map Multiplier", config.mapMultiplier);
            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.Space();

        // *** Map Tiles ***
        showMapTiles = EditorGUILayout.Foldout(showMapTiles, "Map Tiles", true);
        if (showMapTiles)
        {
            EditorGUILayout.BeginVertical("box");
            config.floorTilePrefab = (GameObject)EditorGUILayout.ObjectField("Floor Tile Prefab", config.floorTilePrefab, typeof(GameObject), false);
            config.ceilingTilePrefab = (GameObject)EditorGUILayout.ObjectField("Ceiling Tile Prefab", config.ceilingTilePrefab, typeof(GameObject), false);
            config.wallPrefab = (GameObject)EditorGUILayout.ObjectField("Wall Prefab", config.wallPrefab, typeof(GameObject), false);
            config.doorPrefab = (GameObject)EditorGUILayout.ObjectField("Door Prefab", config.doorPrefab, typeof(GameObject), false);
            config.pillarPrefab = (GameObject)EditorGUILayout.ObjectField("Pillar Prefab", config.pillarPrefab, typeof(GameObject), false);
            config.lampPrefab = (GameObject)EditorGUILayout.ObjectField("Lamp Prefab", config.lampPrefab, typeof(GameObject), false);
            config.generateCeiling = EditorGUILayout.Toggle("Generate Ceiling", config.generateCeiling);
            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.Space();

        // *** Player Settings ***
        showPlayerSettings = EditorGUILayout.Foldout(showPlayerSettings, "Player Settings", true);
        if (showPlayerSettings)
        {
            EditorGUILayout.BeginVertical("box");
            config.playerPrefab = (GameObject)EditorGUILayout.ObjectField("Player Prefab", config.playerPrefab, typeof(GameObject), false);
            config.exitArea = (GameObject)EditorGUILayout.ObjectField("Exit Area", config.exitArea, typeof(GameObject), false);
            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.Space();

        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(config); // Marca el objeto como modificado para guardar cambios
        }
    }
}
