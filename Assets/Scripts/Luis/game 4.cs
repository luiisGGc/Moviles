using UnityEngine;
using UnityEngine.UI;

public class SodaExplosionGame : MonoBehaviour
{
    [Header("Ajustes de Presión")]
    public float sensibilidadAgitado = 2.5f;
    public float presionNecesaria = 100f;
    public float perdidaDePresion = 15f; // La presión baja si dejas de agitar
    public float tiempoLimite = 5.0f;

    [Header("Referencias Visuales")]
    public SpriteRenderer sodaRenderer;
    public Sprite[] spritesPresion; // Sprites de la botella normal a inflada
    public GameObject efectoExplosion; // Partículas de espuma o confeti
    public Slider barraPresion;
    public GameObject winText;
    public GameObject loseText;

    private float presionActual = 0f;
    private float cronometro;
    private bool juegoActivo = true;
    private Vector3 posicionOriginal;

    void Start()
    {
        cronometro = tiempoLimite;
        posicionOriginal = sodaRenderer.transform.localPosition;
        if(barraPresion != null) barraPresion.maxValue = presionNecesaria;
        
        winText.SetActive(false);
        loseText.SetActive(false);
        if(efectoExplosion != null) efectoExplosion.SetActive(false);
    }

    void Update()
    {
        if (!juegoActivo) return;

        cronometro -= Time.deltaTime;
        if (cronometro <= 0) FinalizarJuego(false);

        // 1. Detectar agitación del acelerómetro
        float aceleracion = Input.acceleration.magnitude;
        if (aceleracion > 1.5f) 
        {
            presionActual += aceleracion * sensibilidadAgitado;
        }

        // 2. La presión baja si el jugador se detiene
        presionActual -= perdidaDePresion * Time.deltaTime;
        presionActual = Mathf.Clamp(presionActual, 0, presionNecesaria);

        if(barraPresion != null) barraPresion.value = presionActual;

        // 3. Efectos Visuales
        ActualizarVisuales();

        // 4. Victoria
        if (presionActual >= presionNecesaria)
        {
            FinalizarJuego(true);
        }
    }

    void ActualizarVisuales()
    {
        // Cambiar sprite según la presión (si tienes 3 sprites: normal, tenso, a punto de explotar)
        if (spritesPresion.Length > 0)
        {
            int index = Mathf.FloorToInt((presionActual / presionNecesaria) * (spritesPresion.Length - 1));
            sodaRenderer.sprite = spritesPresion[Mathf.Clamp(index, 0, spritesPresion.Length - 1)];
        }

        // Vibración visual según la presión acumulada
        float intensidad = (presionActual / presionNecesaria) * 0.25f;
        sodaRenderer.transform.localPosition = posicionOriginal + (Vector3)Random.insideUnitCircle * intensidad;
    }

    void FinalizarJuego(bool victoria)
    {
        juegoActivo = false;
        sodaRenderer.transform.localPosition = posicionOriginal;

        if (victoria)
        {
            winText.SetActive(true);
            sodaRenderer.gameObject.SetActive(false); // La botella desaparece/vuela
            if(efectoExplosion != null) efectoExplosion.SetActive(true); // ¡BOOM de espuma!
            Handheld.Vibrate(); // Vibración final fuerte
        }
        else
        {
            loseText.SetActive(true);
        }
    }
}