using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoverBolt : MonoBehaviour {

    public float speed;

    public void Start()
    {
        GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().transform.up * speed;
    }

}
