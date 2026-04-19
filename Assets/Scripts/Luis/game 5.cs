using UnityEngine;
using UnityEngine.UI;

public class LightChargeGame : MonoBehaviour
{
    [Header("Configuración")]
    public float cargaNecesaria = 100f;
    public float velocidadDescarga = 15f;
    public float fuerzaFrotado = 2f; // Qué tanto carga por cada movimiento
    
    [Header("Referencias")]
    public SpriteRenderer personajeRenderer;
    public Sprite spriteFrio;    
    public Sprite spriteFeliz;   
    public Slider barraCarga;
    public GameObject winText;

    private float cargaActual = 0f;
    private bool juegoActivo = true;
    private Vector2 ultimaPosicionTouch;

    void Start()
    {
        if (winText != null) winText.SetActive(false);
        if (personajeRenderer != null) personajeRenderer.sprite = spriteFrio;
        if (barraCarga != null) barraCarga.maxValue = cargaNecesaria;
    }

    void Update()
    {
        if (!juegoActivo) return;

        bool estaFrotando = false;

        // DETECTAR FRICCIÓN (Movimiento rápido del dedo)
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            
            if (touch.phase == TouchPhase.Moved)
            {
                // Calculamos cuánto se movió el dedo desde el frame anterior
                float distanciaMovida = touch.deltaPosition.magnitude;
                
                if (distanciaMovida > 5f) // Si el movimiento es lo suficientemente rápido
                {
                    cargaActual += distanciaMovida * fuerzaFrotado * Time.deltaTime;
                    estaFrotando = true;
                }
            }
        }

        // LÓGICA DE DESCARGA (Si no frotas, se enfría)
        if (!estaFrotando)
        {
            cargaActual -= velocidadDescarga * Time.deltaTime;
        }

        // ACTUALIZAR VISUALES
        cargaActual = Mathf.Clamp(cargaActual, 0, cargaNecesaria);
        if (barraCarga != null) barraCarga.value = cargaActual;

        ActualizarEstado();

        if (cargaActual >= cargaNecesaria) FinalizarJuego();
    }

    void ActualizarEstado()
    {
        if (personajeRenderer == null) return;

        if (cargaActual > cargaNecesaria * 0.6f) 
            personajeRenderer.sprite = spriteFeliz;
        else 
            personajeRenderer.sprite = spriteFrio;

        // El color cambia de azul frío a blanco/amarillo caliente
        personajeRenderer.color = Color.Lerp(new Color(0.6f, 0.8f, 1f), Color.white, cargaActual / cargaNecesaria);
    }

    void FinalizarJuego()
    {
        juegoActivo = false;
        if (winText != null) winText.SetActive(true);
        if (personajeRenderer != null)
        {
            personajeRenderer.sprite = spriteFeliz;
            personajeRenderer.color = Color.yellow;
        }
    }
}