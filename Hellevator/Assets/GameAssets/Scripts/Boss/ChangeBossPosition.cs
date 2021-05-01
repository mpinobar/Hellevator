using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeBossPosition : MonoBehaviour
{
    [SerializeField] Boss m_belcebu;
    [SerializeField] float newPosY;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out DemonBase demon) && demon.IsControlledByPlayer)
            m_belcebu.transform.position = new Vector3(m_belcebu.transform.position.y, newPosY, 0);

    }
}
