using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MecanicaMetro : MonoBehaviour
{
    private Vector2 posicionInicial;
    private Vector2 posicionFinal;

    [Header("Swipe")]
    public float distanciaMinimaSwipe = 50f;

    [Header("Sprites de las Puertas")]
    public GameObject Puerta_Izq;
    public GameObject Puerta_Der;

    [Header("Flechas")]
    public GameObject grupoTutorial;
    public GameObject textoVictoria;
    public GameObject textoPerdiste;

    [Header("Tiempo (Reloj)")]
    public float tiempoMaximo = 10f;
    private float tiempoActual;
    public Image barraDeTiempo;
    public TextMeshProUGUI textoReloj;

    [Header("Movimiento")]
    public float velocidadCierre = 10f;
    public float distanciaApertura = 1.2f;

    private Vector3 posCerradaIzquierda;
    private Vector3 posCerradaDerecha;
    private bool juegoGanado = false;
    private bool juegoTerminado = false;

    private bool reportadoAlGameManager = false;

    void Awake()
    {
        if (Puerta_Izq != null)
        {
            Puerta_Izq.transform.SetParent(null);
            posCerradaIzquierda = Puerta_Izq.transform.localPosition;
        }
        if (Puerta_Der != null)
        {
            Puerta_Der.transform.SetParent(null);
            posCerradaDerecha = Puerta_Der.transform.localPosition;
        }
    }

    void Start()
    {
        if (Puerta_Izq != null) Puerta_Izq.transform.localPosition = posCerradaIzquierda + new Vector3(-distanciaApertura, 0, 0);
        if (Puerta_Der != null) Puerta_Der.transform.localPosition = posCerradaDerecha + new Vector3(distanciaApertura, 0, 0);

        tiempoActual = tiempoMaximo;

        if (textoVictoria) textoVictoria.SetActive(false);
        if (textoPerdiste) textoPerdiste.SetActive(false);
    }

    void Update()
    {
        if (juegoTerminado) return;

        if (juegoGanado)
        {
            CerrarPuertasAnimacion();
            return;
        }

        ManejarCronometro();

        if (juegoTerminado) return;

        bool inicioToque = false;
        bool finToque = false;
        Vector2 inputPos = Vector2.zero;

#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0)) { inicioToque = true; inputPos = Input.mousePosition; }
        if (Input.GetMouseButtonUp(0)) { finToque = true; inputPos = Input.mousePosition; }
#else
        // Soporte Celular
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began) { inicioToque = true; inputPos = touch.position; }
            if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) { finToque = true; inputPos = touch.position; }
        }
#endif

        if (inicioToque)
        {
            posicionInicial = inputPos;
        }
        else if (finToque)
        {
            posicionFinal = inputPos;
            AnalizarSwipe();
        }
    }

    void ManejarCronometro()
    {
        tiempoActual -= Time.deltaTime;

        if (barraDeTiempo != null)
        {
            barraDeTiempo.fillAmount = tiempoActual / tiempoMaximo;
        }

        if (textoReloj != null)
        {
            textoReloj.text = tiempoActual.ToString("F0") + "s";
        }

        if (tiempoActual <= 0 && !juegoGanado)
        {
            tiempoActual = 0;
            juegoTerminado = true;

            if (grupoTutorial) grupoTutorial.SetActive(false);
            if (textoPerdiste) textoPerdiste.SetActive(true);

            if (!reportadoAlGameManager && GameManager.Instance != null)
            {
                reportadoAlGameManager = true;
                GameManager.Instance.ReportarDerrota();
            }
        }
    }

    void AnalizarSwipe()
    {
        float distanciaX = Mathf.Abs(posicionFinal.x - posicionInicial.x);

        if (distanciaX > distanciaMinimaSwipe)
        {
            juegoGanado = true;

            if (grupoTutorial) grupoTutorial.SetActive(false);
        }
    }

    void CerrarPuertasAnimacion()
    {
        if (Puerta_Izq == null || Puerta_Der == null) return;

        Puerta_Izq.transform.localPosition = Vector3.MoveTowards(Puerta_Izq.transform.localPosition, posCerradaIzquierda, velocidadCierre * Time.deltaTime);
        Puerta_Der.transform.localPosition = Vector3.MoveTowards(Puerta_Der.transform.localPosition, posCerradaDerecha, velocidadCierre * Time.deltaTime);

        if (Vector3.Distance(Puerta_Izq.transform.localPosition, posCerradaIzquierda) < 0.01f)
        {
            Puerta_Izq.transform.localPosition = posCerradaIzquierda;
            Puerta_Der.transform.localPosition = posCerradaDerecha;

            if (textoVictoria) textoVictoria.SetActive(true);
            if (!reportadoAlGameManager && GameManager.Instance != null)
            {
                reportadoAlGameManager = true;
                GameManager.Instance.ReportarVictoria();
            }

            enabled = false; // Apaga el script
        }
    }
}