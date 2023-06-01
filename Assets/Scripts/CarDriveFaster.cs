using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarDriveFaster : MonoBehaviour, CarMode
{
    [SerializeField, Tooltip("The speed of the car when driving regularly. Measured in units per second")]
    private float drivingSpeed;
    private float targetSpeed;
    [SerializeField, Tooltip("The acceleration of the car, in units per second squared")]
    private float acceleration;
    
    void CarMode.Activate() {
        targetSpeed = drivingSpeed;
    }

    (float, Vector3) CarMode.FixedUpdate(float crntSpeed, Vector3 crntDirection) {
        crntSpeed = UpdateSpeed(crntSpeed);
        return (crntSpeed, Vector3.zero);
    }
    
    private float UpdateSpeed(float crntSpeed) {
        crntSpeed = Mathf.MoveTowards(crntSpeed, targetSpeed, acceleration * Time.fixedDeltaTime);
        return crntSpeed;
    }
}
