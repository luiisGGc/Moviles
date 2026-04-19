using UnityEngine;
using UnityEngine.Events;

public class MantenElVuelo : MonoBehaviour
{
    [Header("Config")]
    public float threshold = 0.005f;
    public float timeLimit = 4.0f;

    private float timer = 0f;
    private bool gameOver = false;

    [Header("Fisicas")]
    public Transform objetoVolador;
    public float gravedad = 8f;
    public float fuerzaSoplido = 15f;
    public float velocidadMaximaY = 5f;
    private float velocidadActualY = 0f;

    void Update()
    {
        if (gameOver) return;

        timer += Time.deltaTime;
        if (timer >= timeLimit)
        {
            WinGame();
            return;
        }

        float currentVolume = InputMicGlobal.Instance.VolumenActual;

        if (currentVolume > threshold)
        {
            velocidadActualY += fuerzaSoplido * Time.deltaTime;
        }

        velocidadActualY -= gravedad * Time.deltaTime;
        velocidadActualY = Mathf.Clamp(velocidadActualY, -velocidadMaximaY, velocidadMaximaY);

        if (objetoVolador != null)
        {
            objetoVolador.Translate(Vector3.up * velocidadActualY * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!gameOver) LoseGame();
    }

    private void WinGame()
    {
        gameOver = true;
        if (GameManager.Instance != null) GameManager.Instance.ReportarVictoria();
    }

    private void LoseGame()
    {
        gameOver = true;
        if (GameManager.Instance != null) GameManager.Instance.ReportarDerrota();
    }
}