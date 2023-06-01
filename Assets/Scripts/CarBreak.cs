using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarBreak : MonoBehaviour, CarMode
{
    [SerializeField, Tooltip("The target speed of the car after breaking. Measured in units per second. The car will slow down until reaching this speed, then speed up.")]
    private float breakSpeed;
    private float targetSpeed;
    [SerializeField, Tooltip("The decelleration of the car when breaking, in units per second squared")]
    private float decceleration;
    
    void CarMode.Activate() {
        targetSpeed = breakSpeed;
    }

    (float, Vector3) CarMode.FixedUpdate(float crntSpeed, Vector3 crntDirection) {
        crntSpeed = UpdateSpeed(crntSpeed);
        return (crntSpeed, Vector3.zero);
    }
    
    private float UpdateSpeed(float crntSpeed) {
        crntSpeed = Mathf.MoveTowards(crntSpeed, targetSpeed, decceleration * Time.fixedDeltaTime);
        if(crntSpeed == targetSpeed) {
            GetComponent<Car>().SetDefaultState();
        }
        return crntSpeed;
    }
}
