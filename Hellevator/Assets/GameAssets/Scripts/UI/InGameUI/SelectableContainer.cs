using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class SelectableContainer : Selectable, IPointerExitHandler, IPointerEnterHandler
{
    protected List<Selectable> m_selectables;
    [SerializeField] protected int m_currentIndex;
    [SerializeField] protected Navigation m_navigation;
    [SerializeField] protected bool m_keepsIndex;
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

    public int CurrentIndex
    {
        get => m_currentIndex;
        set
        {
            if (value < 0)
            {
                value = 0;
            }
            if (value >= Selectables.Count)
            {
                value = Selectables.Count - 1;
            }
            m_currentIndex = value;
        }
    }
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
        if (Selectables[CurrentIndex] is SelectableContainer)
        {
            if (((SelectableContainer)Selectables[KeptIndex]).KeepsIndex)
            {
                //m_currentIndex = KeptIndex;
                //if (m_currentIndex < 0)
                //    m_currentIndex = 0;
                //if (m_currentIndex >= Selectables.Count)
                //    m_currentIndex = Selectables.Count - 1;

                (Selectables[CurrentIndex] as SelectableContainer).CurrentIndex = KeptIndex;
            }
        }
        if (CurrentIndex < 0)
            CurrentIndex = 0;
        if (CurrentIndex >= Selectables.Count)
            CurrentIndex = Selectables.Count - 1;

        //transform.GetChild(m_currentIndex).GetComponent<Selectable>().OnSelected();
        Selectables[CurrentIndex].OnSelected();
    }

    public override void OnDeselected()
    {
        //for (int i = 0; i < transform.childCount; i++)
        //{
        //    transform.GetChild(i).GetComponent<Selectable>().OnDeselected();
        //}
        Selectables[CurrentIndex].OnDeselected();
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
            CurrentIndex--;
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
            CurrentIndex++;
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
            CurrentIndex--;
            OnSelected();
        }
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
    }
}
