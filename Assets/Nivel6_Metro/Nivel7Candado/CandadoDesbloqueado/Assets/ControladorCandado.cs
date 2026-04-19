using UnityEngine;
using UnityEngine.UI;

public class ControladorCandado : MonoBehaviour
{
    [Header("Configuración de la Aguja")]
    public RectTransform aguja;
    public float velocidad = 400f;
    public float limiteIzquierdo = -250f;
    public float limiteDerecho = 250f;

    [Header("Configuración del Objetivo")]
    public RectTransform lineaBlanca;
    public float margenError = 35f;

    [Header("Progreso")]
    public Image barraLlenado; 
    public GameObject mensajePerfecto;

    [Header("Sprites del Candado")]
    public Image imagenPrincipalCandado; 
    public Sprite spriteCerrado;
    public Sprite spriteAbriendo;
    public Sprite spriteAbierto;

    private float progresoActual = 0f;
    private bool moviendoDerecha = true;
    private bool juegoTerminado = false;

    void Start()
    {
        progresoActual = 0f;
        barraLlenado.fillAmount = 0f;
        mensajePerfecto.SetActive(false);
        imagenPrincipalCandado.sprite = spriteCerrado;
    }

    void Update()
    {
        if (juegoTerminado) return;

        MoverAguja();

        if (Input.GetMouseButtonDown(0))
        {
            ValidarClic();
        }
    }

    void MoverAguja()
    {
        float movimiento = velocidad * Time.deltaTime;

        if (moviendoDerecha)
        {
            aguja.anchoredPosition += new Vector2(movimiento, 0);
            if (aguja.anchoredPosition.x >= limiteDerecho) moviendoDerecha = false;
        }
        else
        {
            aguja.anchoredPosition -= new Vector2(movimiento, 0);
            if (aguja.anchoredPosition.x <= limiteIzquierdo) moviendoDerecha = true;
        }
    }

    void ValidarClic()
    {
        float distancia = Mathf.Abs(aguja.anchoredPosition.x - lineaBlanca.anchoredPosition.x);

        if (distancia <= margenError)
        {
            AumentarProgreso();
        }
    }

    void AumentarProgreso()
    {
        progresoActual += 0.35f; 
        barraLlenado.fillAmount = progresoActual;

        if (progresoActual >= 0.35f && progresoActual < 0.9f)
        {
            imagenPrincipalCandado.sprite = spriteAbriendo;
        }
        else if (progresoActual >= 0.9f)
        {
            GanarJuego();
        }

        StopAllCoroutines();
        StartCoroutine(FlashPerfecto());
    }

    System.Collections.IEnumerator FlashPerfecto()
    {
        mensajePerfecto.SetActive(true);
        yield return new WaitForSeconds(0.4f);
        mensajePerfecto.SetActive(false);
    }

    void GanarJuego()
    {
        juegoTerminado = true;
        barraLlenado.fillAmount = 1f;
        imagenPrincipalCandado.sprite = spriteAbierto;
        mensajePerfecto.SetActive(true);
        Debug.Log("¡Candado abierto!");
    }
}
