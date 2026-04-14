using System;
using UnityEngine;

public class GameController : MonoBehaviour // Singleton class that acts as a locator
{
    public static GameController Instance { get; private set; }

    public event Action OnGameStarted;
    public bool IsGameStarted { get; private set; }

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

    void Update()
    {
        if (IsGameStarted) // Checks if the game has started
            return;

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            IsGameStarted = true;
            OnGameStarted?.Invoke(); // Sends the OnGameStarted event
        }
    }
}
