using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimEvents : MonoBehaviour
{

    [SerializeField]
    private ParticleSystem dazedParticles;

    [SerializeField]
    private ParticleSystem landingSmallParticles;

    [SerializeField]
    private ParticleSystem landingLargeParticles;

    public void PlayDazedParticles()
    {
        dazedParticles.Play();
    }

    public void StopDazedParticles()
    {
        dazedParticles.Stop();
    }

    public void PlayLandingParticlesSmall()
    {
        landingSmallParticles.Play();
    }

    public void PlayLandingParticlesLarge()
    {
        landingLargeParticles.Play();
    }
}
