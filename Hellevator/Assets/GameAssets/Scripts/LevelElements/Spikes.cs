using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour
{
	// On trigger enter kill the character that collided. 
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.GetComponentInParent<DemonBase>() != null)
		{
			DemonBase cmpDemon = collision.GetComponentInParent<DemonBase>();
			cmpDemon.IsInDanger = true;

			

			if (cmpDemon.IsControlledByPlayer)
			{
				PosesionManager.Instance.PossessNearestDemon(100, cmpDemon);
			}
			else
			{
				//Create the method for enemy death
				print("Create the method for enemy death");
				//cmpDemon.Die();
			}
		}
	}
	private void OnTriggerStay2D(Collider2D collision)
	{
		if (collision.GetComponentInParent<DemonBase>() != null)
		{
			DemonBase cmpDemon = collision.GetComponentInParent<DemonBase>();

			cmpDemon.IsInDanger = true;
		}
	}
	private void OnTriggerExit2D(Collider2D collision)
	{
		if(collision.GetComponentInParent<DemonBase>() != null)
		{
			DemonBase cmpDemon = collision.GetComponentInParent<DemonBase>();

			cmpDemon.IsInDanger = false;
		}
	}
}
