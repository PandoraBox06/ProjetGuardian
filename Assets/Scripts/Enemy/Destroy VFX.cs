using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyVFX : MonoBehaviour
{
    IEnumerator DestroyExplosion()
    {

        yield return new WaitForSeconds(1f);

        Destroy(gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        StartCoroutine(DestroyExplosion());
    }
}
