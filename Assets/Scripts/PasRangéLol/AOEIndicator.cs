using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class AOEIndicator : MonoBehaviour
{
    [SerializeField] private ParticleSystem aoeIndicator;
    [SerializeField] private KeyCode key;
    [SerializeField] private float timingBeforeAoe;
    private Coroutine aoeInstance;
    [SerializeField] private Vector3 cordinateAoe;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(key))
        {
            aoeInstance ??= StartCoroutine(AOE(cordinateAoe));
        }
    }

    public IEnumerator AOE(Vector3 cordinate)
    {
        //spawn object to cordinate
        var indicator = Instantiate(aoeIndicator, cordinate, Quaternion.identity);
        indicator.Stop();
        var main = indicator.main;
        main.startLifetimeMultiplier = timingBeforeAoe;
        indicator.Play();
        //show aoe zone
        yield return new WaitForSeconds(timingBeforeAoe);
        // do dmg in the zone
        Debug.Log("Deal Dmg in AOE");
        Destroy(indicator);
        aoeInstance = null;
        StopCoroutine(nameof(AOE));
    }
}
