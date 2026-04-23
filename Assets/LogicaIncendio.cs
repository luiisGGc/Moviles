using UnityEngine;
using UnityEngine.UI;

public class LogicaIncendio : MonoBehaviour
{
    [Header("UI de Progreso")]
    public Image barraProgreso; 
    public float velocidadDescenso = 0.15f; 
    public float fuerzaClick = 0.08f; 
    public float tiempoLimite = 8.0f; // Tiempo para lograr abrir la puerta

    [Header("Sprites de la Puerta")]
    public Image imagenPuerta; 
    public Sprite puertaConFuego;
    public Sprite puertaAbierta;
    public Sprite puertaQuemada; // Opcional: Sprite si pierdes

    private float progresoActual = 0f;
    private float cronometro;
    private bool gameOver = false;

    void Start()
    {
        cronometro = tiempoLimite;
        progresoActual = 0f;
        barraProgreso.fillAmount = 0f;
        
        if (imagenPuerta != null) 
            imagenPuerta.sprite = puertaConFuego;
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

        // 2. Lógica de descenso natural de la barra
        if (progresoActual > 0)
        {
            progresoActual -= velocidadDescenso * Time.deltaTime;
        }

        // 3. Entrada de clics/toques
        if (Input.GetMouseButtonDown(0))
        {
            progresoActual += fuerzaClick;
        }

        // 4. Actualización visual
        progresoActual = Mathf.Clamp(progresoActual, 0f, 1f);
        barraProgreso.fillAmount = progresoActual;

        // 5. Condición de Victoria
        if (progresoActual >= 1f)
        {
            WinGame();
        }
    }

    // --- ÚNICAMENTE LAS FUNCIONES SOLICITADAS ---

    private void WinGame()
    {
        gameOver = true;
        
        if (imagenPuerta != null) 
            imagenPuerta.sprite = puertaAbierta; 

        // Reportar victoria al GameManager global
        if (GameManager.Instance != null) 
        {
            GameManager.Instance.ReportarVictoria();
        }

        Debug.Log("Victoria: Escapaste del incendio.");
    }

    private void LoseGame()
    {
        gameOver = true;
        
        // Opcional: Cambiar sprite a puerta fallida o quemada
        if (imagenPuerta != null && puertaQuemada != null) 
            imagenPuerta.sprite = puertaQuemada;

        // Reportar derrota al GameManager global
        if (GameManager.Instance != null) 
        {
            GameManager.Instance.ReportarDerrota();
        }

        Debug.Log("Derrota: El fuego te alcanzó.");
    }

    // --------------------------------------------
}