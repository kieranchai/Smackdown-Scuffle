using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlwaysFacePlayer : MonoBehaviour
{
    void Update()
    {
        this.transform.forward = transform.position - Camera.main.transform.position;
    }
}
