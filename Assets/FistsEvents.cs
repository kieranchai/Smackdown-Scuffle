using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FistsEvents : MonoBehaviour
{

    public GameObject countdownPopupPrefab;
    Transform chokeCountdown;

    private void Start()
    {
        chokeCountdown = GameObject.Find("Choke Countdown").transform;
    }
    public void SpawnCountdownPopup()
    {
        GameObject popup = Instantiate(countdownPopupPrefab, chokeCountdown);
        popup.GetComponent<CountdownPopupEvents>().counter = 1;
    }
}
