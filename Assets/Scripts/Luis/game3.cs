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
    [SerializeField] private TextMeshProUGUI tapCounterText; 

    private int currentTaps = 0;
    private float timer;
    private bool gameOver = false; 
    private Vector3 initialScale;
    private float handTimer = 0f; 

    void Start()
    {
        initialScale = balloonRenderer.transform.localScale;
        timer = timeLimit;

        if (handCursor != null) handCursor.SetActive(false); 
        
        if (balloonRenderer != null && inflationSprites.Length > 0)
        {
            balloonRenderer.sprite = inflationSprites[0];
        }

        UpdateUI(); 
    }

    void Update()
    {
        if (gameOver) return; 

        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            LoseGame();
        }

        if (handCursor != null && handCursor.activeSelf)
        {
            handTimer -= Time.deltaTime;
            if (handTimer <= 0) handCursor.SetActive(false);
        }

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                currentTaps++;
                MostrarManoFeedback(touch.position);
                ActualizarSpriteGlobo();
                balloonRenderer.transform.localScale += new Vector3(0.01f, 0.01f, 0.01f);
                UpdateUI();

                if (currentTaps >= targetTaps)
                {
                    WinGame();
                }
            }
        }
    }


    private void WinGame()
    {
        gameOver = true;

        // Feedback visual: El globo explota
        if (balloonRenderer != null) balloonRenderer.gameObject.SetActive(false); 
        if (handCursor != null) handCursor.SetActive(false);

        // Reportar al GameManager
        if (GameManager.Instance != null) 
        {
            GameManager.Instance.ReportarVictoria();
        }
        
        Debug.Log("Victoria: Globo inflado.");
    }

    private void LoseGame()
    {
        gameOver = true;

        // Feedback visual: Reset del globo
        if (balloonRenderer != null)
        {
            balloonRenderer.transform.localScale = initialScale;
            if (inflationSprites.Length > 0) balloonRenderer.sprite = inflationSprites[0];
        }
        if (handCursor != null) handCursor.SetActive(false);

        // Reportar al GameManager
        if (GameManager.Instance != null) 
        {
            GameManager.Instance.ReportarDerrota();
        }

        Debug.Log("Derrota: Tiempo agotado.");
    }

    // ------------------------------------------

    void MostrarManoFeedback(Vector2 screenPosition)
    {
        if (handCursor == null || Camera.main == null) return;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, -Camera.main.transform.position.z));
        worldPos.z = 0f;
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
            tapCounterText.text = $"TAPS: {currentTaps}/{targetTaps}";
    }
}