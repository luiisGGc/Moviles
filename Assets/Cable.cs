using UnityEngine;
using UnityEngine.UI;

public class CableLoco : MonoBehaviour
{
    [Header("Configuracion de Movimiento")]
    public float tiempoEntreBrincos = 1.2f;
    public float limiteX = 400f;
    public float limiteY = 250f;

    [Header("Referencias de Victoria")]
    public RectTransform objetivoCargador;
    public Image imagenCargador;
    public Sprite spriteConectado;
    public GameObject cartelVictoria;

    private float cronometro;
    private bool agarrado = false;
    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        if (cartelVictoria != null) cartelVictoria.SetActive(false);
    }

    void Update()
    {
        if (!agarrado)
        {
            cronometro += Time.deltaTime;
            if (cronometro >= tiempoEntreBrincos)
            {
                Saltar();
                cronometro = 0;
            }
        }
        else
        {
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform.parent as RectTransform,
                Input.mousePosition,
                null,
                out localPoint);

            rectTransform.localPosition = localPoint;

            float distancia = Vector2.Distance(rectTransform.localPosition, objetivoCargador.localPosition);
            if (distancia < 80f) Victoria();
        }
    }

    void Saltar()
    {
        float randomX = Random.Range(-limiteX, limiteX);
        float randomY = Random.Range(-limiteY, limiteY);
        rectTransform.localPosition = new Vector3(randomX, randomY, 0);
    }

    public void OnMouseDown() { agarrado = true; }
    public void OnMouseUp() { agarrado = false; }

    void Victoria()
    {
        agarrado = false;
        rectTransform.localPosition = objetivoCargador.localPosition;
        if (imagenCargador != null && spriteConectado != null)
        {
            imagenCargador.sprite = spriteConectado;
            gameObject.SetActive(false);
        }
        if (cartelVictoria != null) cartelVictoria.SetActive(true);
    }
}
