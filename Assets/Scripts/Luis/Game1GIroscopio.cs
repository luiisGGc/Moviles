using UnityEngine;
using TMPro; // Necesario para usar TextMeshPro

public sealed class GyroBalance : MonoBehaviour
{
    [Header("Configuración de Equilibrio")]
    [SerializeField] private float sensitivity = 50.0f; 
    [SerializeField] private float gravityForce = 60f; // Aumentado para que caiga más rápido
    [SerializeField] private float maxAngle = 45f;

    [Header("Referencias UI")]
    [SerializeField] private GameObject loseText; // Arrastra aquí tu letrero de "Perdiste"

    [Header("Sprites de Estado")]
    [SerializeField] private SpriteRenderer characterRenderer; 
    [SerializeField] private Sprite spriteNormal;   // El 1.1 de la imagen
    [SerializeField] private Sprite spritePreocupado; // El 1.2
    [SerializeField] private Sprite spriteSudando;   // El 1.3
    private float currentZRotation = 0f;
    private bool isGameOver = false;

void Start()
    {
        Input.gyro.enabled = true;
        if (characterRenderer == null) characterRenderer = GetComponent<SpriteRenderer>();
    }
    void Update()
    {
        // Si ya perdió, no ejecutamos el movimiento
        if (isGameOver) return;

        float inputX = 0f;

        if (Input.gyro.enabled)
        {
            inputX = Input.gyro.rotationRateUnbiased.z;
        }
        else
        {
            inputX = Input.GetAxis("Horizontal");
        }

        // 1. Aplicamos el giro del usuario
        currentZRotation += inputX * sensitivity * Time.deltaTime;

        // 2. Efecto de gravedad (caída automática)
        // Multiplicamos por un factor extra si quieres que sea aún más rápido al alejarse del centro
        float gravityEffect = (currentZRotation / maxAngle) * gravityForce;
        currentZRotation += gravityEffect * Time.deltaTime;

        // 3. Limitar ángulo para el cálculo visual
        currentZRotation = Mathf.Clamp(currentZRotation, -maxAngle - 10f, maxAngle + 10f);

        // 4. APLICAR AL TRANSFORM
        transform.rotation = Quaternion.Euler(0, 0, currentZRotation);
        ActualizarExpresion();
        // 5. CONDICIÓN DE DERROTA
        if (Mathf.Abs(currentZRotation) > maxAngle)
        {
            GameOver();
        }
    }

    void GameOver()
    {
        isGameOver = true;
        if (loseText != null)
        {
            loseText.SetActive(true); // Muestra el mensaje en pantalla
        }
        Debug.Log("¡Perdiste por superar los 45 grados!");
    }

    public void ResetGame()
    {
        currentZRotation = 0;
        isGameOver = false;
        if (loseText != null)
            loseText.SetActive(false);
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
        else // Más de 35 grados
        {
            characterRenderer.sprite = spriteSudando;
        }
    }
    void OnGUI()
    {
        GUILayout.Label("¿Giroscopio habilitado?: " + Input.gyro.enabled);
        GUILayout.Label("Rotación Actual: " + currentZRotation);
        if (isGameOver) GUILayout.Label("ESTADO: PERDISTE");
    }
}