using UnityEngine;

[CreateAssetMenu(fileName = "MapGeneratorConfig", menuName = "Configurations/Map Generator Config")]
public class MapGeneratorConfig : ScriptableObject
{
    [Header("Configuration")]
    public int randomSeed;
    public Vector2Int size;
    public int roomCount;
    public Vector2Int roomMaxSize;
    public Vector2Int roomMinSize;
    public float mapMultiplier = 1;

    [Header("Map Tiles")]
    public GameObject floorTilePrefab;
    public GameObject ceilingTilePrefab;
    public GameObject wallPrefab;
    public GameObject doorPrefab;
    public GameObject pillarPrefab;
    public GameObject lampPrefab;
    public bool generateCeiling;

    [Header("Player Settings")]
    public GameObject playerPrefab;
    public GameObject exitArea;
}
