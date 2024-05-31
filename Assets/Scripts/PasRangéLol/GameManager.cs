using System;
using Cinemachine;
using FMODUnity;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.PostProcessing;

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

    [SerializeField] private bool _skipCutscene;
    public bool IsTutorialDone
    {
        get => isTutorialDone;
        set => isTutorialDone = value;
    }

    private bool _cutsceneOnce;
    private Action currentAction;
    [SerializeField] private GameObject _CutSceneManager;
    [SerializeField] private GameObject _PlayerCanvas;
    [SerializeField] private GameObject _UICanvas;
    [SerializeField] private GameObject TutoCanvas;
    [SerializeField] private GameObject PlayerHp;
    [SerializeField] private PostProcessVolume _TutoVolume;


    [SerializeField] private StudioEventEmitter menuAmbiant;
    [SerializeField] private StudioEventEmitter musicEmitter;
    public PostProcessVolume TutoVolume
    {
        get => _TutoVolume;
        set => _TutoVolume = value;
    }

    [SerializeField] private CinemachineBrain _cameraBrain;
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
        _cameraBrain.enabled = false;
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
        _CutSceneManager.SetActive(false);
        ChangeGameState(UIManager.Instance.startWithMenu ? GameState.Lobby : GameState.Cutscene);
        menuAmbiant.Play();
    }

    private void Update()
    {
        currentAction?.Invoke();
        if (Input.GetKeyDown(KeyCode.F1))
        {
            SkipToTuto();
        }
        switch (currentWave)
        {
            case 1:
                RuntimeManager.StudioSystem.setParameterByName("Song", 1);
                break;
            case 5:
                RuntimeManager.StudioSystem.setParameterByName("Song", 2);
                break;
            case 8:
                RuntimeManager.StudioSystem.setParameterByName("Song", 3);
                break;
            case 11:
                RuntimeManager.StudioSystem.setParameterByName("Song", 4);
                break;
        }
    }

    void SkipToTuto()
    {
        _cutsceneOnce = true;
        _cameraBrain.enabled = true;
        _CutSceneManager.SetActive(false);
        PlayerInit.Instance.EnablePlayer();
        BlancoCombatManager.Instance.Init();
        ChangeGameState(GameState.Tutorial);
    }
    
    #region StateMachine
    void Lobby()
    {
        if (_PlayerCanvas.activeInHierarchy) _PlayerCanvas.SetActive(false);
        if (!_UICanvas.activeInHierarchy) _UICanvas.SetActive(true);
        if (PlayerHp.activeInHierarchy) PlayerHp.SetActive(false);
        if (TutoCanvas.activeInHierarchy) TutoCanvas.SetActive(false);
    }

    void CutScene()
    {
        menuAmbiant.Stop();
        musicEmitter.Play();
        RuntimeManager.StudioSystem.setParameterByName("Song", 0);
        if (_PlayerCanvas.activeInHierarchy) _PlayerCanvas.SetActive(false);
        if (_UICanvas.activeInHierarchy) _UICanvas.SetActive(false);
        if (PlayerHp.activeInHierarchy) PlayerHp.SetActive(false);
        if (TutoCanvas.activeInHierarchy) TutoCanvas.SetActive(false);
        
        if (_cameraBrain.enabled == false) _cameraBrain.enabled = true;
        if (_skipCutscene || UIManager.Instance.startWithMenu == false)
        {
            PlayerInit.Instance.EnablePlayer();
            BlancoCombatManager.Instance.Init();
            ChangeGameState(GameState.Tutorial);
        }
        else
        {
            if (_cutsceneOnce) return;
            _CutSceneManager.SetActive(true);
            _cutsceneOnce = true;
        }
    }

    void Tutorial()
    {
        if (!_PlayerCanvas.activeInHierarchy) _PlayerCanvas.SetActive(true);
        if (_UICanvas.activeInHierarchy) _UICanvas.SetActive(false);
        if (PlayerHp.activeInHierarchy) PlayerHp.SetActive(false);
        if (!TutoCanvas.activeInHierarchy) TutoCanvas.SetActive(true);
    }

    void PreWave()
    {
        if (_TutoVolume.enabled) _TutoVolume.enabled = false;
        if (!_PlayerCanvas.activeInHierarchy) _PlayerCanvas.SetActive(true);
        if (_UICanvas.activeInHierarchy) _UICanvas.SetActive(false);
        if (PlayerHp.activeInHierarchy) PlayerHp.SetActive(true);
        if (TutoCanvas.activeInHierarchy) TutoCanvas.SetActive(false);
        
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
        if (_PlayerCanvas.activeInHierarchy) _PlayerCanvas.SetActive(false);
        if (!_UICanvas.activeInHierarchy) _UICanvas.SetActive(true);
        if (PlayerHp.activeInHierarchy) PlayerHp.SetActive(false);
        if (TutoCanvas.activeInHierarchy) TutoCanvas.SetActive(false);
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
        _cameraBrain.enabled = true;
        if (isTutorialDone) ChangeGameState(GameState.PreWave);
        else ChangeGameState(GameState.Cutscene);
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

    public void ChangeToTuto()
    {
        PlayerInit.Instance.EnablePlayer();
        BlancoCombatManager.Instance.Init();
        ChangeGameState(GameState.Tutorial);
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
