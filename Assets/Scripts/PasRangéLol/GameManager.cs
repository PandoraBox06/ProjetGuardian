using System;
using UnityEngine;
using UnityEngine.InputSystem;

public enum GameState
{
    Lobby,
    Cutscene,
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
    public GameState currentGameState { get; private set; }

    [SerializeField] float delayBeforeWaveStart = 3f;
    [SerializeField] private int currentWave;
    [SerializeField] private InputActionReference pauseInput;
    private float timer;
    private GameState stateBeforePause;
    private bool isTutorialDone;

    private Action currentAction;
    [SerializeField] private GameObject _CutSceneManager;
    [SerializeField] private GameObject _PlayerCanvas;
    [SerializeField] private GameObject _UICanvas;
    public static event Action StartSpawningWave;
    public static event Action OnFullRegen;
    public static GameManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
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

        ChangeGameState(UIManager.Instance.startWithMenu ? GameState.Lobby : GameState.Cutscene);
    }

    private void Update()
    {
        currentAction?.Invoke();
    }

    #region StateMachine
    void Lobby()
    {
        if (!_PlayerCanvas.activeInHierarchy) _PlayerCanvas.SetActive(false);
        if (!_UICanvas.activeInHierarchy) _UICanvas.SetActive(true);
    }

    void CutScene()
    {
        if (_CutSceneManager.activeInHierarchy == false)
        {
            BlancoCombatManager.Instance.Init();
            ChangeGameState(GameState.Tutorial);
        }
    }

    void Tutorial()
    {
        
    }

    void PreWave()
    {
        if (_PlayerCanvas.activeInHierarchy) _PlayerCanvas.SetActive(true);
        if (_UICanvas.activeInHierarchy) _UICanvas.SetActive(false);
        
        //Before Wave
        timer -= Time.deltaTime;
        if (!(timer <= 0)) return;
        StartSpawningWave?.Invoke();
        ChangeGameState(GameState.InWave);
    }

    void InWave()
    {
        // While the player kill enemy
    }

    void PostWave()
    {
        // After Wave
        timer = delayBeforeWaveStart;
        OnFullRegen?.Invoke();
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


    public bool IsPlaying()
    {
        if (currentGameState == GameState.Lobby ||
            currentGameState == GameState.Defeat || currentGameState == GameState.Pause)
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
        if (!IsPlaying())
        {
            stateBeforePause = currentGameState;
            ChangeGameState(GameState.Pause);
            PauseGame();
        }

        //Play
        else if (currentGameState == GameState.Pause)
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

        currentAction = currentGameState switch
        {
            GameState.Lobby => Lobby,
            GameState.Tutorial => Tutorial,
            GameState.PreWave => PreWave,
            GameState.InWave => InWave,
            GameState.PostWave => PostWave,
            GameState.Defeat => Defeat,
            GameState.Pause => Pause,
            GameState.Cutscene => CutScene,
            _ => currentAction
        };
    }

    public void GetWaveNumber(int wave)
    {
        currentWave = wave;
    }
}
