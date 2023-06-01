using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarFollow : MonoBehaviour, CarMode
{
    [SerializeField, Tooltip("The car to follow")]
    private Car follow;
    [SerializeField, Tooltip("The reaction time of the driver, in seconds")]
    private float reactionTime;
    [SerializeField, Tooltip("The acceleration of the car")]
    private float acceleration;
    private Queue<float> followSpeeds = new Queue<float>();
    private float crntActiveTime = 0;
    (float, Vector3) CarMode.FixedUpdate(float crntSpeed, Vector3 crntDirection)
    {
        followSpeeds.Enqueue(follow.crntSpeed);
        if(crntActiveTime != reactionTime) {
            crntActiveTime = Mathf.MoveTowards(crntActiveTime, reactionTime, Time.fixedDeltaTime);   
        }
        else {
            followSpeeds.Dequeue();
        }
        float numSpeeds = 0;
        float averageSpeed = 0;
        foreach (float speed in followSpeeds) {
            numSpeeds++;
            averageSpeed += speed;
        }
        averageSpeed = averageSpeed / numSpeeds;
        crntSpeed = Mathf.MoveTowards(crntSpeed, averageSpeed, acceleration * Time.fixedDeltaTime);
        return(crntSpeed, Vector3.zero);
    }
    void CarMode.Activate() {
        follow = GetComponent<Car>().next;
    }
    void CarMode.Deactivate() {
        followSpeeds.Clear();
        crntActiveTime = 0;
    }
}
