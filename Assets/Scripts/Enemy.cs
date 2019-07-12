using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    // Use this for initialization
    public float speed = 10.0f;

    void Start()
    {
        // Set the velocity of the rigidbody
       GetComponent<Rigidbody>().velocity = transform.forward * speed;

        // Create a red indicator for this asteroid
        var indicator = IndicatorManager.instance.AddIndicator(gameObject, Color.magenta);

        // BEGIN 3d_asteroid_gamemanager
        // Track the distance from this object to the current space station
        // that's managed by the GameManager
        indicator.showDistanceTo = GameManager.instance.currentSpaceStation.transform;
        // END 3d_asteroid_gamemanager
    }
}
