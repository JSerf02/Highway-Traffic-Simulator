using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlindspotDetector : MonoBehaviour
{
    public List<Car> carsInBlindspot = new List<Car>();
    public int NumCarsInBlindspot() {
        return carsInBlindspot.Count;
    }
    private void OnTriggerEnter(Collider collider) {
        Car car = null;
        if(collider.gameObject.TryGetComponent<Car>(out car) && car.gameObject != gameObject) {
            carsInBlindspot.Add(car);
        }
    }
    private void OnTriggerExit(Collider collider) {
        Car car = null;
        if(collider.gameObject.TryGetComponent<Car>(out car) && car.gameObject != gameObject) {
            carsInBlindspot.Remove(car);
        }
    }
}
