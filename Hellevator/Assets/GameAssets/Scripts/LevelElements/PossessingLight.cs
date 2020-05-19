using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PossessingLight : MonoBehaviour
{

    bool m_travelling;
    DemonBase m_target;
    DemonBase m_originDemon;
    float m_lastDemonPossessionRange;
    [SerializeField] float m_speed = 3.5f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (m_travelling)
        {
            if(m_target == null)
            {
                Debug.Log("DEMON DIED WHILE TRAVELING TO POSSESS IT, LOOKING FOR ANOTHER DEMON");
                m_target = PosesionManager.Instance.LookForNearestDemon(m_lastDemonPossessionRange, transform, m_originDemon);
                if(m_target == null)
                {
                    Debug.Log("DEMON DIED WHILE TRAVELING TO POSSESS IT AND COULDN'T FIND A NEW ONE TO POSSESS, RESTARTING LEVEL");
                    LevelManager.Instance.StartRestartingLevel();
                }
                else
                {
                    Debug.Log("DEMON DIED WHILE TRAVELING TO POSSESS IT. FOUND ANOTHER ONE");
                }
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, m_target.Torso.position, m_speed * Time.deltaTime);
            }
        }
        
    }

    public void Begin(DemonBase d, float lastDemonPossessionRange, DemonBase originDemon)
    {
        m_target = d;
        m_originDemon = originDemon;
        m_lastDemonPossessionRange = lastDemonPossessionRange;
        m_travelling = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.transform.root.GetComponent<DemonBase>() == m_target)
        {
            PosesionManager.Instance.PossessNewDemon(m_target);
        }
    }

}
