//using System.Runtime.CompilerServices;
//using UnityEngine;
//using UnityEngine.InputSystem;

//public class CollectableItem : MonoBehaviour
//{
//    public enum ItemType { Cruz, Dinamita }  
//    public ItemType itemType;
//    public int itemValue = 0;


//    private Transform player;


//    void Start()
//    {

//    }

//    private void OnTriggerEnter(Collider other)
//    {
//        if (other.CompareTag("Player"))
//        {
//            CollectItem();
//        }
//    }

//    void Update()
//    {


//    }

//    void CollectItem()
//    {
//        if (GameManager.Instance != null)
//        {
//            switch (itemType)
//            {
//                case ItemType.Cruz:
//                    GameManager.Instance.AddScore(itemValue);

//                    break;

//                case ItemType.Dinamita:
//                    GameManager.Instance.AddScore(itemValue);
//                    GameManager.Instance.AddItem();
//                    break;
//            }

//        }




//        Destroy(gameObject);
//    }
//}
using UnityEngine;
using UnityEngine.InputSystem;

public class CollectableItem : MonoBehaviour
{
    public enum ItemType { Cruz, Dinamita }
    public ItemType itemType;
    public int itemValue = 0;

    [Header("Sonidos")]
    public AudioClip cruzSound;      // Sonido para cruz
    public AudioClip dinamitaSound;  // Sonido para dinamita
    public float soundVolume = 1f;   // Volumen del sonido (0-1)

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CollectItem();
        }
    }

    void CollectItem()
    {
        if (GameManager.Instance != null)
        {
            switch (itemType)
            {
                case ItemType.Cruz:
                    GameManager.Instance.AddScore(itemValue);
                    if (cruzSound != null)
                        AudioSource.PlayClipAtPoint(cruzSound, transform.position, soundVolume);
                    break;

                case ItemType.Dinamita:
                    GameManager.Instance.AddScore(itemValue);
                    GameManager.Instance.AddItem();
                    if (dinamitaSound != null)
                        AudioSource.PlayClipAtPoint(dinamitaSound, transform.position, soundVolume);
                    break;
            }
        }

        // Destruye el ítem después de reproducir el sonido
        Destroy(gameObject);
    }
}