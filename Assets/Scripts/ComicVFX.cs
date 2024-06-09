using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComicVFX : MonoBehaviour
{
    private float lifeTime;

    [SerializeField]
    private ParticleSystem _bg;

    [SerializeField]
    private ParticleSystem _textF;

    [SerializeField]
    private ParticleSystem _textB;

    [SerializeField]
    private Material text1;

    [SerializeField]
    private Material text2;

    [SerializeField]
    private Material bg1;

    [SerializeField]
    private Material bg2;

    private void Awake()
    {
        if (Random.Range(0, 2) == 1)
        {
            _textF.GetComponent<Renderer>().material = text2;
            _textB.GetComponent<Renderer>().material = text2;
        } else
        {
            _textF.GetComponent<Renderer>().material = text1;
            _textB.GetComponent<Renderer>().material = text1;
        }

        if (Random.Range(0, 2) == 1)
        {
            _bg.GetComponent<Renderer>().material = bg1;
        }
        else
        {
            _bg.GetComponent<Renderer>().material = bg2;
        }
    }

    void Update()
    {
        lifeTime += Time.deltaTime;
        if (lifeTime >= 1.0f)
        {
            Destroy(gameObject);
        }
    }
}
