using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    [SerializeField] Wave[] waves;
    private int numberOfWave;
    private int actualWave;
    private int enemySpawn;
    private List<GameObject> enemyDump;
    [SerializeField] float delayBeforeWaveStart = 3f;
    [SerializeField] private float delayBetweenSpawn = 1f;
    [SerializeField] private float radius;
    private bool fullWaveSpawn;
    private float spawnTimer;
    private float waveTimer;
    private void OnEnable()
    {
        Enemy.OnDeath += RemoveEnemyFromWave;
    }

    private void OnDisable()
    {
        Enemy.OnDeath -= RemoveEnemyFromWave;
    }

    private void Start()
    {
        numberOfWave = waves.Length;
        waveTimer = Time.time + delayBeforeWaveStart;
    }

    private void Update()
    {
        if (actualWave >= numberOfWave)
        {
            return;
        }
        
        if (fullWaveSpawn)
        {
            if (enemyDump.Count <= 0)
            {
                fullWaveSpawn = false;
                actualWave++;
                enemySpawn = 0;
                waveTimer = Time.time + delayBeforeWaveStart;
            }
        }

        if (Time.time > waveTimer && !fullWaveSpawn)
        {
            SpawnWave();
        }
    }

    private void SpawnWave()
    {
        if(enemySpawn >= waves[actualWave].numberOfEnemyForTheWave) return;
        
        for (int i = 0; i < waves[actualWave].numberOfEnemyForTheWave; i++)
        {
            if (Time.time > spawnTimer)
            {
                //On tire une position random dans une sphère :
                Vector3 rdPos = Random.insideUnitSphere;
                //On met sa position en y à zéro, la position est donc maintenant DANS un cercle orienté sur xz :
                rdPos.y = 0f;
                //On normalise la pos, ce qui fait qu'elle est maintenant SUR un cercle de 1m de rayon, qu'on multiplie par radius : 
                rdPos = rdPos.normalized * radius;
                
                GameObject thisEnemy = Instantiate(waves[actualWave].enemyPrefab, transform.position + rdPos, Quaternion.identity);
                enemyDump.Add(thisEnemy);
                thisEnemy.transform.parent = transform;
                enemySpawn++;
                spawnTimer = Time.time + delayBetweenSpawn;
            }
            if (enemySpawn >= waves[actualWave].numberOfEnemyForTheWave) fullWaveSpawn = true;
        }
    }

    private void RemoveEnemyFromWave(GameObject go)
    {
        enemyDump.Remove(go);
    }
    
    private void OnDrawGizmos()
    {
        //++ Handles est une classe similaire à Gizmos, qui permet notamment de dessiner des arcs, disques, etc.
        //(Là où Gizmos s'arrête aux WireSphere & WireBox)
        Handles.color = Color.yellow;
        Handles.DrawWireDisc(transform.position, Vector3.up, radius);
    }
}
