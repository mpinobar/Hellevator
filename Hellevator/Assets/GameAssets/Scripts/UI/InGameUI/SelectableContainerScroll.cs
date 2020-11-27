using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableContainerScroll : SelectableContainer
{
    [SerializeField] float m_YToDisplace = 300;
    [SerializeField] float m_minY = 0;
    [SerializeField] float m_maxY = 725;
    RectTransform m_rectTransform;

    private void Awake()
    {
        m_rectTransform = GetComponent<RectTransform>();
        
    }

    public override void NavigateDown()
    {
        if (m_navigation == Navigation.Horizontal && m_parentContainer != null)
        {
            OnDeselected();
            if (m_keepsIndex)
            {
                m_parentContainer.KeptIndex = CurrentIndex;
            }
            m_parentContainer.NavigateDown();
        }
        else
        {
            float newY = Mathf.Clamp (Mathf.Max(m_YToDisplace*(CurrentIndex),0),m_minY,m_maxY);
            //Debug.LogError(newY);
            m_rectTransform.localPosition = new Vector3(m_rectTransform.localPosition.x, newY, m_rectTransform.localPosition.z);
            CurrentIndex++;
            OnSelected();
        }
    }
    public override void NavigateUp()
    {
        if (m_navigation == Navigation.Horizontal && m_parentContainer != null)
        {
            OnDeselected();
            if (m_keepsIndex)
            {
                m_parentContainer.KeptIndex = CurrentIndex;
            }
            m_parentContainer.NavigateUp();
        }
        else
        {
            float newY = Mathf.Clamp (Mathf.Max(m_YToDisplace*(CurrentIndex-2),0),m_minY,m_maxY);
            //Debug.LogError(newY);
            m_rectTransform.localPosition = new Vector3(m_rectTransform.localPosition.x, newY, m_rectTransform.localPosition.z);
            CurrentIndex--;
            OnSelected();
        }
    }
}
