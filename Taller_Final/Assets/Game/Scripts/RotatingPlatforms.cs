using UnityEngine;

public class RotatingPlatform : MonoBehaviour
{
    [Header("Configuración de Rotación")]
    [SerializeField] private Transform puntoDeRotacion; // El centro alrededor del cual girará (tu isla)
    [SerializeField] private float velocidadRotacion = 30f; // Grados por segundo
    [SerializeField] private float radio = 10f; // Distancia desde el centro
    [SerializeField] private Vector3 ejeRotacion = Vector3.up; // Eje de rotación (Y por defecto)

    [Header("Configuración Inicial")]
    [SerializeField] private float anguloInicial = 0f; // Ángulo inicial en grados

    private float anguloActual;

    void Start()
    {
        // Si no se asigna un punto de rotación, usar la posición actual como referencia
        if (puntoDeRotacion == null)
        {
            Debug.LogWarning("No se asignó un punto de rotación. Usa un GameObject vacío como centro.");
        }

        anguloActual = anguloInicial;
        ActualizarPosicion();
    }

    void Update()
    {
        if (puntoDeRotacion == null) return;

        // Incrementar el ángulo según la velocidad
        anguloActual += velocidadRotacion * Time.deltaTime;

        // Mantener el ángulo entre 0 y 360
        if (anguloActual >= 360f)
            anguloActual -= 360f;

        ActualizarPosicion();
    }

    void ActualizarPosicion()
    {
        if (puntoDeRotacion == null) return;

        // Convertir el ángulo a radianes
        float radianes = anguloActual * Mathf.Deg2Rad;

        // Calcular la nueva posición en círculo
        Vector3 offset;

        // Si gira alrededor del eje Y (horizontal)
        if (ejeRotacion == Vector3.up)
        {
            offset = new Vector3(
                Mathf.Cos(radianes) * radio,
                0,
                Mathf.Sin(radianes) * radio
            );
        }
        // Si gira alrededor del eje X (vertical frontal)
        else if (ejeRotacion == Vector3.right)
        {
            offset = new Vector3(
                0,
                Mathf.Sin(radianes) * radio,
                Mathf.Cos(radianes) * radio
            );
        }
        // Si gira alrededor del eje Z (vertical lateral)
        else if (ejeRotacion == Vector3.forward)
        {
            offset = new Vector3(
                Mathf.Cos(radianes) * radio,
                Mathf.Sin(radianes) * radio,
                0
            );
        }
        else
        {
            // Eje personalizado
            offset = Quaternion.AngleAxis(anguloActual, ejeRotacion) * (Vector3.forward * radio);
        }

        // Aplicar la nueva posición
        transform.position = puntoDeRotacion.position + offset;

        // Opcional: Hacer que la plataforma mire hacia el centro
        // transform.LookAt(puntoDeRotacion);
    }

    // Hacer que el jugador se mueva con la plataforma
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

    // Visualizar la órbita en el editor
    private void OnDrawGizmos()
    {
        if (puntoDeRotacion == null) return;

        Gizmos.color = Color.cyan;

        // Dibujar el círculo de la órbita
        int segmentos = 50;
        float anguloPorSegmento = 360f / segmentos;

        for (int i = 0; i < segmentos; i++)
        {
            float angulo1 = i * anguloPorSegmento * Mathf.Deg2Rad;
            float angulo2 = (i + 1) * anguloPorSegmento * Mathf.Deg2Rad;

            Vector3 punto1, punto2;

            if (ejeRotacion == Vector3.up)
            {
                punto1 = puntoDeRotacion.position + new Vector3(Mathf.Cos(angulo1) * radio, 0, Mathf.Sin(angulo1) * radio);
                punto2 = puntoDeRotacion.position + new Vector3(Mathf.Cos(angulo2) * radio, 0, Mathf.Sin(angulo2) * radio);
            }
            else if (ejeRotacion == Vector3.right)
            {
                punto1 = puntoDeRotacion.position + new Vector3(0, Mathf.Sin(angulo1) * radio, Mathf.Cos(angulo1) * radio);
                punto2 = puntoDeRotacion.position + new Vector3(0, Mathf.Sin(angulo2) * radio, Mathf.Cos(angulo2) * radio);
            }
            else
            {
                punto1 = puntoDeRotacion.position + new Vector3(Mathf.Cos(angulo1) * radio, Mathf.Sin(angulo1) * radio, 0);
                punto2 = puntoDeRotacion.position + new Vector3(Mathf.Cos(angulo2) * radio, Mathf.Sin(angulo2) * radio, 0);
            }

            Gizmos.DrawLine(punto1, punto2);
        }

        // Dibujar línea desde el centro hasta la plataforma
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(puntoDeRotacion.position, transform.position);
    }
}