using UnityEngine;
using UnityEngine.UI;

public class SodaExplosionGame : MonoBehaviour
{
    [Header("Ajustes de Presión")]
    public float sensibilidadAgitado = 2.5f;
    public float presionNecesaria = 100f;
    public float perdidaDePresion = 15f; 
    public float tiempoLimite = 5.0f;

    [Header("Referencias Visuales")]
    public SpriteRenderer sodaRenderer;
    public Sprite[] spritesPresion; 
    public GameObject efectoExplosion; 
    public Slider barraPresion;

    private float presionActual = 0f;
    private float cronometro;
    private bool gameOver = false; // Variable de control de estado
    private Vector3 posicionOriginal;

    void Start()
    {
        cronometro = tiempoLimite;
        posicionOriginal = sodaRenderer.transform.localPosition;
        if(barraPresion != null) barraPresion.maxValue = presionNecesaria;
        
        if(efectoExplosion != null) efectoExplosion.SetActive(false);
    }

    void Update()
    {
        if (gameOver) return; // Si el juego terminó, se detiene la lógica

        cronometro -= Time.deltaTime;
        if (cronometro <= 0) LoseGame();

        float aceleracion = Input.acceleration.magnitude;
        if (aceleracion > 1.5f) 
        {
            presionActual += aceleracion * sensibilidadAgitado;
        }

        presionActual -= perdidaDePresion * Time.deltaTime;
        presionActual = Mathf.Clamp(presionActual, 0, presionNecesaria);

        if(barraPresion != null) barraPresion.value = presionActual;

        ActualizarVisuales();

        if (presionActual >= presionNecesaria)
        {
            WinGame();
        }
    }

    // --- ÚNICAMENTE LAS FUNCIONES SOLICITADAS ---

    private void WinGame()
    {
        gameOver = true;
        sodaRenderer.transform.localPosition = posicionOriginal;
        sodaRenderer.gameObject.SetActive(false); 
        
        if(efectoExplosion != null) efectoExplosion.SetActive(true); 
        Handheld.Vibrate(); 

        // Reportar victoria al GameManager global
        if (GameManager.Instance != null) 
        {
            GameManager.Instance.ReportarVictoria();
        }
        
        Debug.Log("Victoria: Soda explotada.");
    }

    private void LoseGame()
    {
        gameOver = true;
        sodaRenderer.transform.localPosition = posicionOriginal;

        // Reportar derrota al GameManager global
        if (GameManager.Instance != null) 
        {
            GameManager.Instance.ReportarDerrota();
        }

        Debug.Log("Derrota: Tiempo agotado.");
    }

    // --------------------------------------------

    void ActualizarVisuales()
    {
        if (spritesPresion.Length > 0)
        {
            int index = Mathf.FloorToInt((presionActual / presionNecesaria) * (spritesPresion.Length - 1));
            sodaRenderer.sprite = spritesPresion[Mathf.Clamp(index, 0, spritesPresion.Length - 1)];
        }

        float intensidad = (presionActual / presionNecesaria) * 0.25f;
        sodaRenderer.transform.localPosition = posicionOriginal + (Vector3)Random.insideUnitCircle * intensidad;
    }
}