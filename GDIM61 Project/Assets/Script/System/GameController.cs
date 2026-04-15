using System;
using UnityEngine;

public class GameController : MonoBehaviour // Singleton class that acts as a locator
{
    public static GameController Instance { get; private set; }

    public event Action OnSailStarted;
    public enum GameState
    {
        MainMenu,
        Sailing,
        Painting

       
    }
    public GameState currentState;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void StartSail()
    {
        if (currentState == GameState.Sailing)
            return;

        currentState = GameState.Sailing;
        OnSailStarted?.Invoke();
    }
    
    public void StartPaint()
    {
        if (currentState == GameState.Painting)
            return;
        currentState = GameState.Painting;

    }
}
