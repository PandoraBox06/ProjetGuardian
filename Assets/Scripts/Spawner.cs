using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject objectToSpawn; // Le GameObject à instancier
    public int numberOfObjects = 10; // Nombre d'objets à instancier
    public KeyCode spawnKey = KeyCode.P; // Touche pour instancier les objets

    // Update est appelée une fois par frame
    void Update()
    {
        // Vérifie si la touche spécifiée est enfoncée
        if (Input.GetKeyDown(spawnKey))
        {
            // Appelle la méthode SpawnObjects pour instancier les objets
            SpawnObjects();
        }
    }

    // Méthode pour instancier les objets
    void SpawnObjects()
    {
       
            // Instancie le GameObject spécifié
            Instantiate(objectToSpawn, transform.position, Quaternion.identity);
       
    }
}
