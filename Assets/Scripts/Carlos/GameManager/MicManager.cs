using UnityEngine;
#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif

public class InputMicGlobal : MonoBehaviour
{
    //esto da acceso global al microfono para que se pueda usar en cualquier minijuego y se active una vez algo asi tienen que hacer con el swipe y la rotacion
    public static InputMicGlobal Instance; 

    public int sampleWindow = 128;
    private AudioClip microphoneClip;
    private string micName;
    public float VolumenActual { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); //para que no se destruya
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
#if PLATFORM_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
            Permission.RequestUserPermission(Permission.Microphone);
#endif
        if (Microphone.devices.Length > 0)
        {
            micName = Microphone.devices[0];
            microphoneClip = Microphone.Start(micName, true, 1, 44100);
        }
    }

    void Update()
    {
        VolumenActual = CalcularRMS();

        //para probar en PC en cualquier minijuego
#if UNITY_EDITOR
        if (Input.GetKey(KeyCode.Space)) VolumenActual = 1.0f;
#endif
    }

    private float CalcularRMS()
    {
        if (Microphone.GetPosition(micName) <= 0) return 0;
        float[] waveData = new float[sampleWindow];
        int micPosition = Microphone.GetPosition(micName) - (sampleWindow + 1);
        if (micPosition < 0) return 0;
        microphoneClip.GetData(waveData, micPosition);
        float sum = 0;
        for (int i = 0; i < sampleWindow; i++) sum += waveData[i] * waveData[i];
        return Mathf.Sqrt(sum / sampleWindow);
    }
}