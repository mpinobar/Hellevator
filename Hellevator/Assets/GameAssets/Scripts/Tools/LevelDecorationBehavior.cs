using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(SpriteRenderer))]
public class LevelDecorationBehavior : MonoBehaviour
{
    [SerializeField] private LevelDecoration m_levelDecoration = LevelDecoration.Top;
    [SerializeField] private bool m_generateSprite;
      

    private void Awake()
    {
        GenerateLevelDecorationSprite();
    }

    private void GenerateLevelDecorationSprite()
    {
        SpriteRenderer spr = GetComponent<SpriteRenderer>();
        spr.sprite = LevelDecorationRandomizer.Instance.GetSprite(m_levelDecoration);
    }

    private void Update()
    {
        if (m_generateSprite)
        {
            GenerateLevelDecorationSprite();
            m_generateSprite = false;
        }
    }
}
