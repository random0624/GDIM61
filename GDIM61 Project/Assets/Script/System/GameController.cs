using System;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    public event Action OnSailStarted;
    public event Action OnMainMenuStarted;
    public event Action OnPaintStarted;
    public event Action<GameState> OnStateChanged;

    public enum GameState
    {
        MainMenu,
        Sailing,
        Painting
    }

    [SerializeField] public  GameState currentState = GameState.MainMenu;
    public GameState CurrentState => currentState;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        EnterState(currentState);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void StartSail()
    {
        ChangeState(GameState.Sailing);
    }

    public void StartPaint()
    {
        ChangeState(GameState.Painting);
    }

    public void StartMainMenu()
    {
        ChangeState(GameState.MainMenu);
    }

    public void ChangeState(GameState newState)
    {
        if (currentState == newState)
            return;

        currentState = newState;
        EnterState(newState);
    }

    public void EnterState(GameState state)
    {
        OnStateChanged?.Invoke(state);

        switch (state)
        {
            case GameState.MainMenu:
                OnMainMenuStarted?.Invoke();
                break;

            case GameState.Sailing:
                OnSailStarted?.Invoke();
                break;

            case GameState.Painting:
                OnPaintStarted?.Invoke();
                break;
        }
    }
}
