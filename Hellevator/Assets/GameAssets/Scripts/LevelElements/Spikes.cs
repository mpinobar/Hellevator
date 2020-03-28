using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour
{

    List<SpikesWeightData> m_spikesData;
    
    private void Awake()
    {
        m_spikesData = new List<SpikesWeightData>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            LogBodiesInside();
        }
    }

				
    private void LogBodiesInside()
    {
        print("Number of demons inside: " + m_spikesData.Count);
        for (int i = 0; i < m_spikesData.Count; i++)
        {
            print("Demon number " + i + " with name: " + m_spikesData[i].AssociatedDemon.name + " has " + m_spikesData[i].Colliders.Count + " limbs inside.");
            for (int j = 0; j < m_spikesData[i].Colliders.Count; j++)
            {
                print(m_spikesData[i].Colliders[j].name);
            }
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
                if (cmpDemon.IsControlledByPlayer)
                {
                    print("a");
                    m_spikesData.Add(new SpikesWeightData(cmpDemon, collision));
                    cmpDemon.IsInDanger = true;
                    cmpDemon.SetColor(Color.red);
                    cmpDemon.Die();
                    collision.GetComponentInParent<BloodInstantiate>().InstantiateBlood();
                }
                else
                {
                    if (collision.gameObject.tag != "BodyCollider")
                    {

                        m_spikesData.Add(new SpikesWeightData(cmpDemon, collision));
                        cmpDemon.IsInDanger = true;
                        cmpDemon.SetColor(Color.red);
                        //Create the method for enemy death
                        print("Create the method for enemy death");
                        cmpDemon.Die();
                    }


                }
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
                        if(m_spikesData[i].Colliders.Count == 0)
                        {
                            cmpDemon.IsInDanger = false;
                            cmpDemon.SetColor(Color.white);
                            m_spikesData.RemoveAt(i);
                        }
                        else if(m_spikesData[i].Colliders.Count == 1 && m_spikesData[i].Colliders[0].tag == "BodyCollider")
                        {
                            cmpDemon.SetColor(Color.white);
                            cmpDemon.IsInDanger = false;
                            m_spikesData.RemoveAt(i);
                        }
                    }
                }
            }            
        }
    }
    
}
