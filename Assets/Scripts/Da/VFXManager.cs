using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXManager : MonoBehaviour
{
    public ParticleSystem SlashAnim1;
    public ParticleSystem SlashAnim2;
    public ParticleSystem SlashAnim3;
    public ParticleSystem SlashAnim4;
    public ParticleSystem HoldGood;
    void SpawnVFX()
    {
        SlashAnim1.Play();
    }
    void SpawnVFX2()
    {
        SlashAnim2.Play();
    }
    void SpawnVFX3()
    {
        SlashAnim3.Play();
    }
    void SpawnVFX4()
    {
        SlashAnim4.Play();
    }
    void HoldVFX()
    {
        HoldGood.Play();
    }
}