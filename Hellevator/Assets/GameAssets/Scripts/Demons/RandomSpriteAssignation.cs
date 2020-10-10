﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpriteAssignation : MonoBehaviour
{
    bool init;

    [SerializeField] Sprite [] m_availableSprites;

    // Start is called before the first frame update
    void OnEnable()
    {
        if (!init)
        {
            AssignRandomSprite();
            init = true;
        }
    }

    private void AssignRandomSprite()
    {
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