using System.Collections;  // Para usar corutinas
using UnityEngine;
using UnityEngine.SceneManagement;  // Para cargar escenas

public class LevelManager : MonoBehaviour
{
    // La instancia estática para el patrón Singleton
    public static LevelManager Instance { get; private set; }

    [Tooltip("Lista de prefabs que representan los niveles")]
    public GameObject[] levelPrefabs;

    [Tooltip("Escena de victoria que se cargará al completar el último nivel")]
    public string winLevelName;

    private int currentLevelIndex = -1;  // Índice del nivel actual
    private GameObject currentLevelObject;  // Para mantener una referencia al nivel instanciado

    [Tooltip("Objeto al que el jugador será transportado antes de ser teletransportado al nuevo spawn")]
    public GameObject transportObject;

    [Tooltip("Tiempo que el jugador permanecerá en el objeto de transporte antes de ser teletransportado")]
    public float transportDuration = 2f;

    public GameObject healthPickup;
    private GameObject player;

    private void Awake()
    {
        // Asegurar que solo haya una instancia del LevelManager
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);  // Mantener el manager entre escenas
        }
        else
        {
            Destroy(gameObject);  // Eliminar instancias adicionales
        }

        //NextLevel();
    }

    private void Start()
    {
        player = GameObject.FindWithTag("Player");

        NextLevel();

        healthPickup.SetActive(false);
    }

    // Función para cargar el siguiente nivel
    public void NextLevel()
    {
        currentLevelIndex++;

        // Verificar si hemos llegado al último nivel
        if (currentLevelIndex < levelPrefabs.Length)
        {
            // Usar corutina para manejar el transporte
            StartCoroutine(TransportToNextLevel());
        }
        else
        {
            currentLevelIndex = -1;
            // Si es el último nivel, cargar el winLevel
            LoadWinLevel();
        }
    }

    // Función para cargar un nivel dado su índice
    private void LoadLevel(int levelIndex)
    {
        if (levelIndex >= 0 && levelIndex < levelPrefabs.Length)
        {
            // Instanciamos el prefab del nivel en la escena
            currentLevelObject = Instantiate(levelPrefabs[levelIndex], Vector3.zero, Quaternion.identity);
            Debug.Log($"Nivel {levelIndex + 1} cargado.");
        }
        else
        {
            Debug.LogError("Índice de nivel fuera de rango.");
        }
    }

    // Función para cargar el nivel de victoria
    private void LoadWinLevel()
    {
        if (!string.IsNullOrEmpty(winLevelName))
        {
            // Cargar la escena de victoria
            SceneManager.LoadScene(winLevelName);
            Debug.Log("¡Has ganado! Cargando nivel de victoria.");
        }
        else
        {
            Debug.LogError("No se ha configurado el nombre del nivel de victoria.");
        }
    }

    // Corutina para transportar al jugador
    private IEnumerator TransportToNextLevel()
    {
        Debug.Log("Transportando al jugador...");

        // Reactivar el objeto de salud antes del transporte
        if (healthPickup != null && !healthPickup.activeSelf)
        {
            healthPickup.SetActive(true);
        }

        if (player != null && transportObject != null)
        {
            // Mostrar el HUD del Stopwatch
            HUDManager.Instance.ShowStopwatch();

            // Actualizar posición del jugador al objeto de transporte
            player.transform.position = transportObject.transform.position;

            // Cuenta regresiva del transporte
            float timer = transportDuration;
            while (timer > 0)
            {
                // Actualizar HUD con el tiempo restante
                HUDManager.Instance.UpdateCountdown(timer);
                timer -= Time.deltaTime;
                yield return null;
            }

            // Ocultar el HUD del Stopwatch cuando termine la cuenta regresiva
            HUDManager.Instance.HideStopwatch();

            // Eliminar el nivel anterior
            Destroy(currentLevelObject);

            // Instanciar el nuevo nivel
            LoadLevel(currentLevelIndex);

            // Teletransportar al jugador al spawnPoint del nuevo nivel
            Transform spawnPoint = FindDeepChild(currentLevelObject.transform, "SpawnPoint");
            if (spawnPoint != null)
            {
                // Mover al jugador al SpawnPoint
                player.transform.position = spawnPoint.position;

                // Hacer al jugador hijo del SpawnPoint
                player.transform.SetParent(spawnPoint);

                // Asegurarse de que la escala y rotación del jugador no se vea afectada por el SpawnPoint
                player.transform.localRotation = Quaternion.identity;
                player.transform.localScale = Vector3.one;

                Debug.Log("Jugador teletransportado al nuevo spawn.");
            }
            else
            {
                Debug.LogError("No se encontró el SpawnPoint en el nuevo nivel.");
            }
        }
        else
        {
            Debug.LogError("Jugador o objeto de transporte no encontrado.");
        }
    }

    public Transform FindDeepChild(Transform parent, string name)
    {
        Transform[] allChildren = parent.GetComponentsInChildren<Transform>(true);
        foreach (Transform child in allChildren)
        {
            if (child.name == name)
            {
                return child;
            }
        }
        return null;
    }

    public void RestartLevel()
    {
        player = GameObject.FindWithTag("Player");

        currentLevelIndex = -1;

        NextLevel();

        healthPickup.SetActive(false);
    }
}
