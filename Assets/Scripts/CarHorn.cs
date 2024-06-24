using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarHorn : MonoBehaviour
{
    private AudioSource AS;
    public AudioClip carHorn;
    private void Awake()
    {
        AS = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Enemy"))
        {
            AS.clip = carHorn;
            AS.Play();
        }
    }
}
