using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject objectToSpawn; // Le GameObject � instancier
    public int numberOfObjects = 10; // Nombre d'objets � instancier
    public KeyCode spawnKey = KeyCode.P; // Touche pour instancier les objets

    // Update est appel�e une fois par frame
    void Update()
    {
        // V�rifie si la touche sp�cifi�e est enfonc�e
        if (Input.GetKeyDown(spawnKey))
        {
            // Appelle la m�thode SpawnObjects pour instancier les objets
            SpawnObjects();
        }
    }

    // M�thode pour instancier les objets
    void SpawnObjects()
    {
       
            // Instancie le GameObject sp�cifi�
            Instantiate(objectToSpawn, transform.position, Quaternion.identity);
       
    }
}
