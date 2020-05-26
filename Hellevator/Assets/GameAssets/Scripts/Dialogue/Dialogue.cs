using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue 
{
	[SerializeField] private Sprite m_currentIcon = null;
    [TextArea(3, 10)]
    [SerializeField] private string[] sentences;

    [SerializeField] private DialogEvent[] evento;

	private TriggerEvent m_triggerer = null;

    public string[] Sentences { get => sentences;}
    public DialogEvent[] Evento { get => evento;}
	public TriggerEvent Triggerer { get => m_triggerer; set => m_triggerer = value; }
	public Sprite CurrentIcon { get => m_currentIcon; set => m_currentIcon = value; }
}
