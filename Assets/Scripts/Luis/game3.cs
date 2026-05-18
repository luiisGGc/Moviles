using UnityEngine;
using TMPro;

public class QuickTapGame : MonoBehaviour
{
    [Header("Configuración")]
    public float timeLimit = 10f;
    public int targetTaps = 20;

    [Header("Referencias de Assets (Globo)")]
    [SerializeField] private SpriteRenderer balloonRenderer;
    [SerializeField] private Sprite[] inflationSprites;

    [Header("Referencias de Assets (Feedback)")]
    [SerializeField] private GameObject handCursor;
    [SerializeField] private float handDisplayTime = 0.1f;

    [Header("UI y Estado")]
    public GameObject winText;
    public GameObject loseText;
    [SerializeField] private TextMeshProUGUI tapCounterText;

    private int currentTaps = 0;
    private float timer;
    private bool isPlaying = true;
    private Vector3 initialScale;
    private float handTimer = 0f;

    void Start()
    {
        if (balloonRenderer != null) initialScale = balloonRenderer.transform.localScale;
        timer = timeLimit;

        if (winText != null) winText.SetActive(false);
        if (loseText != null) loseText.SetActive(false);
        if (handCursor != null) handCursor.SetActive(false);

        if (balloonRenderer != null && inflationSprites.Length > 0)
        {
            balloonRenderer.sprite = inflationSprites[0];
        }

        UpdateUI();
    }

    void Update()
    {
        if (!isPlaying) return;

        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            GameOver(false);
            return;
        }

        if (handCursor != null && handCursor.activeSelf)
        {
            handTimer -= Time.deltaTime;
            if (handTimer <= 0)
            {
                handCursor.SetActive(false);
            }
        }

        bool tapDetectado = false;
        Vector3 posicionTap = Vector3.zero;
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                tapDetectado = true;
                posicionTap = touch.position;
            }
        }

#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0))
        {
            tapDetectado = true;
            posicionTap = Input.mousePosition;
        }
#endif

        if (tapDetectado)
        {
            currentTaps++;
            MostrarManoFeedback(posicionTap);
            ActualizarSpriteGlobo();

            if (balloonRenderer != null)
                balloonRenderer.transform.localScale += new Vector3(0.01f, 0.01f, 0.01f);

            UpdateUI();

            if (currentTaps >= targetTaps)
            {
                GameOver(true);
            }
        }
    }

    void MostrarManoFeedback(Vector2 screenPosition)
    {
        if (handCursor == null || Camera.main == null) return;

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, -Camera.main.transform.position.z));
        worldPos.z = 0; 
        handCursor.transform.position = worldPos;
        handCursor.SetActive(true);
        handTimer = handDisplayTime;
    }

    void ActualizarSpriteGlobo()
    {
        if (balloonRenderer == null || inflationSprites.Length == 0) return;
        float progressPercentage = (float)currentTaps / (float)targetTaps;
        int spriteIndex = Mathf.FloorToInt(progressPercentage * (inflationSprites.Length - 1));
        spriteIndex = Mathf.Clamp(spriteIndex, 0, inflationSprites.Length - 1);
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
        if (handCursor != null) handCursor.SetActive(false);

        if (win)
        {
            if (winText != null) winText.SetActive(true);
            if (balloonRenderer != null) balloonRenderer.gameObject.SetActive(false);
            if (GameManager.Instance != null) GameManager.Instance.ReportarVictoria();
        }
        else
        {
            if (loseText != null) loseText.SetActive(true);
            if (balloonRenderer != null)
            {
                balloonRenderer.transform.localScale = initialScale;
                if (inflationSprites.Length > 0)
                {
                    balloonRenderer.sprite = inflationSprites[0];
                }
            }
            if (GameManager.Instance != null) GameManager.Instance.ReportarDerrota();
        }
    }
}