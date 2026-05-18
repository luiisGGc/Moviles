using UnityEngine;
using UnityEngine.UI;

public class LightChargeGame : MonoBehaviour
{
    [Header("Configuración")]
    public float cargaNecesaria = 100f;
    public float velocidadDescarga = 15f;
    public float fuerzaFrotado = 2f;
    public float tiempoLimite = 5.0f; // NUEVO: Tiempo límite para ganar

    [Header("Referencias")]
    public SpriteRenderer personajeRenderer;
    public Sprite spriteFrio;
    public Sprite spriteFeliz;
    public Slider barraCarga;
    public GameObject winText;
    public GameObject loseText;

    private float cargaActual = 0f;
    private bool juegoActivo = true;
    private float cronometro; 

    void Start()
    {
        cronometro = tiempoLimite;

        if (winText != null) winText.SetActive(false);
        if (loseText != null) loseText.SetActive(false);
        if (personajeRenderer != null) personajeRenderer.sprite = spriteFrio;
        if (barraCarga != null) barraCarga.maxValue = cargaNecesaria;
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

        bool estaFrotando = false;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved)
            {
                float distanciaMovida = touch.deltaPosition.magnitude;

                if (distanciaMovida > 5f)
                {
                    cargaActual += distanciaMovida * fuerzaFrotado * Time.deltaTime;
                    estaFrotando = true;
                }
            }
        }

#if UNITY_EDITOR
        if (Input.GetMouseButton(0))
        {
            float mouseDist = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")).magnitude;
            if (mouseDist > 0.1f)
            {
                cargaActual += mouseDist * fuerzaFrotado * 500f * Time.deltaTime;
                estaFrotando = true;
            }
        }
#endif

        if (!estaFrotando)
        {
            cargaActual -= velocidadDescarga * Time.deltaTime;
        }

        cargaActual = Mathf.Clamp(cargaActual, 0, cargaNecesaria);
        if (barraCarga != null) barraCarga.value = cargaActual;

        ActualizarEstado();

        if (cargaActual >= cargaNecesaria)
        {
            FinalizarJuego(true); 
        }
    }

    void ActualizarEstado()
    {
        if (personajeRenderer == null) return;

        if (cargaActual > cargaNecesaria * 0.6f)
            personajeRenderer.sprite = spriteFeliz;
        else
            personajeRenderer.sprite = spriteFrio;

        personajeRenderer.color = Color.Lerp(new Color(0.6f, 0.8f, 1f), Color.white, cargaActual / cargaNecesaria);
    }

    void FinalizarJuego(bool victoria)
    {
        juegoActivo = false;

        if (victoria)
        {
            if (winText != null) winText.SetActive(true);
            if (personajeRenderer != null)
            {
                personajeRenderer.sprite = spriteFeliz;
                personajeRenderer.color = Color.yellow;
            }
            if (GameManager.Instance != null) GameManager.Instance.ReportarVictoria();
        }
        else
        {
            if (loseText != null) loseText.SetActive(true);
            if (GameManager.Instance != null) GameManager.Instance.ReportarDerrota();
        }
    }
}