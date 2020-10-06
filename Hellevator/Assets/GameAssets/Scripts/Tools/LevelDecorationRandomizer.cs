using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LevelDecorationRandomizer : TemporalSingleton<LevelDecorationRandomizer>
{

    [SerializeField] private List<Sprite> m_topRight;
    [SerializeField] private List<Sprite> m_top;
    [SerializeField] private List<Sprite> m_topLeft;
    [SerializeField] private List<Sprite> m_left;
    [SerializeField] private List<Sprite> m_bottomLeft;
    [SerializeField] private List<Sprite> m_bottom;
    [SerializeField] private List<Sprite> m_bottomRight;
    [SerializeField] private List<Sprite> m_right;

    public Sprite GetSprite(LevelDecoration ld)
    {
        Sprite sp = null;
        int rnd = 0;
        switch (ld)
        {
            case LevelDecoration.TopRight:
                rnd = Random.Range(0, m_topRight.Count);
                sp = m_topRight[rnd];
                break;
            case LevelDecoration.Top:
                rnd = Random.Range(0, m_top.Count);
                sp = m_top[rnd];
                break;
            case LevelDecoration.TopLeft:
                rnd = Random.Range(0, m_topLeft.Count);
                sp = m_topLeft[rnd];
                break;
            case LevelDecoration.Left:
                rnd = Random.Range(0, m_left.Count);
                sp = m_left[rnd];
                break;
            case LevelDecoration.BottomLeft:
                rnd = Random.Range(0, m_bottomLeft.Count);
                sp = m_bottomLeft[rnd];
                break;
            case LevelDecoration.Bottom:
                rnd = Random.Range(0, m_bottom.Count);
                sp = m_bottom[rnd];
                break;
            case LevelDecoration.BottomRight:
                rnd = Random.Range(0, m_bottomRight.Count);
                sp = m_bottomRight[rnd];
                break;
            case LevelDecoration.Right:
                rnd = Random.Range(0, m_right.Count);
                sp = m_right[rnd];
                break;
            default:
                break;
        }
        
        return sp;
    }

    
}
