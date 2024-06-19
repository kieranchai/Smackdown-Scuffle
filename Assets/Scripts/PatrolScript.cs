using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolScript : MonoBehaviour
{
    public Transform[] wayPoints;
    public float carSpeed;
    public int currentPoint;

    private void Start()
    {
        currentPoint = 0;
    }

    private void Update()
    {
        if (transform.position != wayPoints[currentPoint].position)
        {
            transform.position = Vector3.MoveTowards(transform.position, wayPoints[currentPoint].position, carSpeed * Time.deltaTime);
            Vector3 direction = wayPoints[currentPoint].position - transform.position;
            if (direction != Vector3.zero)
            {
                Quaternion toRotation = Quaternion.LookRotation(direction, transform.up);
                transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, carSpeed * Time.deltaTime);
            }
        }
        else
        {
            currentPoint = (currentPoint + 1) % wayPoints.Length;
        }
    }
}
