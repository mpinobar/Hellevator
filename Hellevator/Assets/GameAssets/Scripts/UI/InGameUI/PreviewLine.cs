using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewLine : MonoBehaviour
{
    DemonBase currentCharacter;

    LineRenderer m_lr;

    [SerializeField] Transform m_spritePrefab;

    List<Transform> m_sprites;
    Vector3 [] positions;
    private IEnumerator Start()
    {
        m_sprites = new List<Transform>();
        yield return null;
        for (int i = 0; i < 60; i++)
        {
            m_sprites.Add(Instantiate(m_spritePrefab));
        }
    }

    bool reset;
    // Update is called once per frame
    void Update()
    {
        if (InputManager.Instance.ThrowingHead)
        {
            reset = false;
            //Get the references if they are outdated
            if (currentCharacter == null || currentCharacter != PossessionManager.Instance.ControlledDemon)
            {
                currentCharacter = PossessionManager.Instance.ControlledDemon;
                m_lr = currentCharacter.GetComponent<LineRenderer>();
            }
            //if showing not null preview
            if (m_lr.positionCount > 0)
            {
                //Create enough sprites to show preview if necessary
                if (m_lr.positionCount > m_sprites.Count)
                {
                    int difference = m_lr.positionCount - m_sprites.Count;
                    for (int i = 0; i < difference; i++)
                    {
                        m_sprites.Add(Instantiate(m_spritePrefab));
                    }
                }
                //Align sprites to preview
                positions = new Vector3[m_lr.positionCount];
                m_lr.GetPositions(positions);
                for (int i = 0; i < m_sprites.Count - 1; i++)
                {
                    if (i < positions.Length)
                    {
                        m_sprites[i].position = positions[i];
                        Vector2 direction = m_sprites[i+1].position - m_sprites[i].position;
                        m_sprites[i].up = direction.normalized;
                    }
                    else
                        m_sprites[i].position = -Vector3.forward * 500;
                }
                m_sprites[positions.Length - 1].up = m_sprites[positions.Length - 1].position - m_sprites[positions.Length - 2].position;
            }
        }
        else if (!reset)
        {
            reset = true;
            for (int i = 0; i < m_sprites.Count - 1; i++)
            {
                m_sprites[i].position = -Vector3.forward * 500;
            }
        }
    }
}
