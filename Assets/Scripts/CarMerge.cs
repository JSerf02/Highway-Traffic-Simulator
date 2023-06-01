using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMerge : MonoBehaviour, CarMode
{
    [SerializeField, Tooltip("The speed of the car in the vertical direction when merging, in units per second")]
    private float verticalSpeed;
    [SerializeField, Tooltip("The speed of the car in the horizontal direction when merging, in units per second")]
    private float horizontalSpeed;
    [SerializeField, Tooltip("A Transform aligned with the right side of the road")]
    public Transform rightLane;
    [SerializeField, Tooltip("A Transform aligned with the left side of the road")]
    public Transform leftLane;
    private bool mergingRight = false;
    private Car car;
    private Car originalNext = null;
    private Car originalPrev = null;
    private void Start() {
        car = GetComponent<Car>();
    }
    void CarMode.Activate(){
        originalNext = null;
        originalPrev = null;
        if(car.onLeftLane) {
            mergingRight = true;
        }
        else {
            mergingRight = false;
        }

        BlindspotDetector blindspot = car.leftBlindspot;
        if(mergingRight) {
            blindspot = car.rightBlindspot;
        }

        RaycastHit hit;
        if(!Physics.Raycast(blindspot.transform.position, car.drivingDirection, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Car"))) {
            car.SetDefaultState();
            return;
        }
        Car hitCar = hit.collider.transform.parent.GetComponent<Car>();
        originalNext = car.next;
        originalPrev = car.prev;
        car.next = hitCar;
        car.prev = hitCar.prev;
        hitCar.prev = car;
        if(car.prev) {
            car.prev.next = car;
        }

        car.onLeftLane = !car.onLeftLane;
    }
    (float, Vector3) CarMode.FixedUpdate(float crntSpeed, Vector3 crntDirection) {
        Vector3 mergeVelocity = verticalSpeed * car.drivingDirection;
        
        Vector3 horizontalDirection = Quaternion.AngleAxis(mergingRight ? -90 : 90, Vector3.up) * car.drivingDirection;
        Transform lane = mergingRight ? rightLane : leftLane;
        float horizontalDistance = ((lane.position - transform.position) - car.drivingDirection * Vector3.Dot(car.drivingDirection, lane.position - transform.position)).magnitude;
        float crntHorizontalSpeed = Mathf.Min(horizontalDistance / Time.fixedDeltaTime, horizontalSpeed);
        if(crntHorizontalSpeed != horizontalSpeed) {
            car.SetDefaultState();
        }
        mergeVelocity += crntHorizontalSpeed * horizontalDirection;
        return (mergeVelocity.magnitude, mergeVelocity.normalized);
    }
    void CarMode.Deactivate() {
        if(originalNext) {
            originalNext.prev = originalPrev;
        }
        if(originalPrev) {
            originalPrev.next = originalNext;
        }
        GameManager.Instance.merging = false;
    }
}
