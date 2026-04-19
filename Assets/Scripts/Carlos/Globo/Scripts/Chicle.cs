using UnityEngine;
using System.Collections; 

public class InflaElChicleOptimiz : MonoBehaviour
{
    [Header("Configuracion")]
    public float threshold = 0.01f;
    public float timeLimit = 5.0f;
    public float tiempoGanar = 2.0f;
    public float velDesinflado = 1.0f;

    private float timer = 0f;
    private float progresoInflado = 0f;
    private bool gameOver = false;

    [Header("Imagenes")]
    public SpriteRenderer chicoRender;
    public Sprite chicoNeutral;
    public Sprite chicoSoplando;
    public Sprite chicoSorprendido;
    public Sprite chicoTriste;
    public GameObject explosion;

    public Transform chicleTransform;
    public Vector3 escalaIni = Vector3.zero;
    public Vector3 escalaMax = new Vector3(3f, 3f, 3f);


    void Start()
    {
        if (chicoRender != null) chicoRender.sprite = chicoNeutral;
        if (chicleTransform != null) chicleTransform.localScale = escalaIni;
        if (explosion != null) explosion.SetActive(false);
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
            progresoInflado += Time.deltaTime;
            if (chicoRender != null) chicoRender.sprite = chicoSoplando;

            if (progresoInflado >= tiempoGanar)
            {
                StartCoroutine(WinGame());
                return;
            }
        }
        else
        {
            progresoInflado -= Time.deltaTime * velDesinflado;
            progresoInflado = Mathf.Max(0f, progresoInflado);

            if (chicoRender != null) chicoRender.sprite = chicoNeutral;
        }

        if (chicleTransform != null)
        {
            float porcentaje = progresoInflado / tiempoGanar;
            chicleTransform.localScale = Vector3.Lerp(escalaIni, escalaMax, porcentaje);
        }
    }

    private IEnumerator WinGame()
    {
        gameOver = true;

        if (chicoRender != null) chicoRender.sprite = chicoSorprendido;
        if (chicleTransform != null) chicleTransform.gameObject.SetActive(false);
        if (explosion != null) explosion.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        if (explosion != null) explosion.SetActive(false);
        if (GameManager.Instance != null)
            GameManager.Instance.ReportarVictoria();
    }

    private void LoseGame()
    {
        gameOver = true;
        if (chicoRender != null) chicoRender.sprite = chicoTriste;
        if (GameManager.Instance != null)
            GameManager.Instance.ReportarDerrota();
    }
}