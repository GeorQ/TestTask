using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateMovement : MonoBehaviour
{
    private Vector3 initialPos;
    private Vector3 destination;
    private float moveSpeed = 3f;
    private float distanceToChangePoint = 0.4f;


    private void Start()
    {
        initialPos = transform.position;
        SetRandomDestination();
    }

    private void FixedUpdate()
    {
        MoveGate();
    }

    private void SetRandomDestination()
    {
        destination = new Vector3(initialPos.x + Random.Range(-20, 20), initialPos.y, initialPos.z);
    }

    private void MoveGate()
    {
        Vector3 direction = (destination - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.fixedDeltaTime; //TFDT helps calculate velocity per second, rather than velocity per update
        if (Vector3.Distance(transform.position, destination) < distanceToChangePoint)
        {
            SetRandomDestination();
        }
    }
}