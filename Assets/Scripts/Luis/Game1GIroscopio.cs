using UnityEngine;
using TMPro;

public sealed class GyroBalance : MonoBehaviour
{
    [Header("Configuración de Equilibrio")]
    [SerializeField] private float sensitivity = 50.0f;
    [SerializeField] private float gravityForce = 60f;
    [SerializeField] private float maxAngle = 45f;
    [SerializeField] private float timeLimit = 5.0f; 

    [Header("Referencias UI")]
    [SerializeField] private GameObject loseText;
    [SerializeField] private GameObject winText; 

    [Header("Sprites de Estado")]
    [SerializeField] private SpriteRenderer characterRenderer;
    [SerializeField] private Sprite spriteNormal;
    [SerializeField] private Sprite spritePreocupado;
    [SerializeField] private Sprite spriteSudando;

    private float currentZRotation = 0f;
    private bool isGameOver = false;
    private float cronometro = 0f;

    void Start()
    {
        Input.gyro.enabled = true;
        if (characterRenderer == null) characterRenderer = GetComponent<SpriteRenderer>();

        if (loseText != null) loseText.SetActive(false);
        if (winText != null) winText.SetActive(false);
    }

    void Update()
    {
        // Si ya perdió o ganó, detenemos el movimiento
        if (isGameOver) return;
        cronometro += Time.deltaTime;
        if (cronometro >= timeLimit)
        {
            WinGame();
            return;
        }
        float inputX = 0f;

        if (Input.gyro.enabled)
        {
            inputX = Input.gyro.rotationRateUnbiased.z;
        }
        else
        {
            inputX = Input.GetAxis("Horizontal");
        }

        currentZRotation += inputX * sensitivity * Time.deltaTime;
        float gravityEffect = (currentZRotation / maxAngle) * gravityForce;
        currentZRotation += gravityEffect * Time.deltaTime;
        currentZRotation = Mathf.Clamp(currentZRotation, -maxAngle - 10f, maxAngle + 10f);

        transform.rotation = Quaternion.Euler(0, 0, currentZRotation);

        ActualizarExpresion();

        if (Mathf.Abs(currentZRotation) > maxAngle)
        {
            GameOver();
        }
    }

    void WinGame()
    {
        isGameOver = true;
        if (winText != null) winText.SetActive(true);
        Debug.Log("¡Ganaste por mantener el equilibrio!");
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ReportarVictoria();
        }
    }

    void GameOver()
    {
        isGameOver = true;
        if (loseText != null) loseText.SetActive(true);
        Debug.Log("¡Perdiste por superar los 45 grados!");
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ReportarDerrota();
        }
    }

    void ActualizarExpresion()
    {
        float inclinacion = Mathf.Abs(currentZRotation);

        if (inclinacion < 15f)
        {
            characterRenderer.sprite = spriteNormal;
        }
        else if (inclinacion >= 15f && inclinacion < 35f)
        {
            characterRenderer.sprite = spritePreocupado;
        }
        else
        {
            characterRenderer.sprite = spriteSudando;
        }
    }
}