using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PruebasMovement : TemporalSingleton<PruebasMovement>
{
	
	[SerializeField] private float m_speed = 0f;
	[SerializeField] private float m_jumpStrength = 0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void MoveSquare(float x)
	{
		this.transform.position = new Vector3(this.transform.position.x + x * Time.deltaTime * m_speed, this.transform.position.y, this.transform.position.z);
	}

	public void Jump()
	{
		this.GetComponent<Rigidbody2D>().AddForce(transform.up * m_jumpStrength);
	}
}
