using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selectable : MonoBehaviour
{
    [SerializeField] protected SelectableContainer m_parentContainer;
    [SerializeField] protected GameObject m_selectedHighlight;

    public virtual void OnSelected()
    {
        if (m_selectedHighlight)
            m_selectedHighlight.SetActive(true);
        //else
        //    Debug.LogError("Selecting " + name);
        UIController.Instance.Selected = this;
    }

    public virtual void OnDeselected()
    {
        if (m_selectedHighlight)
            m_selectedHighlight.SetActive(false);
        //else
        //    Debug.LogError("Deselecting " + name);
    }

    public virtual void NavigateUp()
    {
        OnDeselected();        
        m_parentContainer.NavigateUp();
    }

    public virtual void NavigateDown()
    {
        OnDeselected();
        m_parentContainer.NavigateDown();
    }
    public virtual void NavigateLeft()
    {
        OnDeselected();
        m_parentContainer.NavigateLeft();
    }

    public virtual void NavigateRight()
    {
        OnDeselected();
        m_parentContainer.NavigateRight();
    }

}
