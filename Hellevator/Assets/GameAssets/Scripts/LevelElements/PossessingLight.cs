using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PossessingLight : MonoBehaviour
{

    bool m_travelling;
    DemonBase target;
    [SerializeField] float speed = 3.5f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (m_travelling)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.Torso.position, speed * Time.deltaTime);
        }
        
    }

    public void Begin(DemonBase d)
    {
        target = d;
        m_travelling = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.transform.root.GetComponent<DemonBase>() == target)
        {
            PosesionManager.Instance.PossessNewDemon(target);
        }
    }

}
