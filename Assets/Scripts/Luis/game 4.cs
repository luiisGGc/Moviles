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

        if (barraPresion != null) barraPresion.maxValue = presionNecesaria;

        // Se agregaron los chequeos de null por seguridad
        if (winText != null) winText.SetActive(false);
        if (loseText != null) loseText.SetActive(false);
        if (efectoExplosion != null) efectoExplosion.SetActive(false);
    }

    void Update()
    {
        if (!juegoActivo) return;

        cronometro -= Time.deltaTime;

        if (cronometro <= 0)
        {
            FinalizarJuego(false);
            return;
        }

        float aceleracion = Input.acceleration.magnitude;
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Space)) aceleracion = 10.0f;
#endif

        if (aceleracion > 1.5f)
        {
            presionActual += aceleracion * sensibilidadAgitado;
        }

        presionActual -= perdidaDePresion * Time.deltaTime;
        presionActual = Mathf.Clamp(presionActual, 0, presionNecesaria);

        if (barraPresion != null) barraPresion.value = presionActual;
        ActualizarVisuales();

        if (presionActual >= presionNecesaria)
        {
            FinalizarJuego(true);
        }
    }

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

    void FinalizarJuego(bool victoria)
    {
        juegoActivo = false;
        sodaRenderer.transform.localPosition = posicionOriginal;

        if (victoria)
        {
            if (winText != null) winText.SetActive(true);

            sodaRenderer.gameObject.SetActive(false);
            if (efectoExplosion != null) efectoExplosion.SetActive(true);
            Handheld.Vibrate();

            if (GameManager.Instance != null) GameManager.Instance.ReportarVictoria();
        }
        else
        {
            if (loseText != null) loseText.SetActive(true);

            if (GameManager.Instance != null) GameManager.Instance.ReportarDerrota();
        }
    }
}