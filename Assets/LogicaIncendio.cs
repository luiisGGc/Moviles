using UnityEngine;
using UnityEngine.UI;

public class LogicaIncendio : MonoBehaviour
{
    [Header("Configuraciones")]
    public float tiempoLimite = 5.0f; 
    public float velocidadDescenso = 0.15f;
    public float fuerzaClick = 0.08f;

    [Header("UI de Progreso")]
    public Image barraProgreso;

    [Header("Sprites de la Puerta")]
    public Image imagenPuerta;
    public Sprite puertaConFuego;
    public Sprite puertaAbierta;

    [Header("UI de Victoria/Derrota")]
    public GameObject mensajeGanaste;
    public GameObject mensajePerdiste; 

    private float progresoActual = 0f;
    private bool juegoTerminado = false;
    private float cronometro;

    void Start()
    {
        cronometro = tiempoLimite;
        progresoActual = 0f;

        if (barraProgreso != null) barraProgreso.fillAmount = 0f;
        if (imagenPuerta != null && puertaConFuego != null) imagenPuerta.sprite = puertaConFuego;

        if (mensajeGanaste != null) mensajeGanaste.SetActive(false);
        if (mensajePerdiste != null) mensajePerdiste.SetActive(false);
    }

    void Update()
    {
        if (juegoTerminado) return;

        cronometro -= Time.deltaTime;
        if (cronometro <= 0)
        {
            Perder();
            return;
        }

        if (progresoActual > 0)
        {
            progresoActual -= velocidadDescenso * Time.deltaTime;
        }

        bool toqueDetectado = false;
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                toqueDetectado = true;
            }
        }

#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0))
        {
            toqueDetectado = true;
        }
#endif

        if (toqueDetectado)
        {
            progresoActual += fuerzaClick;
        }

        progresoActual = Mathf.Clamp(progresoActual, 0f, 1f);
        if (barraProgreso != null) barraProgreso.fillAmount = progresoActual;
        if (progresoActual >= 1f)
        {
            Ganar();
        }
    }

    void Ganar()
    {
        juegoTerminado = true;

        if (imagenPuerta != null && puertaAbierta != null)
            imagenPuerta.sprite = puertaAbierta;

        if (mensajeGanaste != null) mensajeGanaste.SetActive(true);
        if (GameManager.Instance != null) GameManager.Instance.ReportarVictoria();
    }

    void Perder()
    {
        juegoTerminado = true;

        if (mensajePerdiste != null) mensajePerdiste.SetActive(true);
        if (GameManager.Instance != null) GameManager.Instance.ReportarDerrota();
    }
}