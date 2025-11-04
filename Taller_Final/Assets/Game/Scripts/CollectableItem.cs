using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class CollectableItem : MonoBehaviour
{
    public enum ItemType { Cruz, Dianmita }  
    public ItemType itemType;
    public int itemValue = 0;
    

    private Transform player;
   

    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CollectItem();
        }
    }
    
    void Update()
    {


    }

    void CollectItem()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(itemValue);
            GameManager.Instance.AddItem();
        }

        Destroy(gameObject);
    }
}
