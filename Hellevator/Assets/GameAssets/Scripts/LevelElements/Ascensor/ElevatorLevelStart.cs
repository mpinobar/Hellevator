using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorLevelStart : MonoBehaviour
{
	[SerializeField] private Transform m_endPos;
	[SerializeField] private float m_movementSpeed;
	[SerializeField] private float m_delayDoor;
	[SerializeField] private float m_delayBeforeSceneChange;
	[SerializeField] private AudioClip m_changeSceneSFX;

	[SerializeField] private GameObject m_trigger;

	private bool moving = false;

	DemonBase m_demon = null;

	private void Start()
	{
		moving = false;	
		Satan.OnDeath += StartCutScene;	

	}

    // Update is called once per frame
    private void Update()
	{
        if (moving)
        {
			if (Vector2.Distance(this.transform.position, m_endPos.position) > 0)
			{
                if (PossessionManager.Instance.ControlledDemon != null)
                {
					PossessionManager.Instance.ControlledDemon.CanMove = false;
                }
				this.transform.position = Vector2.MoveTowards(this.transform.position, m_endPos.position, m_movementSpeed * Time.unscaledDeltaTime);
				if (Vector2.Distance(this.transform.position, m_endPos.position) == 0)
				{
					StartCoroutine(DelayDoorOpen());
				}
			}
		}
	}
	private void StartCutScene()
    {
		moving = true; 
		if (PossessionManager.Instance.ControlledDemon != null)
		{
			PossessionManager.Instance.ControlledDemon.CanMove = false;
		}
	}

	IEnumerator DelayDoorOpen()
    {
        this.GetComponent<Animator>().SetTrigger("OpenDoor");
		yield return new WaitForSecondsRealtime(m_delayDoor);
		PossessionManager.Instance.ControlledDemon.CanMove = true;
	}

	IEnumerator CloseDoorAndChangeScene()
    {
		yield return new WaitForSecondsRealtime(m_delayBeforeSceneChange);
		m_trigger.SetActive(true);
	}
    private void OnTriggerEnter2D(Collider2D collision)
    {
		m_demon = collision.GetComponent<DemonBase>();
		if(m_demon != null)
        {
            if (m_demon.IsControlledByPlayer)
            {
				PossessionManager.Instance.ControlledDemon.CanMove = false;
				this.GetComponent<Animator>().SetTrigger("CloseDoor");
				StartCoroutine(CloseDoorAndChangeScene());
			}
        }
	}
}
