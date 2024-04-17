using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public enum GameState
{
    Lobby,
    Tutorial,
    PreWave,
    InWave,
    PostWave,
    Defeat,
    Pause,
    Null
}


public class GameManager : MonoBehaviour
{
    [SerializeField] private GameState currentGameState;
    [SerializeField] float delayBeforeWaveStart = 3f;
    [SerializeField] private int currentWave;
    [SerializeField] private InputActionReference pauseInput;
    private float timer;
    private GameState stateBeforePause;
    private bool isTutorialDone;
    private Action GameStateAction;

    public event Action StartSpawningWave;
    public static GameManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        GameStateAction = Lobby;
    }
    private void OnDestroy()
    {
        if (this == Instance)
            Instance = null;
    }

    private void OnEnable()
    {
        WaveManager.RefreshWaveCount += GetWaveNumber;
        pauseInput.action.performed += PauseInput;
    }

    private void OnDisable()
    {
        WaveManager.RefreshWaveCount -= GetWaveNumber;
    }

    private void Start()
    {
        timer = delayBeforeWaveStart;
        stateBeforePause = GameState.Null;
    }

    private void Update()
    {
        GameStateAction();
    }

    #region StateMachine
    void Lobby()
    {
        //Main Menu
    }

    void Tutorial()
    {
        //Tutorial Related
    }

    void PreWave()
    {
        //Before Wave
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            StartSpawningWave?.Invoke();
            ChangeGameState(GameState.InWave);
        }
    }

    void InWave()
    {
        // While the player kill enemy
    }

    void PostWave()
    {
        // After Wave
        timer = delayBeforeWaveStart;
        ChangeGameState(GameState.PreWave);
    }

    void Defeat()
    {
        // Player Died
    }

    void Pause()
    {
        // Menu while in Game
    }
    #endregion

    public bool IsInPlayMode()
    {
        if (GameStateAction == Lobby || GameStateAction == Pause || GameStateAction == Defeat)
        {
            return false;
        }
        return true;
    }
    
    public void StartGame()
    {
        //methods to launch to enter gameMode
        UIManager.Instance.LockCursor();
        if (isTutorialDone) ChangeGameState(GameState.PreWave);
        else ChangeGameState(GameState.Tutorial);
    }

    public void StopGame() //jsp à quoi ça va servir lol
    {
        //methods to launch to exit gameMode
        UIManager.Instance.UnlockCursor();
    }

    public void PauseInput(InputAction.CallbackContext callback)
    {
        //Pause
        if (IsInPlayMode())
        {
            stateBeforePause = currentGameState;
            ChangeGameState(GameState.Pause);
            PauseGame();
        }
        //Play
        else if (GameStateAction == Pause)
        {
            if (stateBeforePause == GameState.Null)
            {
                Debug.LogWarning("No state is registered, we cannot get out of pause");
                return;
            }
            ChangeGameState(stateBeforePause);
            stateBeforePause = GameState.Null;
            ContinueGame();
        } 
    }

    public void PauseGame()
    {
        UIManager.Instance.UnlockCursor();
        Time.timeScale = 0;
        
        UIManager.Instance.OpenOnePanel(PanelsNames.Pause);
    }

    public void ContinueGame()
    {
        UIManager.Instance.LockCursor();
        Time.timeScale = 1;
        
        UIManager.Instance.ClosePanels();
    }

    public void ChangeGameState(GameState state)
    {
        if (state == currentGameState) return;

        currentGameState = state;
        switch (currentGameState)
        {
            case GameState.Lobby:
                GameStateAction = Lobby;
                break;
            case GameState.Tutorial:
                GameStateAction = Tutorial;
                break;
            case GameState.PreWave:
                GameStateAction = PreWave;
                break;
            case GameState.InWave:
                GameStateAction = InWave;
                break;
            case GameState.PostWave:
                GameStateAction = PostWave;
                break;
            case GameState.Defeat:
                GameStateAction = Defeat;
                break;
            case GameState.Pause:
                GameStateAction = Pause;
                break;
        }
    }

    public void GetWaveNumber(int wave)
    {
        currentWave = wave;
    }
}
