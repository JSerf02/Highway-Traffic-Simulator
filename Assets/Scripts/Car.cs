using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CarState {
    Drive,
    Follow,
    Break,
    DriveFaster,
    Merge,
    None
}
interface CarMode {
    void Update() {

    }
    (float speed, Vector3 direction) FixedUpdate(float crntSpeed, Vector3 crntDirection) {
        return (crntSpeed, Vector3.zero);
    }
    void Activate() {

    }
    void Deactivate() {

    }
}
[RequireComponent(typeof(CarDrive)), RequireComponent(typeof(CarFollow)), RequireComponent(typeof(CarBreak)), RequireComponent(typeof(CarDriveFaster)), RequireComponent(typeof(CarMerge))]
public class Car : MonoBehaviour
{
    [Header("Debugging Controls")]
    [SerializeField, Tooltip("Press this button to make the car break")]
    private bool triggerBreak;
    [Header("Car Properties")]
    [System.NonSerialized]
    public Car next = null;
    [System.NonSerialized]
    public Car prev = null;
    private Dictionary<CarState, CarMode> carModes = new Dictionary<CarState, CarMode>();
    [SerializeField, Tooltip("The current state of the car")]
    private CarState crntState = CarState.Drive;
    private CarState prevState = CarState.None;
    [SerializeField, Tooltip("The current speed of the car, in units per second")]
    public float crntSpeed;
    [SerializeField, Tooltip("The current direction the car is travelling (automatically normalized internally)")]
    public Vector3 drivingDirection = Vector3.forward;
    [SerializeField, Tooltip("The probability that the car will randomly break on a given frame. Should be between 0 and 1")]
    private float breakChance;
    [SerializeField, Tooltip("The probability that the car will randomly merge on a given frame. Should be between 0 and 1")]
    private float mergeChance;
    [SerializeField, Tooltip("The probability that the car will randomly honk on a given frame when the person in front is breaking. Should be between 0 and 1")]
    private float honkChance;
    [SerializeField, Tooltip("The probability that the car will randomly make an engine sound on a given frame. Should be between 0 and 1")]
    private float engineChance;
    [SerializeField, Tooltip("The distance this car want to be from the car in front of it")]
    private float followDistance;
    [SerializeField, Tooltip("The minimum distance between 2 cars")]
    private float minDistance;
    [SerializeField, Tooltip("Enable if the car begins on the left lane")]
    public bool onLeftLane;
    [SerializeField, Tooltip("The left blindspot detector")]
    public BlindspotDetector leftBlindspot;
    [SerializeField, Tooltip("The right blindspot detector")]
    public BlindspotDetector rightBlindspot;
    [SerializeField, Tooltip("The horn audio clip")]
    private AudioClip horn;
    [SerializeField, Tooltip("The engine audio clip")]
    private AudioClip engine;
    [System.NonSerialized]
    public bool inMergeZone = false;
    private Vector3 _direction;
    private Vector3 Direction {
        get {
            return _direction.normalized;
        }
        set {
            _direction = value;
        }
    }
    private bool mergeQueued = false;
    private void Start() {
        Direction = drivingDirection;

        carModes[CarState.Drive] = GetComponent<CarDrive>();
        carModes[CarState.Follow] = GetComponent<CarFollow>();
        carModes[CarState.Break] = GetComponent<CarBreak>();
        carModes[CarState.DriveFaster] = GetComponent<CarDriveFaster>();
        carModes[CarState.Merge] = GetComponent<CarMerge>();
    }
    private void FixedUpdate() {
        UpdateStates();

        var crntValues = carModes[crntState].FixedUpdate(crntSpeed, Direction);
        crntSpeed = crntValues.speed;
        Direction = crntValues.direction == Vector3.zero ? drivingDirection : crntValues.direction;
        UpdatePosition();

        if(next && next.crntState == CarState.Break && Random.value < honkChance) {
            AudioSource.PlayClipAtPoint(horn, transform.position);
        }
        if(Random.value < engineChance) {
            AudioSource.PlayClipAtPoint(engine, transform.position);
        }

        // Bandaid for weird glitch that makes cars drive on the grass like assholes
        if(transform.position.x > 1.5 || transform.position.x < -1.5) {
            transform.position = new Vector3(Mathf.Sign(transform.position.x), transform.position.y, transform.position.z);
            SetDefaultState();
        }
        if(next == this || prev == this) {
            Destroy();
        }

    }
    private void Update() {
        if(mergeQueued) {
            UpdateMerge();
        }
        carModes[crntState].Update();
    }
    private void UpdatePosition() {
        if(next == null) {
            transform.position += Direction * crntSpeed * Time.fixedDeltaTime;
            return;
        }
        Vector3 nextPosition = next.transform.position;
        nextPosition = transform.position + drivingDirection * Vector3.Dot(drivingDirection, next.transform.position - transform.position);
        transform.position = Vector3.MoveTowards(transform.position, nextPosition - minDistance * Direction, crntSpeed * Time.fixedDeltaTime);
    }
    private void UpdateStates() {
        if(triggerBreak || (crntState != CarState.Break && crntState != CarState.Merge && Random.value <= breakChance)) {
            triggerBreak = false;
            Break();
        }
        else if(next != null && prev != null && crntState != CarState.Merge && crntState != CarState.Break && inMergeZone && Random.value <= mergeChance && !GameManager.Instance.merging) {
            QueueMerge();
        }
        if(crntState != CarState.Break && crntState != CarState.Merge) {
            SetDefaultState();
        }
        if(crntState != prevState) {
            if(prevState != CarState.None) {
                carModes[prevState].Deactivate();
            }
            prevState = crntState;
            carModes[crntState].Activate();
        }
    }
    public void SetDefaultState() {
        if(next == null) {
            crntState = CarState.Drive;
            return;
        }
        else if((transform.position - next.transform.position).magnitude > followDistance) {
            crntState = CarState.DriveFaster;
            return;
        }
        crntState = CarState.Follow;
    }
    public void Destroy() {
        if(next) {
            next.prev = null;
        }
        if(prev) {
            prev.next = null;
        }
        GameObject.Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider collider) {
        if(collider.gameObject.layer != LayerMask.NameToLayer("Despawn")) {
            return;
        }
        Destroy();
    }
    private void Break() {
        crntState = CarState.Break;
        Debug.Log("Break!");
    }
    private void QueueMerge() {
        mergeQueued = true;
        GameManager.Instance.merging = true;
        Debug.Log("Merge Queued!" + transform.position);
    }
    private void UpdateMerge() {
        BlindspotDetector crntBlindspot = leftBlindspot;
        if(onLeftLane) {
            crntBlindspot = rightBlindspot;
        }
        if(crntBlindspot.NumCarsInBlindspot() == 0) {
            mergeQueued = false;
            crntState = CarState.Merge;
            Debug.Log("Merge Started!");
            return;
        }
        foreach(Car car in crntBlindspot.carsInBlindspot) {
            if(car.crntState != CarState.Break) {
                car.Break();
            }
        }
    }
}


