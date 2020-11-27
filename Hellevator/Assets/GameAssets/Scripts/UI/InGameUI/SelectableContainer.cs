using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableContainer : Selectable
{
    List<Selectable> m_selectables;
    [SerializeField] int m_currentIndex;
    [SerializeField] Navigation m_navigation;
    [SerializeField] bool m_keepsIndex;
    int m_keptIndex = 0;
    public List<Selectable> Selectables
    {
        get
        {
            if (m_selectables == null)
            {
                m_selectables = new List<Selectable>();
                for (int i = 0; i < transform.childCount; i++)
                {
                    if (transform.GetChild(i).GetComponent<Selectable>())
                        m_selectables.Add(transform.GetChild(i).GetComponent<Selectable>());
                }
            }
            return m_selectables;
        }
        set => m_selectables = value;
    }

    public int CurrentIndex { get => m_currentIndex; set => m_currentIndex = value; }
    public int KeptIndex { get => m_keptIndex; set => m_keptIndex = value; }
    public bool KeepsIndex { get => m_keepsIndex; set => m_keepsIndex = value; }

    //[SerializeField] protected SelectableContainer m_parentContainer;
    //[SerializeField] bool m_isParent;

    public enum Navigation
    {
        Horizontal, Vertical
    }

    private void OnEnable()
    {
        if (Selectables == null)
        {
            Selectables = new List<Selectable>();
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).GetComponent<Selectable>())
                    Selectables.Add(transform.GetChild(i).GetComponent<Selectable>());
            }
        }

        if (m_parentContainer == null)
        {
            OnSelected();
        }
    }

    public override void OnSelected()
    {
        //SelectableContainer selected = (SelectableContainer)Selectables[KeptIndex];
        if (Selectables[KeptIndex] is SelectableContainer)
        {
            if (((SelectableContainer)Selectables[KeptIndex]).KeepsIndex)
            {
                //m_currentIndex = KeptIndex;
                //if (m_currentIndex < 0)
                //    m_currentIndex = 0;
                //if (m_currentIndex >= Selectables.Count)
                //    m_currentIndex = Selectables.Count - 1;

                (Selectables[m_currentIndex] as SelectableContainer).CurrentIndex = KeptIndex;
            }
        }
        if (m_currentIndex < 0)
            m_currentIndex = 0;
        if (m_currentIndex >= Selectables.Count)
            m_currentIndex = Selectables.Count - 1;

        //transform.GetChild(m_currentIndex).GetComponent<Selectable>().OnSelected();
        Selectables[m_currentIndex].OnSelected();
    }

    public override void OnDeselected()
    {
        //for (int i = 0; i < transform.childCount; i++)
        //{
        //    transform.GetChild(i).GetComponent<Selectable>().OnDeselected();
        //}
        Selectables[m_currentIndex].OnDeselected();
    }

    public override void NavigateLeft()
    {
        if (m_navigation == Navigation.Vertical && m_parentContainer != null)
        {
            OnDeselected();
            if (m_keepsIndex)
            {
                m_parentContainer.KeptIndex = CurrentIndex;
            }
            m_parentContainer.NavigateLeft();
        }
        else
        {
            m_currentIndex--;
            OnSelected();
        }
    }

    public override void NavigateRight()
    {
        if (m_navigation == Navigation.Vertical && m_parentContainer != null)
        {
            OnDeselected();
            if (m_keepsIndex)
            {
                m_parentContainer.KeptIndex = CurrentIndex;
            }
            m_parentContainer.NavigateRight();
        }
        else
        {
            m_currentIndex++;
            OnSelected();
        }
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
            m_currentIndex++;
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
            m_currentIndex--;
            OnSelected();
        }
    }
}
