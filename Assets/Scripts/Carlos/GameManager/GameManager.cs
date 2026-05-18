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
    public int puntuacionActual = 0;
    public int puntuacionMaxima;
    public List<Niveles> nivelesDisponibles;

    [Header("UI General")]
    public GameObject pantallaTransicion;
    public TextMeshProUGUI textoInstruccion;
    public Image iconoAccion;
    public Sprite iconoSoplar, iconoTocar, iconoRotar;

    public TextMeshProUGUI textoVidas;
    public GameObject menuPausa;
    public GameObject panelGameOver;

    private bool juegoPausado = false;
    private string escenaActual = "";

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        puntuacionMaxima = PlayerPrefs.GetInt("PuntuacionMaxima", 0);
        ActualizarUI();

        juegoPausado = false;
        if (menuPausa != null) menuPausa.SetActive(false);
        if (panelGameOver != null) panelGameOver.SetActive(false);
        Time.timeScale = 1;

        SiguienteNivel();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) TogglePausa();
    }

    void OnApplicationPause(bool isPaused)
    {
        if (isPaused && !juegoPausado)
        {
            TogglePausa();
        }
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus && !juegoPausado)
        {
            TogglePausa();
        }
    }

    public void SiguienteNivel()
    {
        if (vidas <= 0)
        {
            Debug.Log("GAME OVER");
            BorrarProgreso();
            if (panelGameOver != null)
            {
                panelGameOver.SetActive(true);
            }
            Time.timeScale = 0f; 
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
        puntuacionActual += 1;
        GuardarProgreso();
        Invoke("SiguienteNivel", 1.5f);
    }

    public void ReportarDerrota()
    {
        Debug.Log("Minijuego Perdido!");
        vidas--;
        ActualizarUI();
        GuardarProgreso();
        Invoke("SiguienteNivel", 1.5f);
    }

    private void ActualizarUI()
    {
        if (textoVidas != null) textoVidas.text = "Vidas: " + vidas;
    }

    public void TogglePausa()
    {
        juegoPausado = !juegoPausado;
        if (menuPausa != null) menuPausa.SetActive(juegoPausado);
        Time.timeScale = juegoPausado ? 0 : 1;
    }

    public void GuardarProgreso()
    {
        PlayerPrefs.SetInt("Vidas", vidas);
        PlayerPrefs.SetInt("PuntuacionActual", puntuacionActual);
        PlayerPrefs.SetInt("HayPartidaGuardada", 1);

        if (puntuacionActual > puntuacionMaxima)
        {
            puntuacionMaxima = puntuacionActual;
            PlayerPrefs.SetInt("PuntuacionMaxima", puntuacionMaxima);
        }
        PlayerPrefs.Save();
        Debug.Log("Partida Guardada Exitosamente");
    }

    public void CargarProgreso()
    {
        if (PlayerPrefs.GetInt("HayPartidaGuardada", 0) == 1)
        {
            vidas = PlayerPrefs.GetInt("Vidas", 4);
            puntuacionActual = PlayerPrefs.GetInt("PuntuacionActual", 0);
            Debug.Log("Partida Cargada. Vidas: " + vidas + " | Puntos: " + puntuacionActual);
        }
        else
        {
            Debug.Log("No hay partida guardada previa.");
        }
    }

    public void BorrarProgreso()
    {
        PlayerPrefs.SetInt("HayPartidaGuardada", 0);
        vidas = 4;
        puntuacionActual = 0;
        PlayerPrefs.Save();
        Debug.Log("Progreso temporal borrado (Game Over).");
    }

    public void ReiniciarPartida()
    {
        Time.timeScale = 1f;
        if (GameManager.Instance != null)
        {
            GameManager.Instance.BorrarProgreso();
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SalirAlMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }


}