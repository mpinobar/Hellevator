using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTriggerCleanBodies : MonoBehaviour
{
    [SerializeField]List<GameObject> m_objectsToDestroy = new List<GameObject>(0);

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<DemonBase>() != null)
        {
            if(collision.GetComponent<DemonBase>() == PossessionManager.Instance.ControlledDemon)
            {
                for (int i = 0; i < m_objectsToDestroy.Count; i++)
                {
                    if(m_objectsToDestroy[i] != null && m_objectsToDestroy[i].activeSelf)
                    {
                        if (PossessionManager.Instance.ControlledDemon != m_objectsToDestroy[i].GetComponent<DemonBase>())
                        {
                            m_objectsToDestroy[i].SetActive(false);
                        }
                    }
                }

                this.gameObject.SetActive(false);
            }
        }
    }

}
