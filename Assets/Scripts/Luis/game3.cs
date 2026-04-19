using UnityEngine;
using TMPro; // Si usas texto TMP para el contador

public class QuickTapGame : MonoBehaviour
{
    [Header("Configuración")]
    public float timeLimit = 10f;     // Tiempo límite
    public int targetTaps = 20;      // Toques necesarios (ej. 20 para que coincida con los 5 sprites)

    [Header("Referencias de Assets (Globo)")]
    [SerializeField] private SpriteRenderer balloonRenderer; // El renderer del globo
    [SerializeField] private Sprite[] inflationSprites;    // Array de 5 sprites (2.1 a 2.5 de la imagen)

    [Header("Referencias de Assets (Feedback)")]
    [SerializeField] private GameObject handCursor;       // El objeto con el sprite de la mano (3.x)
    [SerializeField] private float handDisplayTime = 0.1f; // Cuánto tiempo visible la mano

    [Header("UI y Estado")]
    public GameObject winText;
    public GameObject loseText;
    [SerializeField] private TextMeshProUGUI tapCounterText; // Opcional: Texto contador (TAPS: 0/20)

    private int currentTaps = 0;
    private float timer;
    private bool isPlaying = true;
    private Vector3 initialScale;
    private float handTimer = 0f; // Temporizador interno para ocultar la mano

    void Start()
    {
        // Guardar escala inicial y configurar tiempo
        initialScale = balloonRenderer.transform.localScale;
        timer = timeLimit;

        // Configuración inicial de UI y Assets
        winText.SetActive(false);
        loseText.SetActive(false);
        if (handCursor != null) handCursor.SetActive(false); // Mano oculta al inicio
        
        // Asegurar que el globo empiece en el sprite 0 (2.1 deflado)
        if (balloonRenderer != null && inflationSprites.Length > 0)
        {
            balloonRenderer.sprite = inflationSprites[0];
        }

        UpdateUI(); // Actualizar texto inicial
    }

    void Update()
    {
        if (!isPlaying) return;

        // 1. Manejo del tiempo general
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            GameOver(false);
        }

        // 2. Temporizador para ocultar la mano de feedback
        if (handCursor != null && handCursor.activeSelf)
        {
            handTimer -= Time.deltaTime;
            if (handTimer <= 0)
            {
                handCursor.SetActive(false);
            }
        }

        // 3. Detección de Toques Rápidos (Touch)
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // Solo contamos el momento exacto en que pone el dedo (Began)
            if (touch.phase == TouchPhase.Began)
            {
                currentTaps++;
                
                // --- A) Feedback Visual de la Mano ---
                MostrarManoFeedback(touch.position);

                // --- B) Lógica de Inflado (Sprites) ---
                ActualizarSpriteGlobo();

                // --- C) Efecto visual extra (Escala) ---
                // Mantenemos un pequeño crecimiento de escala por tap para más dinamismo
                balloonRenderer.transform.localScale += new Vector3(0.01f, 0.01f, 0.01f);

                UpdateUI(); // Actualizar contador de UI

                // --- D) Condición de Victoria ---
                if (currentTaps >= targetTaps)
                {
                    GameOver(true);
                }
            }
        }
    }

    // Muestra la mano en la posición del tap por un breve momento
    void MostrarManoFeedback(Vector2 screenPosition)
    {
        if (handCursor == null || Camera.main == null) return;

        // Convertir posición de pantalla a mundo
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, -Camera.main.transform.position.z));
        worldPos.z = 0f; // Asegurar que esté en el plano 2D

        // Posicionar y activar la mano
        handCursor.transform.position = worldPos;
        handCursor.SetActive(true);
        handTimer = handDisplayTime; // Reiniciar temporizador de la mano
    }

    // Cambia el sprite del globo según el porcentaje de taps completados
    void ActualizarSpriteGlobo()
    {
        if (balloonRenderer == null || inflationSprites.Length == 0) return;

        // Calculamos qué índice de sprite usar (0 a 4 si hay 5 sprites)
        // Usamos una regla de tres simple basada en targetTaps
        float progressPercentage = (float)currentTaps / (float)targetTaps;
        
        // Multiplicamos por la longitud del array de sprites - 1
        int spriteIndex = Mathf.FloorToInt(progressPercentage * (inflationSprites.Length - 1));

        // Asegurar que el índice no se salga del rango (0 a 4)
        spriteIndex = Mathf.Clamp(spriteIndex, 0, inflationSprites.Length - 1);

        // Aplicar el nuevo sprite
        balloonRenderer.sprite = inflationSprites[spriteIndex];
    }

    void UpdateUI()
    {
        if (tapCounterText != null)
        {
            tapCounterText.text = $"TAPS: {currentTaps}/{targetTaps}";
        }
    }

    void GameOver(bool win)
    {
        isPlaying = false;
        if (win)
        {
            winText.SetActive(true);
            // Efecto extra: El globo explota (desactivamos el objeto)
            balloonRenderer.gameObject.SetActive(false); 
        }
        else
        {
            loseText.SetActive(true);
            // El globo vuelve a su tamaño pequeño y sprite inicial
            balloonRenderer.transform.localScale = initialScale;
            if (inflationSprites.Length > 0)
            {
                balloonRenderer.sprite = inflationSprites[0];
            }
        }
        
        // Ocultar mano al terminar
        if (handCursor != null) handCursor.SetActive(false);
    }
}