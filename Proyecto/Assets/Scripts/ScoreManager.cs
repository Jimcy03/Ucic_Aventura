using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }
    
    private int currentScore;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void AddScore(int points)
    {
        currentScore += points;
        Debug.Log($"Score: {currentScore}");
        // Aquí puedes actualizar la UI del puntaje
    }
    
    public int GetCurrentScore()
    {
        return currentScore;
    }
}