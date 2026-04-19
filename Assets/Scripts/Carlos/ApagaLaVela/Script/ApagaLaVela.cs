using UnityEngine;
using UnityEngine.Events;

public class ApagaLaVela : MonoBehaviour
{
    [Header("Config")]
    public float threshold = 0.005f;
    public float timeLimit = 4.0f;
    public float tiempoAviso = 2.0f;
    public float tiempoReq = 0.5f;

    private float timer = 0f;
    private float tiempoSoplandoAct = 0f;
    private bool gameOver = false;
    private bool momentoSoplar = false;

    [Header("Abuela")]
    public SpriteRenderer abuelaRenderer;
    public Sprite abuelaNormal;
    public Sprite abuelaSoplando;
    public Sprite abuelaGanar;
    public Sprite abuelaPerder;

    [Header("Lo demas")]
    public SpriteRenderer senoraRenderer;
    public Sprite senoraNeutral;
    public Sprite senoraGritando;
    public GameObject signoAdmiracion;
    public GameObject llamaVela;

    void Start()
    {
        if (signoAdmiracion != null) signoAdmiracion.SetActive(false);
        if (senoraRenderer != null) senoraRenderer.sprite = senoraNeutral;
        if (abuelaRenderer != null) abuelaRenderer.sprite = abuelaNormal;
    }

    void Update()
    {
        if (gameOver) return;

        timer += Time.deltaTime;
        if (timer >= tiempoAviso && !momentoSoplar)
        {
            momentoSoplar = true;
            if (senoraRenderer != null) senoraRenderer.sprite = senoraGritando;
            if (signoAdmiracion != null) signoAdmiracion.SetActive(true);
        }

        if (timer >= timeLimit)
        {
            LoseGame();
            return;
        }

        float currentVolume = InputMicGlobal.Instance.VolumenActual;

        if (currentVolume > threshold)
        {
            if (abuelaRenderer != null) abuelaRenderer.sprite = abuelaSoplando;

            if (momentoSoplar)
            {
                tiempoSoplandoAct += Time.deltaTime;
                if (tiempoSoplandoAct >= tiempoReq) WinGame();
            }
        }
        else
        {
            if (abuelaRenderer != null) abuelaRenderer.sprite = abuelaNormal;
            tiempoSoplandoAct = 0f; 
        }
    }

    private void WinGame()
    {
        gameOver = true;
        if (abuelaRenderer != null) abuelaRenderer.sprite = abuelaGanar;
        if (llamaVela != null) llamaVela.SetActive(false);
        if (GameManager.Instance != null) GameManager.Instance.ReportarVictoria();
    }

    private void LoseGame()
    {
        gameOver = true;
        if (abuelaRenderer != null) abuelaRenderer.sprite = abuelaPerder;
        if (senoraRenderer != null) senoraRenderer.sprite = senoraNeutral;
        if (signoAdmiracion != null) signoAdmiracion.SetActive(false);
        if (GameManager.Instance != null) GameManager.Instance.ReportarDerrota();
    }
}