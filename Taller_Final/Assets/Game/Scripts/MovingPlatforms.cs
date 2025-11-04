using System.Collections;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    [SerializeField] private float distanciaMovimiento = 5f; // Distancia que se moverá la plataforma
    [SerializeField] private float velocidad = 2f; // Velocidad del movimiento
    [SerializeField] private float tiempoEspera = 3f; // Tiempo de espera en cada extremo

    private Vector3 posicionInicial;
    private Vector3 posicionObjetivo;
    private bool moviendoADerecha = true;
    private bool enMovimiento = false;

    void Start()
    {
        // Guardar la posición inicial
        posicionInicial = transform.position;

        // Calcular la posición objetivo (a la derecha)
        posicionObjetivo = posicionInicial + new Vector3(distanciaMovimiento, 0, 0);

        // Iniciar la corrutina de movimiento
        StartCoroutine(MoverPlataforma());
    }

    IEnumerator MoverPlataforma()
    {
        while (true)
        {
            // Esperar antes de moverse
            yield return new WaitForSeconds(tiempoEspera);

            enMovimiento = true;

            // Determinar hacia dónde moverse
            Vector3 destino = moviendoADerecha ? posicionObjetivo : posicionInicial;

            // Mover la plataforma hacia el destino
            while (Vector3.Distance(transform.position, destino) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    destino,
                    velocidad * Time.deltaTime
                );
                yield return null;
            }

            // Asegurar que llegue exactamente al destino
            transform.position = destino;

            enMovimiento = false;

            // Cambiar la dirección para el próximo movimiento
            moviendoADerecha = !moviendoADerecha;
        }
    }

    // Hacer que los objetos se muevan con la plataforma
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.SetParent(transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.SetParent(null);
        }
    }

    // Visualizar el recorrido en el editor
    private void OnDrawGizmos()
    {
        Vector3 inicio = Application.isPlaying ? posicionInicial : transform.position;
        Vector3 fin = inicio + new Vector3(distanciaMovimiento, 0, 0);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(inicio, fin);
        Gizmos.DrawWireSphere(inicio, 0.3f);
        Gizmos.DrawWireSphere(fin, 0.3f);
    }
}