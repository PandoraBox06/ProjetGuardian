using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXManager : MonoBehaviour
{
    public ParticleSystem SlashAnim1;
    public ParticleSystem SlashAnim2;
    public ParticleSystem SlashAnim3;
    public ParticleSystem SlashAnim4;
    void SpawnVFX()
    {
        SlashAnim1.Play();
        Debug.Log("OUI");
    }
    void SpawnVFX2()
    {
        SlashAnim2.Play();
        Debug.Log("OUI");
    }
    void SpawnVFX3()
    {
        SlashAnim3.Play();
        Debug.Log("OUI");
    }
    void SpawnVFX4()
    {
        SlashAnim4.Play();
        Debug.Log("OUI");
    }
}