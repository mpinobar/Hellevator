using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineKart : MonoBehaviour
{
	[SerializeField] private MinekartActivationPreassurePlate[] m_preassurePlates;
	[SerializeField] private Transform m_minekartBeginingPos = null;
	[SerializeField] private Transform m_minekartEndPos = null; 
	[SerializeField] private Transform m_minekart = null;
	private Rigidbody2D m_cmpRbMinekart = null;
		

	[SerializeField] private float m_weightNeeded = 0f;
	[SerializeField] private float m_speed = 0f;
	[SerializeField] private float m_stoppingDistance = 0f;


    //Sound variables
    [SerializeField] private AudioClip m_machineClip;
    private AudioSource m_audioSource;



    private float m_currentWeight = 0;
	private float m_disntanceToEndPos = 0f;
	private float m_percentage = 0f;

	private void Awake()
	{
        m_audioSource = GetComponent<AudioSource>();
        m_audioSource.clip = m_machineClip;
        m_audioSource.loop = true;
        m_audioSource.playOnAwake = false;
        m_audioSource.volume = MusicManager.Instance.SfxVolume;

        m_cmpRbMinekart = m_minekart.GetComponent<Rigidbody2D>();
	}

	// Start is called before the first frame update
	void Start()
    {
		m_minekart.position = m_minekartBeginingPos.position;
		m_disntanceToEndPos =  m_minekartEndPos.position.x - m_minekartBeginingPos.position.x;
    }

    // Update is called once per frame
    void LateUpdate()
    {
		m_currentWeight = CalculateWeightOnPreassurePlates();
		m_percentage = m_currentWeight / m_weightNeeded;

		if(m_percentage > 1)
		{
			m_percentage = 1;
		}

		Vector2 currentDestination = new Vector2(m_minekartBeginingPos.position.x + m_percentage * m_disntanceToEndPos, m_minekart.position.y);

		float speedModifier = 0f;

		if(currentDestination.x < m_minekart.position.x)
		{
            if (!m_audioSource.isPlaying)
            {
                m_audioSource.Play();
            }
			speedModifier = -1;
		}
		else if(currentDestination.x > m_minekart.position.x)
		{
            if (!m_audioSource.isPlaying)
            {
                m_audioSource.Play();
            }
            speedModifier = 1;
		}
		if (Vector2.Distance(m_minekart.position, currentDestination) <= m_stoppingDistance)
		{
            if (m_audioSource.isPlaying)
            {
                m_audioSource.Stop();
            }
            speedModifier = 0f;
		}

		m_cmpRbMinekart.velocity = new Vector2(m_speed * Time.deltaTime * speedModifier, 0);

		//m_minekart.position = Vector2.MoveTowards(m_minekart.position, new Vector2(m_minekartBeginingPos.position.x + m_percentage * m_disntanceToEndPos, m_minekart.position.y), m_speed * Time.deltaTime);

	}

	private float CalculateWeightOnPreassurePlates()
	{
		float number = 0;

		for (int i = 0; i < m_preassurePlates.Length; i++)
		{
			number = number + m_preassurePlates[i].CurrentWeight;
		}

		return number;
	}
}
