using UnityEngine;

public class CleanGameFinalV2 : MonoBehaviour
{
    [Header("Configuración de Limpieza")]
    public int brushSize = 20;
    
    [Header("Referencias Visuales")]
    public GameObject cursorMano;       // Arrastra aquí el sprite de la mano
    public ParticleSystem particulas;    // Arrastra aquí tu sistema de partículas

    private SpriteRenderer sRender;
    private Texture2D editableTex;
    private Color32[] pixels;
    private int width, height;

   void Start()
{
    sRender = GetComponent<SpriteRenderer>();
    if (sRender == null || sRender.sprite == null) return;

    // 1. Obtener la textura original y el rectángulo del sprite recortado
    Texture2D sourceTex = sRender.sprite.texture;
    Rect r = sRender.sprite.rect;

    width = (int)r.width;
    height = (int)r.height;

    // 2. Crear la textura editable solo con el tamaño del recorte
    editableTex = new Texture2D(width, height, TextureFormat.RGBA32, false);
    
    // 3. Extraer solo los píxeles del área recortada
    pixels = sourceTex.GetPixels32(); // Nota: GetPixels32 obtiene todo, pero filtraremos
    Color32[] croppedPixels = sourceTex.GetPixels32();
    
    // Obtenemos el bloque exacto de píxeles del sprite
    Color32[] finalPixels = new Color32[width * height];
    
    int startX = (int)r.x;
    int startY = (int)r.y;

    for (int y = 0; y < height; y++)
    {
        for (int x = 0; x < width; x++)
        {
            // Mapeamos del global al local
            finalPixels[y * width + x] = croppedPixels[(startY + y) * sourceTex.width + (startX + x)];
        }
    }

    // 4. Asignar los píxeles filtrados a nuestra textura de trabajo
    pixels = finalPixels;
    editableTex.SetPixels32(pixels);
    editableTex.Apply();

    // 5. Reemplazar el sprite visual por nuestra versión editable
    sRender.sprite = Sprite.Create(editableTex, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));

    // Inicializar mano y partículas
    if (cursorMano != null) cursorMano.SetActive(false);
    if (particulas != null) particulas.Stop();
}
   void Update()
{
    if (Input.touchCount > 0) 
    {
        Touch touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Began)
        {
            // El signo "?" verifica si existe antes de actuar
            cursorMano?.SetActive(true);
            particulas?.Play();
        }

        if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
        {
            if (Camera.main == null) return;
            
            Vector3 touchWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, -Camera.main.transform.position.z));
            touchWorldPos.z = 0;

            // Movemos solo si las referencias no son nulas
            if (cursorMano != null) cursorMano.transform.position = touchWorldPos;
            if (particulas != null) particulas.transform.position = touchWorldPos;

            HandleCleaning(touch.position);
        }

        if (touch.phase == TouchPhase.Ended)
        {
            cursorMano?.SetActive(false);
            particulas?.Stop();
        }
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
                        }
                    }
                }
            }
        }

        if (changed)
        {
            editableTex.SetPixels32(pixels);
            editableTex.Apply();
        }
    }
}