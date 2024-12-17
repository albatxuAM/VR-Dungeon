using TMPro;
using UnityEngine;

public class HUDManager : MonoBehaviour
{
    [Header("Stopwatch UI Elements")]
    [Tooltip("Panel del HUD que contiene el stopwatch.")]
    public GameObject stopwatchPanel;

    [Tooltip("Texto que muestra la cuenta regresiva.")]
    public TMP_Text countdownText;

    [Header("Options")]
    [Tooltip("Mostrar o no los decimales en el temporizador.")]
    public bool showDecimals = true;

    // Singleton para acceder fácilmente desde otros scripts
    public static HUDManager Instance { get; private set; }

    private void Awake()
    {
        // Configurar Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Asegurarse de que el HUD esté inicialmente oculto
        HideStopwatch();
    }

    // Muestra el HUD del stopwatch.
    public void ShowStopwatch()
    {
        if (stopwatchPanel != null)
        {
            stopwatchPanel.SetActive(true);
        }
    }

    // Actualiza el texto de la cuenta regresiva.
    public void UpdateCountdown(float seconds)
    {
        if (countdownText != null)
        {
            if (showDecimals)
            {
                countdownText.text = seconds.ToString("F1"); // Formato con un decimal
            }
            else
            {
                countdownText.text = Mathf.CeilToInt(seconds).ToString(); // Redondeo hacia arriba, sin decimales
            }
        }
    }

    // Oculta el HUD del stopwatch.
    public void HideStopwatch()
    {
        if (stopwatchPanel != null)
        {
            stopwatchPanel.SetActive(false);
        }
    }
}
