using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    private Vector3 lastCheckpointPosition;
    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        // Guarda la posición inicial como el primer checkpoint
        lastCheckpointPosition = transform.position;
    }

    void Update()
    {
        // Si el jugador cae por debajo de cierto punto, reaparece
        if (transform.position.y < -10f)
        {
            Respawn();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Detecta si toca un checkpoint
        if (other.CompareTag("Checkpoint"))
        {
            // Guarda la posición de respawn
            lastCheckpointPosition = other.transform.position;
            Debug.Log("Checkpoint actualizado: " + lastCheckpointPosition);

            // Busca si el checkpoint tiene un sistema de partículas de fuego
            ParticleSystem fuego = other.GetComponentInChildren<ParticleSystem>();
            if (fuego != null && !fuego.isPlaying)
            {
                fuego.Play(); // Enciende las partículas si aún no lo estaban
                Debug.Log("Partículas del checkpoint activadas");
            }
        }
    }

    void Respawn()
    {
        // Desactiva temporalmente el CharacterController para moverlo sin errores
        controller.enabled = false;
        transform.position = lastCheckpointPosition;
        controller.enabled = true;

        Debug.Log("Jugador reapareció en el último checkpoint");
    }
}