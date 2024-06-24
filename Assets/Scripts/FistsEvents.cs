using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FistsEvents : MonoBehaviour
{
    public GameObject countdownPopupPrefab;
    Transform chokeCountdown;

    [HideInInspector]
    public PlayerController player;

    public AudioClip swingSFX;

    private void Awake()
    {
        player = FindAnyObjectByType<PlayerController>();
    }

    private void Start()
    {
        chokeCountdown = GameObject.Find("Choke Countdown").transform;
    }

    public void PlaySwing()
    {
        player.AS.PlayOneShot(swingSFX);
    }

    public void SpawnCountdownPopup()
    {
        GameObject popup = Instantiate(countdownPopupPrefab, chokeCountdown);
        popup.GetComponent<CountdownPopupEvents>().counter = 1;
    }
}
