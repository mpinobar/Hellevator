using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpriteAssignation : MonoBehaviour
{
    bool m_init;

    [SerializeField] Sprite [] m_availableSprites;

    // Start is called before the first frame update
    void OnEnable()
    {
        if (!m_init)
        {
            AssignRandomSprite();
            m_init = true;
        }
    }

    private void AssignRandomSprite()
    {
        if(m_availableSprites.Length > 0)
        GetComponent<SpriteRenderer>().sprite = m_availableSprites[Random.Range(0, m_availableSprites.Length)];
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            AssignRandomSprite();
        }
    }
}
