using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalTransport : MonoBehaviour
{
    [SerializeField] private float m_speed = 2f;
    [SerializeField] private TransportDirection direction = TransportDirection.Right;
    public List<DemonBase> m_enemiesOnPreassurePlate;
    [SerializeField] private LayerMask m_enemyLayerMask;
    List<SpikesWeightData> m_spikesData;
    List<HorizontalTransportData> m_charactersRgbs;
    private float dir;
    private List<Transform> m_bonesTransforms;
    private List<Vector3> m_bonesPositions;
    private List<Quaternion> m_bonesRotations;
    private void Awake()
    {
        m_enemiesOnPreassurePlate = new List<DemonBase>();
        m_spikesData = new List<SpikesWeightData>();
        m_charactersRgbs = new List<HorizontalTransportData>();
        dir = (int)direction * 2 - 1;
        m_bonesTransforms = new List<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < m_enemiesOnPreassurePlate.Count; i++)
        {
            if (m_enemiesOnPreassurePlate[i] != null)
            {
                m_enemiesOnPreassurePlate[i].DragMovement(dir * m_speed);
            }
            else
            {
                m_enemiesOnPreassurePlate.TrimExcess();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        DemonBase cmpDemon = collision.transform.GetComponentInParent<DemonBase>();

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
                    if (!m_spikesData[i].Colliders.Contains(collision.collider) && collision.gameObject.tag != "BodyCollider")
                    {
                        m_spikesData[i].Colliders.Add(collision.collider);

                    }
                }
            }
            if (!isCounted)
            {
                m_spikesData.Add(new SpikesWeightData(cmpDemon, collision.collider));
                m_charactersRgbs.Add(new HorizontalTransportData(cmpDemon.Torso.GetComponent<Rigidbody2D>(), false));
                cmpDemon.DragMovement(m_speed * dir);
                m_enemiesOnPreassurePlate.Add(cmpDemon);


            }

        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        DemonBase cmpDemon = collision.transform.GetComponentInParent<DemonBase>();
        if (cmpDemon != null && collision.gameObject.tag != "BodyCollider")
        {

            for (int i = 0; i < m_spikesData.Count; i++)
            {
                //if the demon is already inside the spikes
                if (cmpDemon == m_spikesData[i].AssociatedDemon)
                {
                    //remove the collider from the associated demon's collider list 
                    if (m_spikesData[i].Colliders.Contains(collision.collider))
                    {
                        m_spikesData[i].Colliders.Remove(collision.collider);

                        //all the limbs have exited the spikes
                        if (m_spikesData[i].Colliders.Count == 0)
                        {
                            StopAllCoroutines();
                            cmpDemon.DragMovement(0);
                            StartCoroutine(PushedBodyInertia(cmpDemon));
                            m_spikesData.RemoveAt(i);
                            m_enemiesOnPreassurePlate.Remove(cmpDemon);
                        }
                        else if (m_spikesData[i].Colliders.Count == 1 && m_spikesData[i].Colliders[0].tag == "BodyCollider")
                        {
                            StopAllCoroutines();
                            cmpDemon.DragMovement(0);
                            StartCoroutine(PushedBodyInertia(cmpDemon));
                            m_enemiesOnPreassurePlate.Remove(cmpDemon);
                            m_spikesData.RemoveAt(i);
                        }
                    }
                }
            }
        }
    }

    IEnumerator PushedBodyInertia(DemonBase demon)
    {
        float maxTimeDrag = 0.25f;
        float time = maxTimeDrag;
        while (time > 0)
        {
            if (demon && demon.IsGrounded())
            {
                demon.DragMovement(m_speed * dir);
            }
            else
            {
                time = 0;
            }
            time -= Time.deltaTime;
            yield return null;
        }
    }
}
