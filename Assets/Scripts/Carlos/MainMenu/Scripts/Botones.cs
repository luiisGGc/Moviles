using UnityEngine;
using UnityEngine.SceneManagement; 

public class SceneLoader : MonoBehaviour
{
   
    public void CambiarEscenaNom(string nombreEscena)
    {
        SceneManager.LoadScene(nombreEscena);
    }

    public void CambiarEscenaInd(int indiceEscena)
    {
        SceneManager.LoadScene(indiceEscena);
    }

    public void Salir()
    {
        Debug.Log("El juego se ha cerrado"); 
        Application.Quit(); 
    }
}