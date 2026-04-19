using UnityEngine;
using UnityEngine.Events;

public class Sopa : MonoBehaviour
{
    [Header("Config")]
    public float threshold = 0.005f;
    public float timeLimit = 5.0f;
    public float TiempoReq = 1.5f;

    private float tiempoAct = 0f;
    private float timer = 0f;
    private bool gameOver = false;

    [Header("Niñoyfuego")]
    public SpriteRenderer boyRenderer;
    public Sprite spriteNormal;        
    public Sprite spriteSoplando;      
    public Sprite spriteGanar;         
    public Sprite spritePerder;        
    public Transform fuegoTransform;   
    private Vector3 escalaFuego;


    void Start()
    {
        if (fuegoTransform != null)
        {
            escalaFuego = fuegoTransform.localScale;
        }

        // Asegurarnos de que inicie con la cara correcta
        if (boyRenderer != null) boyRenderer.sprite = spriteNormal;
    }

    void Update()
    {
        if (gameOver) return;

        timer += Time.deltaTime;
        if (timer >= timeLimit)
        {
            LoseGame();
            return;
        }

        float currentVolume = InputMicGlobal.Instance.VolumenActual;

        if (currentVolume > threshold)
        {
            tiempoAct += Time.deltaTime;

            if (boyRenderer != null) boyRenderer.sprite = spriteSoplando;

            if (fuegoTransform != null)
            {
                float progreso = tiempoAct / TiempoReq;
                fuegoTransform.localScale = Vector3.Lerp(escalaFuego, Vector3.zero, progreso);
            }

            if (tiempoAct >= TiempoReq)
            {
                WinGame();
            }
        }
        else
        {
            if (boyRenderer != null) boyRenderer.sprite = spriteNormal;
        }
    }

    private void WinGame()
    {
        gameOver = true;
        if (boyRenderer != null) boyRenderer.sprite = spriteGanar;
        if (fuegoTransform != null) fuegoTransform.gameObject.SetActive(false); 
        if (GameManager.Instance != null)
            GameManager.Instance.ReportarVictoria();
    }

    private void LoseGame()
    {
        gameOver = true;
        if (boyRenderer != null) boyRenderer.sprite = spritePerder;
        if (GameManager.Instance != null)
            GameManager.Instance.ReportarDerrota();
    }
}