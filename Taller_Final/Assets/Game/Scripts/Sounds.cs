using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AmbienceSound : MonoBehaviour
{
    [Tooltip("Área del sonido (Collider con 'Is Trigger' activado)")]
    [SerializeField] private Collider area;

    [Tooltip("Jugador o GameObject que se rastreará")]
    [SerializeField] private Transform player;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        // Validaciones básicas
        if (area == null)
            Debug.LogWarning($"{name}: No se asignó ningún Collider como área de sonido.", this);

        if (player == null)
            Debug.LogWarning($"{name}: No se asignó ningún jugador para rastrear.", this);
    }

    private void Update()
    {
        if (area == null || player == null)
            return;

        // Encuentra el punto más cercano dentro del área al jugador
        Vector3 closestPoint = area.ClosestPoint(player.position);

        // Mueve el objeto del sonido a esa posición
        transform.position = closestPoint;

        // (Opcional) Ajusta el volumen según la distancia del jugador al centro del área
        float distance = Vector3.Distance(player.position, area.bounds.center);
        float maxDistance = area.bounds.extents.magnitude;
        audioSource.volume = Mathf.Lerp(1f, 0f, distance / maxDistance);
    }
}