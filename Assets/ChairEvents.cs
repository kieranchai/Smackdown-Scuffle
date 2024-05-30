using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;

public class ChairEvents : MonoBehaviour
{
    [HideInInspector]
    public PlayerController player;

    public void MoveForward()
    {
        StartCoroutine(ForcedFwdMovement());
    }

    public System.Collections.IEnumerator ForcedFwdMovement()
    {
        float moveTime = 0f;
        while (moveTime < 0.2f)
        {
            player.CC.Move(player.transform.forward * 5f * Time.deltaTime);
            moveTime += Time.deltaTime;
            yield return null;
        }
    }
}
