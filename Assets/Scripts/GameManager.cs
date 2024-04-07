using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    Lobby,
    Tutorial,
    PreWave,
    InWave,
    PostWave,
    Defeat,
    Pause
}


public class GameManager : MonoBehaviour
{
    [SerializeField] private GameState currentGameState;
    [SerializeField] float delayBeforeWaveStart = 3f;
    [SerializeField] private int currentWave;
    [SerializeField] private KeyCode waveActivator;
    private float timer;

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
    }
    private void OnDestroy()
    {
        if (this == Instance)
            Instance = null;
    }

    private void OnEnable()
    {
        WaveManager.RefreshWaveCount += GetWaveNumber;
    }

    private void OnDisable()
    {
        WaveManager.RefreshWaveCount -= GetWaveNumber;
    }

    private void Start()
    {
        timer = delayBeforeWaveStart;
    }

    private void Update()
    {
        if (Input.GetKeyDown(waveActivator))
        {
            ChangeGameState(GameState.PreWave);
        }
        
        switch (currentGameState)
        {
            case GameState.Lobby:
                Lobby();
                break;
            case GameState.Tutorial:
                Tutorial();
                break;
            case GameState.PreWave:
                PreWave();
                break;
            case GameState.InWave:
                InWave();
                break;
            case GameState.PostWave:
                PostWave();
                break;
            case GameState.Defeat:
                Defeat();
                break;
            case GameState.Pause:
                Pause();
                break;
        }
    }

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

    public void ChangeGameState(GameState state)
    {
        if (state == currentGameState) return;
        currentGameState = state;
    }

    public void GetWaveNumber(int wave)
    {
        currentWave = wave;
    }
    
    private void OnGUI()
    {
        GUILayout.Space(100f);
        GUILayout.Box($"Press {waveActivator} to Start Wave", GUILayout.Width(150f), GUILayout.Height(25f));
    }
}
