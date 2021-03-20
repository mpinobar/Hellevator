using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathWithDelay : MonoBehaviour
{
    //public bool debug;
    List<SpikesWeightData> m_spikesData;
    [SerializeField] float m_killDelay = 1f;
    [SerializeField] AudioClip m_audioClip;
    Animator m_animator;
    [SerializeField] Transform m_attachedTransform;
    [SerializeField] float m_delayToAttach;
    //int m_dataCount;

    private void Awake()
    {
        m_animator = GetComponentInChildren<Animator>();
        m_spikesData = new List<SpikesWeightData>();
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
                    if (!m_spikesData[i].Colliders.Contains(collision))
                    {
                        m_spikesData[i].Colliders.Add(collision);
                    }
                }
            }
            if (!isCounted)
            {
                //Debug.LogError(cmpDemon.name + " entered spike " + name);

                m_spikesData.Add(new SpikesWeightData(cmpDemon, collision));
                cmpDemon.IsInDanger = true;
                cmpDemon.CanMove = false;
                StartCoroutine(KillWithDelay(cmpDemon, m_killDelay));

            }

        }
    }

    private IEnumerator KillWithDelay(DemonBase character, float delay)
    {
        //Animate here
        if (m_animator)
        {
            m_animator.SetTrigger("Eat");
        }
        yield return new WaitForSeconds(m_delayToAttach);
        if (m_audioClip)
            AudioManager.Instance.PlayAudioSFX(m_audioClip, false);
        float t = 0;
        while(t < delay)
        {
            character.Torso.transform.position = Vector3.Lerp(character.Torso.transform.position, m_attachedTransform.position, 3 * Time.deltaTime);
            t += Time.deltaTime;

            yield return null;
        }
        character.Die(true);
        Destroy(character.gameObject);
    }

    private void LateUpdate()
    {
        //if (debug)
        //{
        //    Debug.LogError(m_spikesData.Count);
        //}
        for (int i = 0; i < m_spikesData.Count; i++)
        {
            m_spikesData[i].AssociatedDemon.IsInDanger = true;
        }
        //m_dataCount = m_spikesData.Count;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponentInParent<DemonBase>() != null /*&& collision.gameObject.tag != "BodyCollider"*/)
        {
            DemonBase cmpDemon = collision.GetComponentInParent<DemonBase>();

            for (int i = 0; i < m_spikesData.Count; i++)
            {
                //if the demon is already inside the spikes
                if (cmpDemon == m_spikesData[i].AssociatedDemon)
                {
                    m_spikesData.RemoveAt(i);
                    cmpDemon.IsInDanger = false;
                    //Debug.LogError(cmpDemon.name + " exited spike " + name);
                    //remove the collider from the associated demon's collider list 
                    //if (m_spikesData[i].Colliders.Contains(collision))
                    //{
                    //    m_spikesData[i].Colliders.Remove(collision);

                    //    //all the limbs have exited the spikes
                    //    if (m_spikesData[i].Colliders.Count == 0)
                    //    {
                    //        m_spikesData.RemoveAt(i);
                    //        cmpDemon.IsInDanger = false;
                    //    }
                    //    else if (m_spikesData[i].Colliders.Count == 1 && m_spikesData[i].Colliders[0].tag == "BodyCollider")
                    //    {
                    //        m_spikesData.RemoveAt(i);
                    //        cmpDemon.IsInDanger = false;
                    //    }
                    //}
                }
            }
        }
    }
}
