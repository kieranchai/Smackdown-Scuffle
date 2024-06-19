using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagTeamer : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField]
    private GameObject comicExplosionVFX;

    [SerializeField]
    private GameObject explosionVFX;

    [HideInInspector]
    public int damage;

    [SerializeField]
    private ParticleSystem speedLinesVFX;

    private void Start()
    {
        GetComponent<Animator>().Play("Slam");
        rb = GetComponent<Rigidbody>();
    }

    public void Landed()
    {
        Instantiate(comicExplosionVFX, transform.position + new Vector3(0, 1f, 0), Quaternion.identity);
        Instantiate(explosionVFX, transform.position, Quaternion.identity);
        rb.isKinematic = true;
        GetComponent<Collider>().enabled = false;
        speedLinesVFX.Stop();
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
