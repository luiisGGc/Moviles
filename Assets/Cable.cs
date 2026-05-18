using UnityEngine;
using UnityEngine.UI;

public class CableLoco : MonoBehaviour
{
    [Header("Configuracion de Movimiento")]
    public float tiempoEntreBrincos = 1.2f;
    public float limiteX = 400f;
    public float limiteY = 250f;
    public float tiempoLimite = 5.0f; 

    [Header("Referencias de UI")]
    public RectTransform objetivoCargador;
    public Image imagenCargador;
    public Sprite spriteConectado;
    public GameObject cartelVictoria;
    public GameObject cartelDerrota; 

    private float cronometroSalto;
    private float cronometroJuego;
    private bool agarrado = false;
    private bool juegoTerminado = false;
    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        cronometroJuego = tiempoLimite;

        if (cartelVictoria != null) cartelVictoria.SetActive(false);
        if (cartelDerrota != null) cartelDerrota.SetActive(false);
    }

    void Update()
    {
        if (juegoTerminado) return;
        cronometroJuego -= Time.deltaTime;
        if (cronometroJuego <= 0)
        {
            Derrota();
            return;
        }

        ManejarInput();

        if (!agarrado)
        {
            cronometroSalto += Time.deltaTime;
            if (cronometroSalto >= tiempoEntreBrincos)
            {
                Saltar();
                cronometroSalto = 0;
            }
        }
        else
        {
            Vector2 inputPos = Input.mousePosition;
            if (Input.touchCount > 0) inputPos = Input.GetTouch(0).position;

            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform.parent as RectTransform,
                inputPos,
                null,
                out localPoint);

            rectTransform.localPosition = localPoint;
            if (objetivoCargador != null)
            {
                float distancia = Vector2.Distance(rectTransform.localPosition, objetivoCargador.localPosition);
                if (distancia < 80f)
                {
                    Victoria();
                }
            }
        }
    }

    void ManejarInput()
    {
        bool interactuandoAhorita = false;
        bool soltandoAhorita = false;
        Vector2 inputPos = Vector2.zero;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            inputPos = touch.position;
            if (touch.phase == TouchPhase.Began) interactuandoAhorita = true;
            if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) soltandoAhorita = true;
        }

#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0)) { interactuandoAhorita = true; inputPos = Input.mousePosition; }
        if (Input.GetMouseButtonUp(0)) { soltandoAhorita = true; }
#endif

        if (interactuandoAhorita)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, inputPos, null))
            {
                agarrado = true;
            }
        }
        else if (soltandoAhorita)
        {
            agarrado = false;
        }
    }

    void Saltar()
    {
        float randomX = Random.Range(-limiteX, limiteX);
        float randomY = Random.Range(-limiteY, limiteY);
        rectTransform.localPosition = new Vector3(randomX, randomY, 0);
    }

    void Victoria()
    {
        juegoTerminado = true;
        agarrado = false;
        if (objetivoCargador != null)
            rectTransform.localPosition = objetivoCargador.localPosition;

        if (imagenCargador != null && spriteConectado != null)
        {
            imagenCargador.sprite = spriteConectado;
            gameObject.SetActive(false); 
        }

        if (cartelVictoria != null) cartelVictoria.SetActive(true);

        if (GameManager.Instance != null) GameManager.Instance.ReportarVictoria();
    }

    void Derrota()
    {
        juegoTerminado = true;
        agarrado = false;
        if (cartelDerrota != null) cartelDerrota.SetActive(true);
        if (GameManager.Instance != null) GameManager.Instance.ReportarDerrota();
    }
}