using UnityEngine;
using UnityEngine.UI;

public class LogicaIncendio : MonoBehaviour
{
    [Header("UI de Progreso")]
    public Image barraProgreso;
    public float velocidadDescenso = 0.15f;
    public float fuerzaClick = 0.08f;

    [Header("Sprites de la Puerta")]
    public Image imagenPuerta;
    public Sprite puertaConFuego;
    public Sprite puertaAbierta;

    [Header("Ganar")]
    public GameObject mensajeGanaste;

    private float progresoActual = 0f;
    private bool juegoTerminado = false;

    void Start()
    {
        progresoActual = 0f;
        barraProgreso.fillAmount = 0f;
        imagenPuerta.sprite = puertaConFuego;
        mensajeGanaste.SetActive(false);
    }

    void Update()
    {
        if (juegoTerminado) return;

        if (progresoActual > 0)
        {
            progresoActual -= velocidadDescenso * Time.deltaTime;
        }

        if (Input.GetMouseButtonDown(0))
        {
            progresoActual += fuerzaClick;
        }

        progresoActual = Mathf.Clamp(progresoActual, 0f, 1f);

        barraProgreso.fillAmount = progresoActual;

        if (progresoActual >= 1f)
        {
            Ganar();
        }
    }

    void Ganar()
    {
        juegoTerminado = true;
        imagenPuerta.sprite = puertaAbierta;
        mensajeGanaste.SetActive(true);
        Debug.Log("¡Escapaste del incendio!");
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ReportarVictoria();
        }
    }
}