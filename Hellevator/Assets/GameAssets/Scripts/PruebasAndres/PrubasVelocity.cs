using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrubasVelocity : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
		Vector3 velo = this.GetComponent<Rigidbody2D>().velocity;
		float gravity = (-9.8f) * velo.y;
		this.GetComponent<Rigidbody2D>().velocity = new Vector3( (-1* 500*Time.deltaTime), velo.y, 0);
    }
}
