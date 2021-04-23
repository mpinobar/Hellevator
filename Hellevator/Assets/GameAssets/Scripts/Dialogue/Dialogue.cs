using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue 
{
    [TextArea(3, 10)]
    [SerializeField] private string[] sentences;

    [SerializeField] private DialogEvent[] evento;

	private TriggerDialogo m_triggerer = null;

    public string[] Sentences { get => sentences;}
    public DialogEvent[] Evento { get => evento;}
	public TriggerDialogo Triggerer { get => m_triggerer; set => m_triggerer = value; }
}
