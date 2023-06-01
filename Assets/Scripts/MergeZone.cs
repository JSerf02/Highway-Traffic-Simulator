using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MergeZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider collider){
        Car car;
        if(!collider.transform.parent || !collider.transform.parent.TryGetComponent<Car>(out car)) {
            return;
        }
        car.inMergeZone = true;
    }
    private void OnTriggerExit(Collider collider){
        Car car;
        if(!collider.transform.parent || !collider.transform.parent.TryGetComponent<Car>(out car)) {
            return;
        }
        car.inMergeZone = false;
    }
}
