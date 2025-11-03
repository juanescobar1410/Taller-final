using UnityEngine;

public class PlataformaLerpSuave : MonoBehaviour
{
    [Header("Configuración de movimiento")]
    public float altura = 3f;          // Distancia que sube o baja
    public float velocidad = 1f;       // Velocidad del movimiento

    private Vector3 posicionInicial;
    private Vector3 posicionFinal;

    void Start()
    {
        posicionInicial = transform.position;
        posicionFinal = new Vector3(posicionInicial.x, posicionInicial.y + altura, posicionInicial.z);
    }

    void Update()
    {
        // Valor oscilante entre 0 y 1 (movimiento cíclico)
        float t = Mathf.PingPong(Time.time * velocidad, 1f);

        // Aplicar una curva senoidal para suavizar los extremos
        float tSuavizado = Mathf.SmoothStep(0f, 1f, Mathf.Sin(t * Mathf.PI * 0.5f));

        // Movimiento interpolado suavemente
        transform.position = Vector3.Lerp(posicionInicial, posicionFinal, tSuavizado);
    }
}