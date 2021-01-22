using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorLevelStart : MonoBehaviour
{
	bool puertasAbiertas = false;

	// Update is called once per frame
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.GetComponent<DemonBase>() != null)
		{
			if (collision.GetComponent<DemonBase>() == PossessionManager.Instance.ControlledDemon)
			{
				if (puertasAbiertas)
				{
					this.GetComponent<Animator>().SetTrigger("CloseDoor");
					puertasAbiertas = false;
				}
				else
				{
					this.GetComponent<Animator>().SetTrigger("OpenDoor");
					puertasAbiertas = true;
				}
				
			}
		}
	}
}
