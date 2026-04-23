using UnityEngine;
using TMPro;

public sealed class GyroBalance : MonoBehaviour
{
    [Header("Configuración de Equilibrio")]
    [SerializeField] private float sensitivity = 50.0f; 
    [SerializeField] private float gravityForce = 60f; 
    [SerializeField] private float maxAngle = 45f;
    [SerializeField] private float tiempoParaGanar = 10f; // Tiempo que debe aguantar

    [Header("Sprites de Estado")]
    [SerializeField] private SpriteRenderer characterRenderer; 
    [SerializeField] private Sprite spriteNormal;   
    [SerializeField] private Sprite spritePreocupado; 
    [SerializeField] private Sprite spriteSudando;
    [SerializeField] private Sprite spriteGanar;   // Sprite de victoria
    [SerializeField] private Sprite spritePerder;  // Sprite de derrota

    private float currentZRotation = 0f;
    private float tiempoTranscurrido = 0f;
    private bool gameOver = false; // Cambiado a minúscula para coincidir con tu estructura

    void Start()
    {
        Input.gyro.enabled = true;
        if (characterRenderer == null) characterRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (gameOver) return;

        // Lógica de tiempo para ganar
        tiempoTranscurrido += Time.deltaTime;
        if (tiempoTranscurrido >= tiempoParaGanar)
        {
            WinGame();
        }

        float inputX = Input.gyro.enabled ? Input.gyro.rotationRateUnbiased.z : Input.GetAxis("Horizontal");

        currentZRotation += inputX * sensitivity * Time.deltaTime;
        float gravityEffect = (currentZRotation / maxAngle) * gravityForce;
        currentZRotation += gravityEffect * Time.deltaTime;
        currentZRotation = Mathf.Clamp(currentZRotation, -maxAngle - 10f, maxAngle + 10f);

        transform.rotation = Quaternion.Euler(0, 0, currentZRotation);
        ActualizarExpresion();

        if (Mathf.Abs(currentZRotation) > maxAngle)
        {
            LoseGame();
        }
    }

    // --- ESTRUCTURA SOLICITADA ---

    private void WinGame()
    {
        gameOver = true;
        
        // Cambio de sprite a victoria
        if (characterRenderer != null && spriteGanar != null) 
            characterRenderer.sprite = spriteGanar;

        // Reportar al GameManager global
        if (GameManager.Instance != null) 
            GameManager.Instance.ReportarVictoria();
            
        Debug.Log("¡Ganaste! Aguantaste el equilibrio.");
    }

    private void LoseGame()
    {
        gameOver = true;

        // Cambio de sprite a derrota
        if (characterRenderer != null && spritePerder != null) 
            characterRenderer.sprite = spritePerder;

        // Reportar al GameManager global
        if (GameManager.Instance != null) 
            GameManager.Instance.ReportarDerrota();

        Debug.Log("¡Perdiste! Caíste.");
    }

    // ----------------------------

    void ActualizarExpresion()
    {
        float inclinacion = Mathf.Abs(currentZRotation);
        if (inclinacion < 15f) characterRenderer.sprite = spriteNormal;
        else if (inclinacion >= 15f && inclinacion < 35f) characterRenderer.sprite = spritePreocupado;
        else characterRenderer.sprite = spriteSudando;
    }
}