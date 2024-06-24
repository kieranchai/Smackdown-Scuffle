using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarbageEvents : MonoBehaviour
{
    [HideInInspector]
    public PlayerController player;

    public GameObject GarbageCanObject;
    public GameObject GarbageObject;

    public AudioClip lidSlamSFX;
    public AudioClip wooshSFX;
    public AudioClip carrySFX;
    public AudioClip holsterSFX;
    public AudioClip lidOpenSFX;
    public AudioClip woosh2SFX;
    public AudioClip shakeSFX;

    private void Awake()
    {
        player = FindAnyObjectByType<PlayerController>();
    }

    public void PlayHolsterSFX()
    {
        player.AS.PlayOneShot(holsterSFX);
    }

    public void PlayLidOpenSFX()
    {
        player.AS.PlayOneShot(lidOpenSFX);
    }

    public void PlayLidSlam()
    {
        player.AS.PlayOneShot(lidSlamSFX);
    }

    public void PlayShake()
    {
        player.AS.PlayOneShot(shakeSFX);
    }

    public void ThrowGarbage()
    {
        player.AS.PlayOneShot(woosh2SFX);
        GameObject garbage = Instantiate(GarbageObject, player.garbageSpawnLocation.position, GarbageObject.transform.rotation);
        garbage.GetComponent<Rigidbody>().AddForce(player.PlayerCamera.transform.forward * 20f, ForceMode.Impulse);
        garbage.GetComponent<GarbageBag>().player = player;
    }

    public void DumpGarbage()
    {
        for (int i = 0; i <= 2; i++)
        {
            GameObject garbage = Instantiate(GarbageObject, player.multiGarbageSpawnLocation.position, GarbageObject.transform.rotation);
            garbage.GetComponent<Rigidbody>().AddForce(player.PlayerCamera.transform.forward * 20f, ForceMode.Impulse);
            garbage.GetComponent<GarbageBag>().player = player;
        }
    }

    public void SlowPlayer()
    {
        player.AS.PlayOneShot(carrySFX);
        player.MoveSpeed = 1.5f;
    }

    public void ResetPlayerSpeed()
    {
        player.MoveSpeed = 5f;
    }

    public void ThrowGarbageCan()
    {
        player.AS.PlayOneShot(wooshSFX);
        GameObject garbage = Instantiate(GarbageCanObject, player.garbageCanSpawnLocation.position, GarbageCanObject.transform.rotation);
        garbage.GetComponent<Rigidbody>().AddForce(player.PlayerCamera.transform.forward * 70f, ForceMode.Impulse);
        garbage.GetComponent<Trashbin>().player = player;
        ResetPlayerSpeed();
    }
}
