using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private void Awake() 
    { 
        // If there is an instance, and it's not me, delete myself.
        
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        } 
    }
    [SerializeField, Tooltip("When enabled, a car is merging. Used to make sure only one car merges at a time to prevent weird issues")]
    public bool merging = false;
    [SerializeField, Tooltip("The max time the game should be fast forwarded at the beginning before reverting to regular speed")]
    private float fastForwardTime;
    [SerializeField, Tooltip("The time scale of the game when fast forwarded")]
    private float fastForwardScale;
    [SerializeField, Tooltip("The time scale of the game regularly")]
    private float regularScale = 1;
    private float timeSoFar = 0;
    private void Start() {
        Time.timeScale = fastForwardScale;
    }
    private void FixedUpdate() {
        
        if(Time.timeScale == regularScale) {
            return;
        }
        timeSoFar += Time.fixedDeltaTime;
        if(timeSoFar >= fastForwardTime) {
            Time.timeScale = regularScale;
        }
    }
}
