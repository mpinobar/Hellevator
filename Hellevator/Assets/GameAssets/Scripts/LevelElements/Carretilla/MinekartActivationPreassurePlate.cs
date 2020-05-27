using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinekartActivationPreassurePlate : MonoBehaviour
{
	//Weight and type variables
	[SerializeField] private float m_weightNeeded;
	[Tooltip("Check this if the pressure plate blocks possession of bodies on this pressure plate")]
	[SerializeField] private bool m_blocksPossession;
	private float m_currentWeight;
	private List<DemonBase> m_enemiesOnPreassurePlate;
	[SerializeField] private LayerMask m_enemyLayerMask;

	//Position Variables
	[SerializeField] private float m_speed;
	[SerializeField] private Transform m_pressurePlateEndPosition;
	[SerializeField] private Transform m_parent;
	private Vector3 m_startingPosition;
	private float m_distanceToEndPosition;
	
	List<SpikesWeightData> m_spikesData;


    private bool m_isActive = false;
	private float m_percentage = 0f;
	private float m_positionY = 0f;

	public bool IsActive { get => m_isActive; set => m_isActive = value; }
	public float CurrentWeight { get => m_currentWeight; set => m_currentWeight = value; }

	private void Awake()
	{
		m_enemiesOnPreassurePlate = new List<DemonBase>(0);
		m_currentWeight = 0;

        m_spikesData = new List<SpikesWeightData>();
		m_startingPosition = m_parent.position;
		m_distanceToEndPosition = m_parent.transform.position.y - m_pressurePlateEndPosition.position.y;
	}

    // Update is called once per frame
    void Update()
    {
		CalculateAccumulatedWeight();

		if (m_currentWeight >= m_weightNeeded)
		{
			m_currentWeight = m_weightNeeded;
		}

		if (!m_isActive)
		{
			m_percentage = m_currentWeight / m_weightNeeded;
			if(m_percentage > 1f)
			{
				m_percentage = 1f;
			}
			m_positionY = m_distanceToEndPosition * m_percentage;

            

			m_parent.transform.position = Vector3.MoveTowards(m_parent.transform.position, new Vector3(m_startingPosition.x, m_startingPosition.y - m_positionY, m_startingPosition.z), m_speed * Time.deltaTime);
		}
	}


	// On trigger enter kill the character that collided. 
	private void OnTriggerEnter2D(Collider2D collision)
	{
		DemonBase cmpDemon = collision.GetComponentInParent<DemonBase>();

		if (cmpDemon != null)
		{
			bool isCounted = false;

			for (int i = 0; i < m_spikesData.Count; i++)
			{
				//if the demon is already inside the spikes
				if (cmpDemon == m_spikesData[i].AssociatedDemon)
				{
					isCounted = true;

					//add the collider to the associated demon's collider list if it isnt already included
					if (!m_spikesData[i].Colliders.Contains(collision) && collision.gameObject.tag != "BodyCollider")
					{
						m_spikesData[i].Colliders.Add(collision);
					}
				}
			}
			if (!isCounted)
			{

				m_spikesData.Add(new SpikesWeightData(cmpDemon, collision));
				m_enemiesOnPreassurePlate.Add(cmpDemon);
				m_currentWeight += cmpDemon.Weight;

			}

		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.GetComponentInParent<DemonBase>() != null && collision.gameObject.tag != "BodyCollider")
		{
			DemonBase cmpDemon = collision.GetComponentInParent<DemonBase>();

			for (int i = 0; i < m_spikesData.Count; i++)
			{
				//if the demon is already inside the spikes
				if (cmpDemon == m_spikesData[i].AssociatedDemon)
				{
					//remove the collider from the associated demon's collider list 
					if (m_spikesData[i].Colliders.Contains(collision))
					{
						m_spikesData[i].Colliders.Remove(collision);

						//all the limbs have exited the spikes
						if (m_spikesData[i].Colliders.Count == 0)
						{
							m_currentWeight -= cmpDemon.Weight;
							m_spikesData.RemoveAt(i);
							m_enemiesOnPreassurePlate.Remove(cmpDemon);
							cmpDemon.IsPossessionBlocked = false;
						}
						else if (m_spikesData[i].Colliders.Count == 1 && m_spikesData[i].Colliders[0].tag == "BodyCollider")
						{
							m_enemiesOnPreassurePlate.Remove(cmpDemon);
							m_currentWeight -= cmpDemon.Weight;
							m_spikesData.RemoveAt(i);
							cmpDemon.IsPossessionBlocked = false;
						}
					}
				}
			}
		}
	}

	/// <summary>
	/// Calls recurring method to calculate accumulated weight of all stacked bodies
	/// </summary>
	private void CalculateAccumulatedWeight()
	{

		float totalWeight = 0;
		for (int i = 0; i < m_enemiesOnPreassurePlate.Count; i++)
		{
			totalWeight += CalculateWeightOnTop(m_enemiesOnPreassurePlate[i]);
		}
		m_currentWeight = totalWeight;
	}

	/// <summary>
	/// Recurring method that returns the weight of all bodies on top of the specified demon
	/// </summary>
	/// <param name="demon">The demon upon which the funcion will be called again</param>
	/// <returns>The sum of the weights on top</returns>
	private float CalculateWeightOnTop(DemonBase demon)
	{
		float totalWeight = demon.Weight;

		if (m_blocksPossession)
		{
			demon.IsPossessionBlocked = true;
		}

		List<DemonBase> demonsOnTop = new List<DemonBase>();
		float distanceToCast = 0.5f;

		//cast a circle upwards to see if there is a different body on top
		RaycastHit2D[] hits = Physics2D.CircleCastAll(demon.LimbsColliders[0].transform.position, 0.3f, Vector2.up, distanceToCast, m_enemyLayerMask);

		for (int j = 0; j < hits.Length; j++)
		{
			//check if the collider on top is a demon
			DemonBase demonOnTop = hits[j].transform.root.GetComponent<DemonBase>();
			if (demonOnTop != null)
			{
				//add it to a list of demons that are on top of this one, making sure not to add it twice
				if (demonOnTop != demon && !demonsOnTop.Contains(demonOnTop) && !m_enemiesOnPreassurePlate.Contains(demonOnTop))
				{
					demonsOnTop.Add(demonOnTop);

					//block possession of the demons on top if it should be done
					if (m_blocksPossession)
					{
						demonOnTop.IsPossessionBlocked = true;
					}
				}
			}
		}
		//recurring method for the demons on top of this one
		for (int i = 0; i < demonsOnTop.Count; i++)
		{
			totalWeight += CalculateWeightOnTop(demonsOnTop[i]);
		}
		return totalWeight;
	}
}
