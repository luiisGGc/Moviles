using UnityEngine;
using UnityEngine.UI;

public class LogicaFugaGas : MonoBehaviour
{
    [Header("Configuración de Barra")]
    public Image barraProgreso;
    public float velocidadReparacion = 0.25f; 
    public float velocidadFuga = 0.15f;      
    public float tiempoLimite = 10f; // Tiempo para reparar la fuga

    [Header("Visuales de Tubería")]
    public Image imagenTuberia;
    public Sprite tuberiaRota;
    public Sprite tuberiaArreglada;
    public Sprite tuberiaExplotada; // Opcional para la derrota

    private float progreso = 0f;
    private float cronometro;
    private bool gameOver = false;

    void Start()
    {
        cronometro = tiempoLimite;
        progreso = 0f;
        barraProgreso.fillAmount = 0f;
        if (imagenTuberia != null) imagenTuberia.sprite = tuberiaRota;
    }

    void Update()
    {
        if (gameOver) return;

        // 1. Manejo del tiempo (Derrota)
        cronometro -= Time.deltaTime;
        if (cronometro <= 0)
        {
            LoseGame();
        }

        // 2. Lógica de reparación (Mantener pulsado)
        if (Input.GetMouseButton(0))
        {
            progreso += velocidadReparacion * Time.deltaTime;
        }
        else 
        {
            if (progreso > 0) progreso -= velocidadFuga * Time.deltaTime;
        }

        // 3. Actualización de UI
        progreso = Mathf.Clamp(progreso, 0f, 1f);
        barraProgreso.fillAmount = progreso;

        // 4. Condición de Victoria
        if (progreso >= 1f)
        {
            WinGame();
        }
    }

    // --- ÚNICAMENTE LAS FUNCIONES SOLICITADAS ---

    private void WinGame()
    {
        gameOver = true;
        
        if (imagenTuberia != null) 
            imagenTuberia.sprite = tuberiaArreglada;

        // Reportar victoria al GameManager global
        if (GameManager.Instance != null) 
        {
            GameManager.Instance.ReportarVictoria();
        }

        Debug.Log("Victoria: Fuga reparada.");
    }

    private void LoseGame()
    {
        gameOver = true;

        if (imagenTuberia != null && tuberiaExplotada != null) 
            imagenTuberia.sprite = tuberiaExplotada;

        // Reportar derrota al GameManager global
        if (GameManager.Instance != null) 
        {
            GameManager.Instance.ReportarDerrota();
        }

        Debug.Log("Derrota: El gas se escapó por completo.");
    }

    // --------------------------------------------
}