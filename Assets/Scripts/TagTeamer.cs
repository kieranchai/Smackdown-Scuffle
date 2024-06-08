using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagTeamer : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField]
    private ParticleSystem poofParticles;

    [HideInInspector]
    public int damage;

    private void Start()
    {
        GetComponent<Animator>().Play("Slam");
        rb = GetComponent<Rigidbody>();
    }

    public void Landed()
    {
        poofParticles.Play();
        rb.isKinematic = true;
        GetComponent<Collider>().enabled = false;
        Destroy(transform.GetChild(0).gameObject);

        Invoke(nameof(Disappear), 2f);
    }

    public void Disappear()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Landed();
        if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<EnemyController>().GetSlammed(damage);
            // Play dazed particles on Enemy
        }
    }
}
