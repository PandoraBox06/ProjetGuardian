using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXManagerEnnemy : MonoBehaviour
{
    public ParticleSystem SlashEnnemy1;
    public ParticleSystem SlashEnnemy2;
    public ParticleSystem SlashEnnemy3;
    public ParticleSystem StunEnnemy;
    public ParticleSystem InstanceAttaque;

    void SlashEnemy()
    {
        SlashEnnemy1.Play();
        SlashEnnemy2.Play();
    }
    void InstanceAttaqueEnemy()
    {
        InstanceAttaque.Play();
    }
    void SlashEnemy2()
    {
        SlashEnnemy3.Play();
    }
    void Stun()
    {
        StunEnnemy.Play();
    }

}