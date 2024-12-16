using System.Collections;  // Para usar corutinas
using UnityEngine;
using UnityEngine.SceneManagement;  // Para cargar escenas

public class LevelManager : MonoBehaviour
{
    // La instancia est�tica para el patr�n Singleton
    public static LevelManager Instance { get; private set; }

    [Tooltip("Lista de prefabs que representan los niveles")]
    public GameObject[] levelPrefabs;

    [Tooltip("Escena de victoria que se cargar� al completar el �ltimo nivel")]
    public string winLevelName;

    private int currentLevelIndex = 0;  // �ndice del nivel actual
    private GameObject currentLevelObject;  // Para mantener una referencia al nivel instanciado

    [Tooltip("Objeto al que el jugador ser� transportado antes de ser teletransportado al nuevo spawn")]
    public GameObject transportObject;

    [Tooltip("Tiempo que el jugador permanecer� en el objeto de transporte antes de ser teletransportado")]
    public float transportDuration = 2f;

    private void Awake()
    {
        // Asegurar que solo haya una instancia del LevelManager
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // Mantener el manager entre escenas
        }
        else
        {
            Destroy(gameObject);  // Eliminar instancias adicionales
        }
    }

    private void Start()
    {
        LoadLevel(currentLevelIndex);  // Cargar el primer nivel al inicio
    }

    // Funci�n para cargar el siguiente nivel
    public void NextLevel()
    {
        currentLevelIndex++;

        // Verificar si hemos llegado al �ltimo nivel
        if (currentLevelIndex < levelPrefabs.Length)
        {
            StartCoroutine(TransportToNextLevel());  // Usar corutina para manejar el transporte
        }
        else
        {
            // Si es el �ltimo nivel, cargar el winLevel
            LoadWinLevel();
        }
    }

    // Funci�n para cargar un nivel dado su �ndice
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
            Debug.LogError("�ndice de nivel fuera de rango.");
        }
    }

    // Funci�n para cargar el nivel de victoria
    private void LoadWinLevel()
    {
        if (!string.IsNullOrEmpty(winLevelName))
        {
            // Cargar la escena de victoria
            SceneManager.LoadScene(winLevelName);
            Debug.Log("�Has ganado! Cargando nivel de victoria.");
        }
        else
        {
            Debug.LogError("No se ha configurado el nombre del nivel de victoria.");
        }
    }

    // Corutina para transportar al jugador
    private IEnumerator TransportToNextLevel()
    {
        // Transportar al jugador al objeto de transporte
        Debug.Log("Transportando al jugador...");
        GameObject player = GameObject.FindWithTag("Player"); // Asumimos que el jugador tiene la etiqueta "Player"

        if (player != null && transportObject != null)
        {
            player.transform.position = transportObject.transform.position;

            // Esperar el tiempo de transporte
            yield return new WaitForSeconds(transportDuration);

            // Eliminar el nivel anterior
            Destroy(currentLevelObject);

            // Instanciar el nuevo nivel
            LoadLevel(currentLevelIndex);

            // Teletransportar al jugador al spawnPoint del nuevo nivel
            Transform spawnPoint = currentLevelObject.transform.Find("SpawnPoint");
            if (spawnPoint != null)
            {
                player.transform.position = spawnPoint.position;
                Debug.Log("Jugador teletransportado al nuevo spawn.");
            }
            else
            {
                Debug.LogError("No se encontr� el SpawnPoint en el nuevo nivel.");
            }
        }
        else
        {
            Debug.LogError("Jugador o objeto de transporte no encontrado.");
        }
    }
}
