using UnityEngine;
using UnityEngine.UI;

public class LightChargeGame : MonoBehaviour
{
    [Header("Configuración")]
    public float cargaNecesaria = 100f;
    public float velocidadDescarga = 15f;
    public float fuerzaFrotado = 2f; 
    public float tiempoLimite = 10f; // Añadido para tener condición de derrota
    
    [Header("Referencias")]
    public SpriteRenderer personajeRenderer;
    public Sprite spriteFrio;    
    public Sprite spriteFeliz;   
    public Slider barraCarga;

    private float cargaActual = 0f;
    private float cronometro;
    private bool gameOver = false; // Variable de control de estado
    private Vector2 ultimaPosicionTouch;

    void Start()
    {
        cronometro = tiempoLimite;
        if (personajeRenderer != null) personajeRenderer.sprite = spriteFrio;
        if (barraCarga != null) barraCarga.maxValue = cargaNecesaria;
    }

    void Update()
    {
        if (gameOver) return; // Si el juego terminó, se detiene la lógica

        // Lógica de tiempo
        cronometro -= Time.deltaTime;
        if (cronometro <= 0) LoseGame();

        bool estaFrotando = false;

        // DETECTAR FRICCIÓN
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            
            if (touch.phase == TouchPhase.Moved)
            {
                float distanciaMovida = touch.deltaPosition.magnitude;
                
                if (distanciaMovida > 5f) 
                {
                    cargaActual += distanciaMovida * fuerzaFrotado * Time.deltaTime;
                    estaFrotando = true;
                }
            }
        }

        if (!estaFrotando)
        {
            cargaActual -= velocidadDescarga * Time.deltaTime;
        }

        cargaActual = Mathf.Clamp(cargaActual, 0, cargaNecesaria);
        if (barraCarga != null) barraCarga.value = cargaActual;

        ActualizarEstado();

        if (cargaActual >= cargaNecesaria) WinGame();
    }

    // --- ÚNICAMENTE LAS FUNCIONES SOLICITADAS ---

    private void WinGame()
    {
        gameOver = true;
        
        if (personajeRenderer != null)
        {
            personajeRenderer.sprite = spriteFeliz;
            personajeRenderer.color = Color.yellow;
        }

        // Reportar victoria al GameManager global
        if (GameManager.Instance != null) 
        {
            GameManager.Instance.ReportarVictoria();
        }
        
        Debug.Log("Victoria: Carga completada.");
    }

    private void LoseGame()
    {
        gameOver = true;

        // Reportar derrota al GameManager global
        if (GameManager.Instance != null) 
        {
            GameManager.Instance.ReportarDerrota();
        }

        Debug.Log("Derrota: Tiempo agotado.");
    }

    // --------------------------------------------

    void ActualizarEstado()
    {
        if (personajeRenderer == null) return;

        if (cargaActual > cargaNecesaria * 0.6f) 
            personajeRenderer.sprite = spriteFeliz;
        else 
            personajeRenderer.sprite = spriteFrio;

        personajeRenderer.color = Color.Lerp(new Color(0.6f, 0.8f, 1f), Color.white, cargaActual / cargaNecesaria);
    }
}