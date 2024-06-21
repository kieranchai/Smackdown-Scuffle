using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialProjectile : MonoBehaviour
{
    public GameObject ImpactVFX;
    public float MoveSpeed = 5f;

    [HideInInspector]
    public float LifeSpan = 3f;
    [HideInInspector]
    public int Damage;
    [HideInInspector]
    public AttackStrength Strength;

    // Start is called before the first frame update
    void Start()
    {
        transform.forward = Camera.main.transform.forward;
        Invoke("Explode", LifeSpan);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += (transform.forward * MoveSpeed) * Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.GetComponent<EnemyController>() != null)
        {
            collision.collider.GetComponent<EnemyController>().TakeDamage(Damage, Strength);
        }
        Explode();
    }
    private void Explode()
    {
        Instantiate(ImpactVFX, transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }
}
