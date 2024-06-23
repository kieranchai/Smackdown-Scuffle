using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class CountdownPopupEvents : MonoBehaviour
{
    public int counter;

    public Sprite countdown1;
    public Sprite countdown2;
    public Sprite countdown3;

    public GameObject countdownPrefab;

    private void Start()
    {
        switch (counter)
        {
            case 1:
                gameObject.transform.Find("Countdown_Text").GetComponent<Image>().sprite = countdown1;
                break;
            case 2:
                gameObject.transform.Find("Countdown_Text").GetComponent<Image>().sprite = countdown2;
                break;
            case 3:
                gameObject.transform.Find("Countdown_Text").GetComponent<Image>().sprite = countdown3;
                break;
        }
    }

    public void DeleteAndSpawnNextCount()
    {
        if (counter == 3)
        {
            Destroy(gameObject);
            return;
        }

        GameObject popup = Instantiate(countdownPrefab, GameObject.Find("Choke Countdown").transform);
        ++popup.GetComponent<CountdownPopupEvents>().counter;

        Destroy(gameObject);
    }
}
