using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCar : MonoBehaviour
{
    [SerializeField, Tooltip("The number of previous forward directions to save and average")]
    private float queueLength;
    private Queue<Vector3> prevForwards = new Queue<Vector3>();
    private Vector3 prevPosition;
    private void Start() {
        prevPosition = transform.position;
    }
    private void FixedUpdate()
    {
        prevForwards.Enqueue(transform.position - prevPosition != Vector3.zero ? (transform.position - prevPosition).normalized : transform.forward);
        prevPosition = transform.position;
        if(prevForwards.Count <= queueLength) {
            return;
        }
        prevForwards.Dequeue();
        Vector3 average = Vector3.zero;
        foreach(Vector3 forward in prevForwards) {
            average += forward;
        }
        average = average / prevForwards.Count;
        transform.forward = average != Vector3.zero ? average : transform.forward;
    }
}
