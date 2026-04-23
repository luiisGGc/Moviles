using UnityEngine;
using UnityEngine.UI;

public class LogicaFugaGas : MonoBehaviour
{
    public Image barraProgreso;
    public float velocidadReparacion = 0.25f; 
    public float velocidadFuga = 0.15f;      

    public Image imagenTuberia;
    public Sprite tuberiaRota;
    public Sprite tuberiaArreglada;

    public GameObject cartelVictoria;
    private float progreso = 0f;
    private bool completado = false;

    void Update()
    {
        if (completado) return;

        if (Input.GetMouseButton(0))
        {
            progreso += velocidadReparacion * Time.deltaTime;
        }
        else 
        {
            if (progreso > 0) progreso -= velocidadFuga * Time.deltaTime;
        }

        progreso = Mathf.Clamp(progreso, 0f, 1f);
        barraProgreso.fillAmount = progreso;

        if (progreso >= 1f)
        {
            Victoria();
        }
    }

    void Victoria()
    {
        completado = true;
        imagenTuberia.sprite = tuberiaArreglada;
        cartelVictoria.SetActive(true);
    }
}