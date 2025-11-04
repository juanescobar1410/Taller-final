using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private float globalTime = 0f;

    private int score = 0;
    private int itemsCount = 0;

    public float GlobalTime { get => globalTime; set => globalTime = value; }
    public int Score { get => score; set => score = value; }
    public int ItemsCount { get => itemsCount; set => itemsCount = value; }

    void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // opcional, persiste entre escenas
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddTime(float timeScene)
    {
        globalTime += timeScene;
    }
    public void AddScore(int scoreItem)
    {
        score += scoreItem;
    }

    public void AddItem()
    {
        itemsCount++;
    }
}
