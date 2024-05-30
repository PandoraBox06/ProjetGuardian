using System;
using System.Collections.Generic;
using FMODUnity;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class WaveManager : MonoBehaviour
{
    [HideInInspector] public UnityEvent ShowNewWave;
    [SerializeField] Wave wave;
    public int numberOfWave { get; private set; }
    private int numberOfSuperWave = 0;
    private int enemySpawn;
    private int superEnemySpawn;
    [SerializeField] int baseNumberOfEnemy = 2;
    [SerializeField] private int addingNumberOfEnemyPerWave = 1;
    [SerializeField] private float factorMultiplicator = 1f;
    [SerializeField] private int superEnemyStartSpawn = 5;
    [SerializeField] int baseNumberOfSuperEnemy = 1;
    [SerializeField] private int addingNumberOfSuperEnemyPerWave = 1;
    [SerializeField] private float factorMultiplicatorSuperEnemy = 1f;
    private int totalNumberOfEnemyPerWave;
    private int totalNumberOfSuperEnemyPerWave;
    private List<GameObject> enemyDump;
    [SerializeField] private float delayBetweenSpawn = 1f;
    [SerializeField] private float radius;
    private bool fullWaveSpawn;
    private bool fullSuperWaveSpawn;
    private float spawnTimer;
    private float superSpawnTimer;

    [SerializeField] private bool canSpawnWave;
    [SerializeField] private KeyCode skippingWave;

    public static event Action<int> RefreshWaveCount;
    public static event Action OnWaveEnd;
    
    private void OnEnable()
    {
        Enemy.OnDeath += RemoveEnemyFromWave;
        GameManager.StartSpawningWave += StartSpawning;
    }

    private void OnDisable()
    {
        Enemy.OnDeath -= RemoveEnemyFromWave;
        GameManager.StartSpawningWave -= StartSpawning;
    }

    private void Start()
    {
        numberOfWave = 1;
        totalNumberOfEnemyPerWave = baseNumberOfEnemy;
        totalNumberOfSuperEnemyPerWave = baseNumberOfSuperEnemy;
        RefreshWaveCount?.Invoke(numberOfWave);
        enemyDump = new();
    }

    private void Update()
    {
        if (Input.GetKeyDown(skippingWave))
        {
            SkipWave();
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            SkipToWave8();
        }
        
        if(!canSpawnWave) return;
        
        if (fullWaveSpawn && fullSuperWaveSpawn)
        {
            if (enemyDump.Count <= 0)
            {
                OnWaveEnd?.Invoke();
                canSpawnWave = false;
                fullWaveSpawn = false;
                fullSuperWaveSpawn = false;
                enemySpawn = 0;
                superEnemySpawn = 0;
                WaveIncrease();
                GameManager.Instance.ChangeGameState(GameState.PostWave);
            }
        }

        if (!fullWaveSpawn)
        {
            SpawnWave();
        }

        if (numberOfWave < superEnemyStartSpawn)
        {
            fullSuperWaveSpawn = true;
        }

        if (numberOfWave >= superEnemyStartSpawn && !fullSuperWaveSpawn)
        {
            SpawnSuperWave();
        }
    }

    void SkipToWave8()
    {
        canSpawnWave = false;
        GameObject[] enemyDumping = enemyDump.ToArray();
        foreach (var enemy in enemyDumping)
        {
            enemy.GetComponent<Enemy>().Die();
        }
        fullWaveSpawn = false;
        fullSuperWaveSpawn = false;
        enemySpawn = 0;
        superEnemySpawn = 0;
        for (int i = 0; i < 7; i++)
        {
            WaveIncrease();
        }
        // RuntimeManager.StudioSystem.setParameterByName("Song", 3);
        GameManager.Instance.ChangeGameState(GameState.PostWave);
    }

    private void SpawnWave()
    {
        if(enemySpawn >= totalNumberOfEnemyPerWave) return;
        
        for (int i = 0; i < totalNumberOfEnemyPerWave; i++)
        {
            if (Time.time > spawnTimer)
            {
                //On tire une position random dans une sph�re :
                Vector3 rdPos = Random.insideUnitSphere;
                //On met sa position en y � z�ro, la position est donc maintenant DANS un cercle orient� sur xz :
                rdPos.y = 0f;
                //On normalise la pos, ce qui fait qu'elle est maintenant SUR un cercle de 1m de rayon, qu'on multiplie par radius : 
                rdPos = rdPos.normalized * radius;
                
                GameObject thisEnemy = Instantiate(wave.enemyPrefab, transform.position + rdPos, Quaternion.identity);
                enemyDump.Add(thisEnemy);
                thisEnemy.transform.parent = transform;
                enemySpawn++;
                spawnTimer = Time.time + delayBetweenSpawn;
            }

            if (enemySpawn >= totalNumberOfEnemyPerWave)
            {
                fullWaveSpawn = true;
            }
        }
    }
    
    private void SpawnSuperWave()
    {
        if(superEnemySpawn >= totalNumberOfSuperEnemyPerWave) return;
        
        for (int i = 0; i < totalNumberOfSuperEnemyPerWave; i++)
        {
            if (Time.time > superSpawnTimer)
            {
                //On tire une position random dans une sph�re :
                Vector3 rdPos = Random.insideUnitSphere;
                //On met sa position en y � z�ro, la position est donc maintenant DANS un cercle orient� sur xz :
                rdPos.y = 0f;
                //On normalise la pos, ce qui fait qu'elle est maintenant SUR un cercle de 1m de rayon, qu'on multiplie par radius : 
                rdPos = rdPos.normalized * radius;
                
                GameObject thisSuperEnemy = Instantiate(wave.superEnemyPrefab, transform.position + rdPos, Quaternion.identity);
                enemyDump.Add(thisSuperEnemy);
                thisSuperEnemy.transform.parent = transform;
                superEnemySpawn++;
                superSpawnTimer = Time.time + delayBetweenSpawn;
            }

            if (superEnemySpawn >= totalNumberOfSuperEnemyPerWave)
            {
                fullSuperWaveSpawn = true;
            }
        }
    }

    private void WaveIncrease()
    {
        numberOfWave++;
        RefreshWaveCount?.Invoke(numberOfWave);
        totalNumberOfEnemyPerWave = baseNumberOfEnemy + (numberOfWave * addingNumberOfEnemyPerWave) * (int)Mathf.Pow(factorMultiplicator, numberOfWave);
        totalNumberOfEnemyPerWave = Mathf.RoundToInt(totalNumberOfEnemyPerWave);
        
        if (numberOfWave >= superEnemyStartSpawn)
        {
            numberOfSuperWave++;
            totalNumberOfSuperEnemyPerWave = baseNumberOfSuperEnemy + (numberOfSuperWave * addingNumberOfSuperEnemyPerWave) * (int)Mathf.Pow(factorMultiplicatorSuperEnemy, numberOfSuperWave);
            totalNumberOfSuperEnemyPerWave = Mathf.RoundToInt(totalNumberOfSuperEnemyPerWave);
        }
        
        ShowNewWave?.Invoke();
    }

    private void RemoveEnemyFromWave(GameObject go)
    {
        enemyDump.Remove(go);
    }

    private void StartSpawning()
    {
        canSpawnWave = true;
    }

    private void SkipWave()
    {
        canSpawnWave = false;
        GameObject[] enemyDumping = enemyDump.ToArray();
        foreach (var enemy in enemyDumping)
        {
            enemy.GetComponent<Enemy>().Die();
        }
        fullWaveSpawn = false;
        fullSuperWaveSpawn = false;
        enemySpawn = 0;
        superEnemySpawn = 0;
        WaveIncrease();
        GameManager.Instance.ChangeGameState(GameState.PostWave);
    }
    
    // private void OnDrawGizmos()
    // {
    //     //++ Handles est une classe similaire � Gizmos, qui permet notamment de dessiner des arcs, disques, etc.
    //     //(L� o� Gizmos s'arr�te aux WireSphere & WireBox)
    //     Handles.color = Color.yellow;
    //     Handles.DrawWireDisc(transform.position, Vector3.up, radius);
    // }

    // private void OnGUI()
    // {
    //     GUILayout.Space(100f);
    //     GUILayout.Box("Press F1 to Start Wave", GUILayout.Width(150f), GUILayout.Height(25f));
    // }
}
