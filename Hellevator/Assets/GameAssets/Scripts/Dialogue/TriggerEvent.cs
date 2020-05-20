using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEvent : MonoBehaviour
{
	
	protected bool isInTrigger = false;

	protected void Start()
	{
		InputManager.Instance.OnInteract += Event;
	}

	protected void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.transform.root.GetComponent<DemonBase>() == PosesionManager.Instance.ControlledDemon)
		{
			isInTrigger = true;
			InputManager.Instance.IsInInteactionTrigger = true;
			DialogueManager.Instance.PressXToTalk.SetActive(true);
		}
	}
	protected void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.transform.root.GetComponent<DemonBase>() == PosesionManager.Instance.ControlledDemon)
		{
			InputManager.Instance.IsInInteactionTrigger = false;
			DialogueManager.Instance.PressXToTalk.SetActive(false);
			isInTrigger = false;
		}
	}

	protected virtual void Event()
	{

	}
	public virtual void DestroyTrigger()
	{
		
	}
}
