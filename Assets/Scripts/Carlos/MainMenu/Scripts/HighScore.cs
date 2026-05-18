using UnityEngine;
using TMPro; 

public class MenuPrincipal : MonoBehaviour
{
   
    public TextMeshProUGUI textoRecord;

    void Start()
    {
        int recordGuardado = PlayerPrefs.GetInt("PuntuacionMaxima", 0);

        if (textoRecord != null)
        {
            textoRecord.text = "Puntuacion Maxima: " + recordGuardado;
        }
    }
}