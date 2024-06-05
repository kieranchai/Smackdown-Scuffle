using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagTeamer : MonoBehaviour
{
    private Rigidbody rb;

    private void Start()
    {
        GetComponent<Animator>().Play("Slam");
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);
    }
}
