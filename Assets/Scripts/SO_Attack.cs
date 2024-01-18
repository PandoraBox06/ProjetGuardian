using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Attack")]
public class SO_Attack : ScriptableObject
{
    public string attackName;

    public AnimatorOverrideController animatorOV;

    public ParticleSystem particle;
}
