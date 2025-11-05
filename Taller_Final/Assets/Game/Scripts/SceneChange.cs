using UnityEngine;
using UnityEngine.SceneManagement;

public class CambioDeEscena : MonoBehaviour
{
    [SerializeField] private string Scene2 = "Escena2"; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene(Scene2);
        }
    }


    public void LoaderScenes(string nameScene)
    {
        SceneManager.LoadScene(nameScene);
    }



}