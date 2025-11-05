using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    private Vector3 lastCheckpointPosition;
    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        lastCheckpointPosition = transform.position;
    }

    void Update()
    {
        if (transform.position.y < -25f)
        {
            Respawn();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Checkpoint"))
        {
            lastCheckpointPosition = other.transform.position;
            Debug.Log("Checkpoint actualizado: " + lastCheckpointPosition);

            ParticleSystem fuego = other.GetComponentInChildren<ParticleSystem>();
            if (fuego != null && !fuego.isPlaying)
            {
                fuego.Play();
                Debug.Log("Partículas del checkpoint activadas");
            }
        }
    }

    void Respawn()
    {
        controller.enabled = false;
        transform.position = lastCheckpointPosition;
        controller.enabled = true;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddFall();
        }

        Debug.Log("Jugador reapareció en el último checkpoint");
    }
}
