using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitmarkerEvents : MonoBehaviour
{
    private AudioSource AS;

    private void Awake()
    {
        AS = GetComponent<AudioSource>();
    }
    public void PlayHitSound()
    {
        AS.Play();
    }
}
