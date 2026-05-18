using UnityEngine;
using UnityEngine.UI;

public class LogicaFugaGas : MonoBehaviour
{
    [Header("Configuraciones")]
    public float velocidadReparacion = 0.25f;
    public float velocidadFuga = 0.15f;
    public float tiempoLimite = 5.0f; 

    [Header("Referencias Visuales")]
    public Image barraProgreso;
    public Image imagenTuberia;
    public Sprite tuberiaRota;
    public Sprite tuberiaArreglada;

    [Header("UI de Victoria/Derrota")]
    public GameObject cartelVictoria;
    public GameObject cartelDerrota; 

    private float progreso = 0f;
    private bool completado = false;
    private float cronometro;

    void Start()
    {
        cronometro = tiempoLimite;

        if (cartelVictoria != null) cartelVictoria.SetActive(false);
        if (cartelDerrota != null) cartelDerrota.SetActive(false);

        if (imagenTuberia != null && tuberiaRota != null)
            imagenTuberia.sprite = tuberiaRota;

        if (barraProgreso != null)
            barraProgreso.fillAmount = 0f;
    }

    void Update()
    {
        if (completado) return;

        cronometro -= Time.deltaTime;
        if (cronometro <= 0)
        {
            Derrota();
            return;
        }

        bool estaReparando = false;

        if (Input.touchCount > 0)
        {
            estaReparando = true;
        }

#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButton(0))
        {
            estaReparando = true;
        }
#endif

        if (estaReparando)
        {
            progreso += velocidadReparacion * Time.deltaTime;
        }
        else
        {
            if (progreso > 0) progreso -= velocidadFuga * Time.deltaTime;
        }

        progreso = Mathf.Clamp(progreso, 0f, 1f);
        if (barraProgreso != null) barraProgreso.fillAmount = progreso;
        if (progreso >= 1f)
        {
            Victoria();
        }
    }

    void Victoria()
    {
        completado = true;

        if (imagenTuberia != null && tuberiaArreglada != null)
            imagenTuberia.sprite = tuberiaArreglada;

        if (cartelVictoria != null) cartelVictoria.SetActive(true);
        if (GameManager.Instance != null) GameManager.Instance.ReportarVictoria();
    }

    void Derrota()
    {
        completado = true;

        if (cartelDerrota != null) cartelDerrota.SetActive(true);
        if (GameManager.Instance != null) GameManager.Instance.ReportarDerrota();
    }
}