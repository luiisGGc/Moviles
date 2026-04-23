using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CleanGameFinalV2 : MonoBehaviour
{
    [Header("Configuración de Limpieza")]
    public int brushSize = 20;
    [Range(0.1f, 1f)] public float porcentajeParaGanar = 0.9f; // 90% limpio
    public float tiempoLimite = 15f;

    [Header("Referencias Visuales")]
    public GameObject cursorMano;
    public ParticleSystem particulas;

    private SpriteRenderer sRender;
    private Texture2D editableTex;
    private Color32[] pixels;
    private int width, height;
    
    private int pixelesTotalesTransparentes;
    private int pixelesLimpiadosActuales = 0;
    private bool gameOver = false;
    private float cronometro;

    void Start()
    {
        cronometro = tiempoLimite;
        sRender = GetComponent<SpriteRenderer>();
        if (sRender == null || sRender.sprite == null) return;

        ConfigurarTexturaEditable();

        // Contamos cuántos píxeles NO son transparentes al inicio para saber qué limpiar
        pixelesTotalesTransparentes = 0;
        foreach (var p in pixels) if (p.a > 0) pixelesTotalesTransparentes++;

        if (cursorMano != null) cursorMano.SetActive(false);
        if (particulas != null) particulas.Stop();
    }

    void Update()
    {
        if (gameOver) return;

        // Lógica de Tiempo
        cronometro -= Time.deltaTime;
        if (cronometro <= 0)
        {
            LoseGame();
        }

        ManejarEntradaTactil();
    }

    // --- ESTRUCTURA SOLICITADA ---

    private void WinGame()
    {
        gameOver = true;
        if (cursorMano != null) cursorMano.SetActive(false);
        if (particulas != null) particulas.Stop();

        
        if (GameManager.Instance != null) GameManager.Instance.ReportarVictoria();
        
        Debug.Log("¡Objeto Limpio! Victoria reportada.");
    }

    private void LoseGame()
    {
        gameOver = true;
        if (cursorMano != null) cursorMano.SetActive(false);
        if (particulas != null) particulas.Stop();


        if (GameManager.Instance != null) GameManager.Instance.ReportarDerrota();

        Debug.Log("¡Se acabó el tiempo! Suciedad acumulada.");
    }

    // ----------------------------

    void EraseAt(int cx, int cy)
    {
        bool changed = false;
        for (int y = cy - brushSize; y < cy + brushSize; y++)
        {
            for (int x = cx - brushSize; x < cx + brushSize; x++)
            {
                if (x >= 0 && x < width && y >= 0 && y < height)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), new Vector2(cx, cy));
                    if (dist < brushSize)
                    {
                        int index = y * width + x;
                        if (pixels[index].a != 0)
                        {
                            pixels[index] = new Color32(0, 0, 0, 0);
                            pixelesLimpiadosActuales++;
                            changed = true;
                        }
                    }
                }
            }
        }

        if (changed)
        {
            editableTex.SetPixels32(pixels);
            editableTex.Apply();
            
            // Verificar si ya se alcanzó el porcentaje de victoria
            if ((float)pixelesLimpiadosActuales / pixelesTotalesTransparentes >= porcentajeParaGanar)
            {
                WinGame();
            }
        }
    }

    // Métodos de apoyo para mantener el Update limpio
    void ManejarEntradaTactil()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, -Camera.main.transform.position.z));
            worldPos.z = 0;

            if (touch.phase == TouchPhase.Began)
            {
                cursorMano?.SetActive(true);
                particulas?.Play();
            }

            if (cursorMano != null) cursorMano.transform.position = worldPos;
            if (particulas != null) particulas.transform.position = worldPos;

            HandleCleaning(touch.position);

            if (touch.phase == TouchPhase.Ended)
            {
                cursorMano?.SetActive(false);
                particulas?.Stop();
            }
        }
    }

    void ConfigurarTexturaEditable()
    {
        Texture2D sourceTex = sRender.sprite.texture;
        Rect r = sRender.sprite.rect;
        width = (int)r.width; height = (int)r.height;
        editableTex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        Color32[] croppedPixels = sourceTex.GetPixels32();
        Color32[] finalPixels = new Color32[width * height];
        int startX = (int)r.x; int startY = (int)r.y;
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                finalPixels[y * width + x] = croppedPixels[(startY + y) * sourceTex.width + (startX + x)];
        pixels = finalPixels;
        editableTex.SetPixels32(pixels);
        editableTex.Apply();
        sRender.sprite = Sprite.Create(editableTex, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));
    }

    void HandleCleaning(Vector2 screenPosition)
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, -Camera.main.transform.position.z));
        Vector2 localPos = transform.InverseTransformPoint(worldPos);
        float unitWidth = sRender.sprite.rect.width / sRender.sprite.pixelsPerUnit;
        float unitHeight = sRender.sprite.rect.height / sRender.sprite.pixelsPerUnit;
        float xPerc = (localPos.x / unitWidth) + 0.5f;
        float yPerc = (localPos.y / unitHeight) + 0.5f;
        if (xPerc >= 0 && xPerc <= 1 && yPerc >= 0 && yPerc <= 1)
        {
            EraseAt((int)(xPerc * width), (int)(yPerc * height));
        }
    }
}