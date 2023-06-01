using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    [SerializeField, Tooltip("The delay after spawning a car before spawning another")]
    private float spawnDelay;
    [SerializeField, Tooltip("The prefab car to spawn")]
    private List<GameObject> carPrefabs;
    [SerializeField, Tooltip("Whether or not this spawner spawns cars on the left lane")]
    private bool onLeftLane;
    [SerializeField, Tooltip("A Transform that lies at the center of the left lane")]
    private Transform leftLane;
    [SerializeField, Tooltip("A Transform that lies at the center of the right lane")]
    private Transform rightLane;
    private float crntTime = 0;
    private Car lastSpawned = null;
    private void FixedUpdate()
    {
        if(crntTime == spawnDelay) {
            crntTime = 0;
            Car newCar = GameObject.Instantiate(carPrefabs[Random.Range(0, carPrefabs.Count)], transform.position, carPrefabs[0].transform.rotation).GetComponent<Car>();
            newCar.prev = null;
            if(lastSpawned) {
                lastSpawned.prev = newCar;
                newCar.next = lastSpawned;
            }
            lastSpawned = newCar;
            newCar.onLeftLane = onLeftLane;

            CarMerge merge = newCar.transform.GetComponent<CarMerge>();
            merge.leftLane = leftLane;
            merge.rightLane = rightLane;
        }
        crntTime = Mathf.MoveTowards(crntTime, spawnDelay, Time.fixedDeltaTime);
    }

}
