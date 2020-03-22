using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodInstantiate : MonoBehaviour
{
    [SerializeField] ParticleSystem bloodPS;


    public void InstantiateBlood()
    {
        bloodPS.Play();
    }
}
