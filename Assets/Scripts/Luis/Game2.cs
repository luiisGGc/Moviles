using UnityEngine;

public class CleanGameFinalV2 : MonoBehaviour
{
    [Header("Configuración de Limpieza")]
    public int brushSize = 20;
    public float tiempoLimite = 5.0f; 
    [Range(0.1f, 1f)]
    public float porcentajeParaGanar = 0.85f; 

    [Header("Referencias Visuales")]
    public GameObject cursorMano;
    public ParticleSystem particulas;
    public GameObject winText; 
    public GameObject loseText; 

    private SpriteRenderer sRender;
    private Texture2D editableTex;
    private Color32[] pixels;
    private int width, height;

    private float cronometro;
    private bool juegoActivo = true;
    private int totalPixelesMugre = 0;
    private int pixelesLimpiados = 0;

    void Start()
    {
        cronometro = tiempoLimite;
        if (winText != null) winText.SetActive(false);
        if (loseText != null) loseText.SetActive(false);

        sRender = GetComponent<SpriteRenderer>();
        if (sRender == null || sRender.sprite == null) return;

        Texture2D sourceTex = sRender.sprite.texture;
        Rect r = sRender.sprite.rect;

        width = (int)r.width;
        height = (int)r.height;

        editableTex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        Color32[] croppedPixels = sourceTex.GetPixels32();
        Color32[] finalPixels = new Color32[width * height];

        int startX = (int)r.x;
        int startY = (int)r.y;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Color32 pixelActual = croppedPixels[(startY + y) * sourceTex.width + (startX + x)];
                finalPixels[y * width + x] = pixelActual;

                if (pixelActual.a > 10)
                {
                    totalPixelesMugre++;
                }
            }
        }

        pixels = finalPixels;
        editableTex.SetPixels32(pixels);
        editableTex.Apply();
        sRender.sprite = Sprite.Create(editableTex, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));

        if (cursorMano != null) cursorMano.SetActive(false);
        if (particulas != null) particulas.Stop();
    }

    void Update()
    {
        if (!juegoActivo) return;
        cronometro -= Time.deltaTime;
        if (cronometro <= 0)
        {
            FinalizarJuego(false);
            return;
        }

        bool interactuando = false;
        bool inicioInteraccion = false;
        bool finInteraccion = false;
        Vector3 inputPosition = Vector3.zero;

#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0)) { inicioInteraccion = true; interactuando = true; }
        else if (Input.GetMouseButton(0)) { interactuando = true; }
        else if (Input.GetMouseButtonUp(0)) { finInteraccion = true; }
        inputPosition = Input.mousePosition;
#else
        // Soporte para Táctil en Celular
        if (Input.touchCount > 0) 
        {
            Touch touch = Input.GetTouch(0);
            inputPosition = touch.position;
            if (touch.phase == TouchPhase.Began) { inicioInteraccion = true; interactuando = true; }
            else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) { interactuando = true; }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) { finInteraccion = true; }
        }
#endif

        if (inicioInteraccion)
        {
            if (cursorMano != null) cursorMano.SetActive(true);
            if (particulas != null) particulas.Play();
        }

        if (interactuando)
        {
            if (Camera.main == null) return;

            Vector3 touchWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(inputPosition.x, inputPosition.y, -Camera.main.transform.position.z));
            touchWorldPos.z = 0;

            if (cursorMano != null) cursorMano.transform.position = touchWorldPos;
            if (particulas != null) particulas.transform.position = touchWorldPos;

            HandleCleaning(inputPosition);
        }

        if (finInteraccion)
        {
            if (cursorMano != null) cursorMano.SetActive(false);
            if (particulas != null) particulas.Stop();
        }
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
            int px = (int)(xPerc * width);
            int py = (int)(yPerc * height);
            EraseAt(px, py);
        }
    }

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
                            changed = true;
                            pixelesLimpiados++;
                        }
                    }
                }
            }
        }

        if (changed)
        {
            editableTex.SetPixels32(pixels);
            editableTex.Apply();
            float progreso = (float)pixelesLimpiados / totalPixelesMugre;
            if (progreso >= porcentajeParaGanar)
            {
                FinalizarJuego(true);
            }
        }
    }

    void FinalizarJuego(bool victoria)
    {
        juegoActivo = false;

        if (cursorMano != null) cursorMano.SetActive(false);
        if (particulas != null) particulas.Stop();

        if (victoria)
        {
            if (winText != null) winText.SetActive(true);
            if (GameManager.Instance != null) GameManager.Instance.ReportarVictoria();
        }
        else
        {
            if (loseText != null) loseText.SetActive(true);
            if (GameManager.Instance != null) GameManager.Instance.ReportarDerrota();
        }
    }
}