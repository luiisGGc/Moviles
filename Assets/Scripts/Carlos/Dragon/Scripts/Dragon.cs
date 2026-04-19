using UnityEngine;
using UnityEngine.Events;

public class EnciendeElDragon : MonoBehaviour
{
    [Header("Configuración")]
    public float thresholdBajo = 0.001f;
    public float thresholdAlto = 0.008f;
    public float timeLimit = 4.0f;
    public float tiempoGanar = 0.2f;

    private float timer = 0f;
    private float tiempoFuerteAct = 0f;
    private bool gameOver = false;

    [Header("RSprites")]
    public SpriteRenderer dragonRenderer;
    public Sprite dragonNormal;
    public Sprite dragonSoplando;
    public Sprite dragonFeliz;
    public Sprite dragonTriste;
    public GameObject fogata;
    public GameObject fogataQuemada;
    public GameObject malvaviscoNormal;
    public GameObject malvaviscoQuemado;
    public GameObject chispitas;
    public GameObject fuegoPro;

    [Header("Eventos")]
    public UnityEvent OnGameWon;
    public UnityEvent OnGameLost;

    void Start()
    {
        if (dragonRenderer != null) dragonRenderer.sprite = dragonNormal;
        if (fogata != null) fogata.SetActive(true);
        if (fogataQuemada != null) fogataQuemada.SetActive(false);
        if (malvaviscoNormal != null) malvaviscoNormal.SetActive(true);
        if (malvaviscoQuemado != null) malvaviscoQuemado.SetActive(false);
        if (chispitas != null) chispitas.SetActive(false);
        if (fuegoPro != null) fuegoPro.SetActive(false);
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
        if (currentVolume >= thresholdAlto)
        {
            if (dragonRenderer != null) dragonRenderer.sprite = dragonSoplando;
            if (chispitas != null) chispitas.SetActive(false);
            if (fuegoPro != null) fuegoPro.SetActive(true);

            tiempoFuerteAct += Time.deltaTime;

            if (tiempoFuerteAct >= tiempoGanar) WinGame();
        }
        else if (currentVolume >= thresholdBajo)
        {
            if (dragonRenderer != null) dragonRenderer.sprite = dragonSoplando;
            if (chispitas != null) chispitas.SetActive(true);
            if (fuegoPro != null) fuegoPro.SetActive(false);

            tiempoFuerteAct = 0f;
        }
        else
        {
            if (dragonRenderer != null) dragonRenderer.sprite = dragonNormal;
            if (chispitas != null) chispitas.SetActive(false);
            if (fuegoPro != null) fuegoPro.SetActive(false);

            tiempoFuerteAct = 0f;
        }
    }

    private void WinGame()
    {
        gameOver = true;

        if (dragonRenderer != null) dragonRenderer.sprite = dragonFeliz;
        if (chispitas != null) chispitas.SetActive(false);
        if (fuegoPro != null) fuegoPro.SetActive(true);
        if (fogata != null) fogata.SetActive(false);
        if (fogataQuemada != null) fogataQuemada.SetActive(true);
        if (malvaviscoNormal != null) malvaviscoNormal.SetActive(false);
        if (malvaviscoQuemado != null) malvaviscoQuemado.SetActive(true);

        OnGameWon.Invoke();
        if (GameManager.Instance != null) GameManager.Instance.ReportarVictoria();
    }

    private void LoseGame()
    {
        gameOver = true;

        if (dragonRenderer != null) dragonRenderer.sprite = dragonTriste;
        if (chispitas != null) chispitas.SetActive(false);
        if (fuegoPro != null) fuegoPro.SetActive(false);

        OnGameLost.Invoke();
        if (GameManager.Instance != null) GameManager.Instance.ReportarDerrota();
    }
}