using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class SplineTest : MonoBehaviour
{
    [SerializeField]
    private float _maxSpeed = 0;
    
    public float MaxSpeed {
        get {
            return _maxSpeed;
        }
        set {
            _maxSpeed = value;
            animate.MaxSpeed = value;
        }
    }
    [SerializeField, Tooltip("The animate component")]
    public SplineAnimate animate;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(animate.MaxSpeed != MaxSpeed) {
            animate.MaxSpeed = MaxSpeed;
            Debug.Log(animate.MaxSpeed);
        }
    }
}
