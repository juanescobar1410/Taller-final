using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.iOS;
using UnityEngine.UI;

public class ControllerScene2 : MonoBehaviour
{
    [Header("UI en juego")]
    public TextMeshProUGUI textoScore;
    

    [Header("Panel Final")]
    public GameObject panelScoreFinal;
    public TextMeshProUGUI textoScoreFinal;
    public TextMeshProUGUI textoCaidas;
    public TextMeshProUGUI textoTiempoFinal; // NUEVO: Para mostrar el tiempo
    //public Button botonReiniciar;
    //public Button botonMenu;

    [Header("Configuración")]
   

    private bool panelMostrado = false;
    private float tiempoEscena2 = 0f;

    void Start()
    {
        Debug.Log("El tiempo de la escena 1 " + GameManager.Instance.GlobalTime.ToString());

        if (panelScoreFinal != null)
            panelScoreFinal.SetActive(false);

    }

    void Update()
    {

        if (!panelMostrado)
        {
            tiempoEscena2 += Time.deltaTime;
        }

        textoScore.text = GameManager.Instance.Score.ToString();
        textoCaidas.text = GameManager.Instance.FallsCount.ToString();




    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            MostrarPanelFinal();
        }
    }
    void MostrarPanelFinal()
    {
        panelMostrado = true;


        GameManager.Instance.AddTime(tiempoEscena2);

        if (panelScoreFinal != null)
        {
            panelScoreFinal.SetActive(true);


            if (textoScoreFinal != null)
                textoScoreFinal.text = "Score: " + GameManager.Instance.Score.ToString();

            if(textoCaidas != null)
                textoCaidas.text = "Caidas: " + GameManager.Instance.FallsCount.ToString();

            if (textoTiempoFinal != null)
            {
                float tiempoTotal = GameManager.Instance.GlobalTime;
                textoTiempoFinal.text = "Tiempo: " + FormatearTiempo(tiempoTotal);
            }


            Time.timeScale = 0f;
        }
    }


    string FormatearTiempo(float tiempo)
    {
        int minutos = Mathf.FloorToInt(tiempo / 60f);
        int segundos = Mathf.FloorToInt(tiempo % 60f);
        int milisegundos = Mathf.FloorToInt((tiempo * 100f) % 100f);


        return string.Format("{0:00}:{1:00}.{2:00}", minutos, segundos, milisegundos);


    }

    //void ReiniciarEscena()
    //{
    //    Time.timeScale = 1f;


    //    GameManager.Instance.GlobalTime = 0f;
    //    GameManager.Instance.Score = 0;
    //    GameManager.Instance.ItemsCount = 0;

    //    UnityEngine.SceneManagement.SceneManager.LoadScene(
    //        UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
    //    );
    //}

    //void VolverAlMenu()
    //{
    //    Time.timeScale = 1f;


    //    GameManager.Instance.GlobalTime = 0f;
    //    GameManager.Instance.Score = 0;
    //    GameManager.Instance.ItemsCount = 0;

    //    UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    //}
}