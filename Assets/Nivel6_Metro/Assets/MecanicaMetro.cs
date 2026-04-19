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

    void Awake()
    {
        if (Puerta_Izq != null) Puerta_Izq.transform.SetParent(null);
        if (Puerta_Der != null) Puerta_Der.transform.SetParent(null);

        posCerradaIzquierda = Puerta_Izq.transform.localPosition;
        posCerradaDerecha = Puerta_Der.transform.localPosition;
    }

    void Start()
    {
        Puerta_Izq.transform.localPosition = posCerradaIzquierda + new Vector3(-distanciaApertura, 0, 0);
        Puerta_Der.transform.localPosition = posCerradaDerecha + new Vector3(distanciaApertura, 0, 0);

        tiempoActual = tiempoMaximo;

        if (textoVictoria) textoVictoria.SetActive(false);
        if (textoPerdiste) textoPerdiste.SetActive(false);
    }

    void Update()
    {
        if (juegoGanado || juegoTerminado)
        {
            if (juegoGanado) CerrarPuertasAnimacion();
            return;
        }

        ManejarCronometro();

        if (Input.GetMouseButtonDown(0))
        {
            posicionInicial = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            posicionFinal = Input.mousePosition;
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

        if (tiempoActual <= 0)
        {
            tiempoActual = 0;
            juegoTerminado = true;
            if (grupoTutorial) grupoTutorial.SetActive(false);
            if (textoPerdiste) textoPerdiste.SetActive(true);
            Debug.Log("Tiempo agotado.");
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
        Puerta_Izq.transform.localPosition = Vector3.MoveTowards(Puerta_Izq.transform.localPosition, posCerradaIzquierda, velocidadCierre * Time.deltaTime);
        Puerta_Der.transform.localPosition = Vector3.MoveTowards(Puerta_Der.transform.localPosition, posCerradaDerecha, velocidadCierre * Time.deltaTime);

        if (Vector3.Distance(Puerta_Izq.transform.localPosition, posCerradaIzquierda) < 0.01f)
        {
            Puerta_Izq.transform.localPosition = posCerradaIzquierda;
            Puerta_Der.transform.localPosition = posCerradaDerecha;

            if (textoVictoria) textoVictoria.SetActive(true);

            Debug.Log("¡Victoria! Nivel completado.");

            enabled = false;
        }
    }
}