using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarbageEvents : MonoBehaviour
{
    [HideInInspector]
    public PlayerController player;

    public GameObject GarbageCanObject;
    public GameObject GarbageObject;

    public void ThrowGarbage()
    {
        GameObject garbage = Instantiate(GarbageObject, player.garbageSpawnLocation.position, GarbageObject.transform.rotation);
        garbage.GetComponent<Rigidbody>().AddForce(player.PlayerCamera.transform.forward * 20f, ForceMode.Impulse);
        garbage.GetComponent<GarbageBag>().player = player;
    }

    public void ThrowGarbageCan()
    {
        GameObject garbage = Instantiate(GarbageCanObject, player.garbageCanSpawnLocation.position, GarbageCanObject.transform.rotation);
        garbage.GetComponent<Rigidbody>().AddForce(player.PlayerCamera.transform.forward * 70f, ForceMode.Impulse);
        garbage.GetComponent<Trashbin>().player = player;
    }
}
