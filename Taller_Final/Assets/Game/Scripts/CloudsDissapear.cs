using System.Collections;
using UnityEngine;

public class DisappearingCloud : MonoBehaviour
{
    [Header("Configuración de Tiempo")]
    [SerializeField] private float tiempoAntesDeDesaparecer = 5f; // Tiempo que el player puede estar parado
    [SerializeField] private float tiempoParaReaparecer = 3f; // Tiempo para que vuelva a aparecer

    [Header("Configuración Visual con Lerp")]
    [SerializeField] private bool usarTransparencia = true; // Hacer la nube transparente antes de desaparecer
    [SerializeField] private float velocidadFade = 2f; // Velocidad del fade (más alto = más rápido)
    [SerializeField] private bool usarEscala = true; // Hacer que la nube se encoja al desaparecer
    [SerializeField] private float velocidadEscala = 3f; // Velocidad del cambio de escala

    [Header("Detección del Player")]
    [SerializeField] private float alturaDetecion = 1f; // Altura para detectar al player encima

    private bool playerEncima = false;
    private float tiempoConPlayer = 0f;
    private bool desapareciendo = false;
    private bool desaparecida = false;
    private bool reapareciendo = false;

    private MeshRenderer meshRenderer;
    private Collider nubeCollider;
    private Material materialOriginal;
    private Color colorOriginal;
    private Vector3 escalaOriginal;

    private float alphaObjetivo = 1f;
    private float escalaObjetivo = 1f;

    private Transform playerTransform;

    void Start()
    {
        // Obtener componentes
        meshRenderer = GetComponent<MeshRenderer>();
        nubeCollider = GetComponent<Collider>();

        // Guardar el material, color y escala original
        if (meshRenderer != null)
        {
            materialOriginal = meshRenderer.material;
            colorOriginal = materialOriginal.color;
        }

        escalaOriginal = transform.localScale;

        // Asegurar que el collider NO sea trigger para que el Character Controller colisione
        if (nubeCollider != null)
        {
            nubeCollider.isTrigger = false;
        }
    }

    void Update()
    {
        // Detectar si el player está encima usando raycast o bounds
        DetectarPlayerEncima();

        // Si el player está encima y la nube no está desaparecida
        if (playerEncima && !desaparecida && !desapareciendo)
        {
            tiempoConPlayer += Time.deltaTime;

            // Calcular qué tan cerca está de desaparecer (0 = inicio, 1 = va a desaparecer)
            float progreso = tiempoConPlayer / tiempoAntesDeDesaparecer;

            // Ajustar los objetivos según el progreso
            if (usarTransparencia)
            {
                alphaObjetivo = 1f - progreso; // De 1 a 0
            }

            if (usarEscala)
            {
                escalaObjetivo = 1f - (progreso * 0.3f); // De 1 a 0.7 (no desaparece completamente aún)
            }

            // Si se cumplió el tiempo, desaparecer
            if (tiempoConPlayer >= tiempoAntesDeDesaparecer)
            {
                StartCoroutine(DesaparecerNube());
            }
        }
        else if (!playerEncima && !desapareciendo && !desaparecida && !reapareciendo)
        {
            // Si el player se va antes de que desaparezca, volver a la normalidad
            if (tiempoConPlayer > 0)
            {
                tiempoConPlayer = Mathf.Max(0, tiempoConPlayer - Time.deltaTime * 2f); // Recuperación más rápida

                float progreso = tiempoConPlayer / tiempoAntesDeDesaparecer;

                alphaObjetivo = 1f;
                escalaObjetivo = 1f;
            }
        }

        // Aplicar Lerp para transiciones suaves
        AplicarEfectosVisuales();
    }

    void AplicarEfectosVisuales()
    {
        if (meshRenderer != null && usarTransparencia)
        {
            // Lerp para el alpha (transparencia)
            Color colorActual = materialOriginal.color;
            float alphaActual = colorActual.a;
            float nuevoAlpha = Mathf.Lerp(alphaActual, alphaObjetivo, Time.deltaTime * velocidadFade);

            colorActual.a = nuevoAlpha;
            materialOriginal.color = colorActual;
        }

        if (usarEscala)
        {
            // Lerp para la escala
            Vector3 escalaActual = transform.localScale;
            Vector3 escalaDeseada = escalaOriginal * escalaObjetivo;
            transform.localScale = Vector3.Lerp(escalaActual, escalaDeseada, Time.deltaTime * velocidadEscala);
        }
    }

    void DetectarPlayerEncima()
    {
        if (desaparecida || nubeCollider == null)
        {
            playerEncima = false;
            return;
        }

        // Obtener los bounds del collider de la nube
        Bounds bounds = nubeCollider.bounds;

        // Crear un área de detección justo encima de la nube
        Vector3 centroDeteccion = new Vector3(bounds.center.x, bounds.max.y + (alturaDetecion * 0.5f), bounds.center.z);
        Vector3 tamañoDeteccion = new Vector3(bounds.size.x * 0.9f, alturaDetecion, bounds.size.z * 0.9f);

        // Buscar colliders en esa área
        Collider[] collidersDetectados = Physics.OverlapBox(centroDeteccion, tamañoDeteccion * 0.5f);

        playerEncima = false;

        foreach (Collider col in collidersDetectados)
        {
            if (col.CompareTag("Player"))
            {
                // Verificar que el player esté realmente encima (no debajo)
                CharacterController cc = col.GetComponent<CharacterController>();
                if (cc != null)
                {
                    // El player está encima si su parte inferior está cerca de la parte superior de la nube
                    float playerBottom = cc.bounds.min.y;
                    float nubeTop = bounds.max.y;

                    if (playerBottom >= nubeTop - 0.5f && playerBottom <= nubeTop + alturaDetecion)
                    {
                        playerEncima = true;
                        playerTransform = col.transform;
                        break;
                    }
                }
            }
        }
    }

    IEnumerator DesaparecerNube()
    {
        desapareciendo = true;

        // Establecer objetivos para desaparecer completamente
        alphaObjetivo = 0f;
        escalaObjetivo = 0f;

        // Esperar a que los efectos visuales se completen con Lerp
        float tiempoEspera = Mathf.Max(1f / velocidadFade, 1f / velocidadEscala);
        yield return new WaitForSeconds(tiempoEspera);

        // Desactivar la visibilidad y colisión
        if (meshRenderer != null)
            meshRenderer.enabled = false;

        if (nubeCollider != null)
            nubeCollider.enabled = false;

        desaparecida = true;
        desapareciendo = false;

        // Esperar el tiempo para reaparecer
        yield return new WaitForSeconds(tiempoParaReaparecer);

        // Reaparecer
        StartCoroutine(ReaparecerNube());
    }

    IEnumerator ReaparecerNube()
    {
        reapareciendo = true;

        // Reactivar la visibilidad y colisión
        if (meshRenderer != null)
        {
            meshRenderer.enabled = true;
        }

        if (nubeCollider != null)
            nubeCollider.enabled = true;

        // Establecer objetivos para reaparecer
        alphaObjetivo = 1f;
        escalaObjetivo = 1f;

        desaparecida = false;

        // Esperar a que los efectos visuales se completen
        float tiempoEspera = Mathf.Max(1f / velocidadFade, 1f / velocidadEscala);
        yield return new WaitForSeconds(tiempoEspera);

        // Resetear variables
        playerEncima = false;
        tiempoConPlayer = 0f;
        reapareciendo = false;
    }

    // Visualización en el editor
    private void OnDrawGizmos()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            Bounds bounds = col.bounds;

            // Dibujar la nube
            if (desaparecida)
            {
                Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
            }
            else if (playerEncima)
            {
                Gizmos.color = new Color(1f, 1f, 0f, 0.5f);
            }
            else
            {
                Gizmos.color = new Color(1f, 1f, 1f, 0.3f);
            }

            Gizmos.DrawWireCube(bounds.center, bounds.size);

            // Dibujar el área de detección
            Gizmos.color = new Color(0f, 1f, 0f, 0.2f);
            Vector3 centroDeteccion = new Vector3(bounds.center.x, bounds.max.y + (alturaDetecion * 0.5f), bounds.center.z);
            Vector3 tamañoDeteccion = new Vector3(bounds.size.x * 0.9f, alturaDetecion, bounds.size.z * 0.9f);
            Gizmos.DrawWireCube(centroDeteccion, tamañoDeteccion);
        }
    }
}