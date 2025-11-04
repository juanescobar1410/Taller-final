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
    private Vector3 posicionAnterior;
    private bool moviendoADerecha = true;
    private bool enMovimiento = false;

    // Para manejar el Character Controller
    private CharacterController playerController;
    private bool playerEnPlataforma = false;

    void Start()
    {
        // Guardar la posición inicial
        posicionInicial = transform.position;
        posicionAnterior = transform.position;

        // Calcular la posición objetivo (a la derecha)
        posicionObjetivo = posicionInicial + new Vector3(distanciaMovimiento, 0, 0);

        // Iniciar la corrutina de movimiento
        StartCoroutine(MoverPlataforma());
    }

    void LateUpdate()
    {
        // Si hay un player en la plataforma, moverlo con ella
        if (playerEnPlataforma && playerController != null)
        {
            // Calcular cuánto se movió la plataforma este frame
            Vector3 movimientoPlataforma = transform.position - posicionAnterior;

            // Mover al player la misma cantidad
            playerController.Move(movimientoPlataforma);
        }

        // Guardar la posición actual para el próximo frame
        posicionAnterior = transform.position;
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

    // Detectar cuando el player entra en la plataforma
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            CharacterController cc = collision.gameObject.GetComponent<CharacterController>();
            if (cc != null)
            {
                playerController = cc;
                playerEnPlataforma = true;
            }
        }
    }

    // Detectar cuando el player sale de la plataforma
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerEnPlataforma = false;
            playerController = null;
        }
    }

    // Mantener la detección mientras el player está en contacto
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Verificar que el player está realmente encima (no tocando los lados)
            foreach (ContactPoint contact in collision.contacts)
            {
                // Si el punto de contacto tiene una normal hacia arriba, está encima
                if (contact.normal.y > 0.5f)
                {
                    if (playerController == null)
                    {
                        CharacterController cc = collision.gameObject.GetComponent<CharacterController>();
                        if (cc != null)
                        {
                            playerController = cc;
                            playerEnPlataforma = true;
                        }
                    }
                    return;
                }
            }

            // Si no hay contacto por arriba, el player no está encima
            playerEnPlataforma = false;
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

        // Indicador visual si el player está en la plataforma
        if (Application.isPlaying && playerEnPlataforma)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position + Vector3.up * 0.5f, Vector3.one * 0.5f);
        }
    }
}