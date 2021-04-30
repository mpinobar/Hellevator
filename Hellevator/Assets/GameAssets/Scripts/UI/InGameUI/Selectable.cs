using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Selectable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] protected SelectableContainer m_parentContainer;
    [SerializeField] protected GameObject m_selectedHighlight;
    Button m_buttonCmp;

    private void Awake()
    {
        m_buttonCmp = GetComponent<Button>();
        
    }

    private void OnEnable()
    {
       
       m_buttonCmp.onClick.AddListener(PlayPressedSound);
    }

    public void PlayPressedSound()
    {
        UIController.Instance.PlayPressedSound();
    }

    public virtual void OnSelected()
    {
        if (m_selectedHighlight)
            m_selectedHighlight.SetActive(true);
        UIController.Instance.Selected = this;
    }
    public virtual void OnDeselected()
    {
        if (m_selectedHighlight)
            m_selectedHighlight.SetActive(false);
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
    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        UIController.Instance.PlaySwapSound();
        OnSelected();
    }
    public virtual void OnPointerExit(PointerEventData eventData)
    {
       // OnDeselected();
    }
    public void Press()
    {
        if (m_buttonCmp)
        {
            m_buttonCmp.Select();            
        }
    }
}
