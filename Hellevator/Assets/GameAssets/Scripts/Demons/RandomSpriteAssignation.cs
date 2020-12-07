using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpriteAssignation : MonoBehaviour
{
    bool m_init;

    [SerializeField] Sprite [] m_availableSprites;
    int m_maskIndex = 0;

    public int MaskIndex { get => m_maskIndex; set => m_maskIndex = value; }

    // Start is called before the first frame update
    void Start()
    {
        if (!m_init)
        {
            AssignRandomSprite();
            m_init = true;
        }
    }

    private void AssignRandomSprite()
    {
        if (m_availableSprites.Length > 0)
        {
            m_maskIndex = Random.Range(0, m_availableSprites.Length);
            GetComponent<SpriteRenderer>().sprite = m_availableSprites[m_maskIndex];
        }

    }

    public void AssignMaskByIndex(int index)
    {
        //Debug.LogError("Index from levelManager: " + index);
        GetComponent<SpriteRenderer>().sprite = m_availableSprites[index];
        m_init = true;
    }

}
