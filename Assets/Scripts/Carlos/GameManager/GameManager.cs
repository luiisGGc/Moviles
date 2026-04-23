using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public enum TipoAccion { Soplar, Tocar, Rotar }
[System.Serializable]
public class Niveles
{
    public string nombreEscena;
    public string instruccionBreve;
    public TipoAccion accionRequerida;
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Configuración del Juego")]
    public int vidas = 4;
    public List<Niveles> nivelesDisponibles;

    [Header("UI General")]
    public GameObject pantallaTransicion;
    public TextMeshProUGUI textoInstruccion;
    public Image iconoAccion;
    public Sprite iconoSoplar, iconoTocar, iconoRotar;

    public TextMeshProUGUI textoVidas;
    public GameObject menuPausa;

    private bool juegoPausado = false;
    private string escenaActual = "";

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
    }

    void Start()
    {
        ActualizarUI();
        SiguienteNivel();
        menuPausa.SetActive(juegoPausado);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) TogglePausa();
    }

    public void SiguienteNivel()
    {
        if (vidas <= 0)
        {
            Debug.Log("GAME OVER");
            return;
        }

        StartCoroutine(RutinaTransicion());
    }

    private IEnumerator RutinaTransicion()
    {
        if (!string.IsNullOrEmpty(escenaActual))
        {
            SceneManager.UnloadSceneAsync(escenaActual);
        }

        Niveles nivelElegido = nivelesDisponibles[Random.Range(0, nivelesDisponibles.Count)];
        escenaActual = nivelElegido.nombreEscena;
        pantallaTransicion.SetActive(true);
        textoInstruccion.text = nivelElegido.instruccionBreve;

        switch (nivelElegido.accionRequerida)
        {
            case TipoAccion.Soplar: iconoAccion.sprite = iconoSoplar; break;
            case TipoAccion.Tocar: iconoAccion.sprite = iconoTocar; break;
            case TipoAccion.Rotar: iconoAccion.sprite = iconoRotar; break;
        }

        yield return new WaitForSeconds(2.0f);
        pantallaTransicion.SetActive(false);
        SceneManager.LoadScene(escenaActual, LoadSceneMode.Additive);
    }

    public void ReportarVictoria()
    {
        Debug.Log("Minijuego Ganado!");
        Invoke("SiguienteNivel", 1.5f); 
    }

    public void ReportarDerrota()
    {
        Debug.Log("Minijuego Perdido!");
        vidas--;
        ActualizarUI();
        Invoke("SiguienteNivel", 1.5f);
    }

    private void ActualizarUI()
    {
        textoVidas.text = "Vidas: " + vidas;
    }

    public void TogglePausa()
    {
        juegoPausado = !juegoPausado;
        menuPausa.SetActive(juegoPausado);
        Time.timeScale = juegoPausado ? 0 : 1; 
    }
}