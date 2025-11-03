using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public Camera followCamera; // Tu única cámara (con FollowCamera)

    private void Start()
    {
        // Desactivar cualquier otra cámara en la escena
        Camera[] allCameras = FindObjectsOfType<Camera>();
        foreach (Camera cam in allCameras)
        {
            if (cam != followCamera)
                cam.gameObject.SetActive(false);
        }

        // Activar solo la cámara principal de seguimiento
        followCamera.gameObject.SetActive(true);
        Debug.Log("Solo la cámara FollowCamera está activa.");
    }
}