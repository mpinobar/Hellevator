using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class DialogueManager : TemporalSingleton<DialogueManager>
{
    //[SerializeField] private TextMeshProUGUI npcNameTxt;
    [SerializeField] private TextMeshProUGUI dialogueTxt;
    [SerializeField] private GameObject m_playerPortrait;
    [SerializeField] private GameObject m_boxImage;

	[SerializeField] private GameObject m_pressXToTalk = null;
	//[SerializeField] private GameObject cuadroDialogo;

    private Queue<string> sentences;

    private Dialogue currentDialogue;
	private GameObject currentImage;

	public GameObject PressXToTalk { get => m_pressXToTalk; set => m_pressXToTalk = value; }

	public override void Awake()
    {
		base.Awake();
        sentences = new Queue<string>();
    }
	private void Start()
	{
		//InputManager.Instance.InteractPressed += NextSentence;
	}

	public void StartTalking(Dialogue dialogue)
    {
        currentDialogue = dialogue;

		dialogueTxt.gameObject.SetActive(true);
		m_playerPortrait.SetActive(true);
		m_boxImage.SetActive(true);

		sentences.Clear();

        foreach (string sentence in currentDialogue.Sentences)
        {
            sentences.Enqueue(sentence);
        }
		NextSentence();
    }
    public void NextSentence()
    {
        if(sentences.Count == 0)
		{
			EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(WriteSentence(sentence));
    }
    IEnumerator WriteSentence(string sentence)
    {
        dialogueTxt.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueTxt.text = dialogueTxt.text + letter;
            yield return null;
        }
    }
    public void EndDialogue()
    {
		if (currentDialogue != null)
		{
			if(currentDialogue.Evento != null)
			{
				for (int i = 0; i < currentDialogue.Evento.Length; i++)
				{
					currentDialogue.Evento[i].ActivateEvent();
				}
			}			
		}		
    }
	public void DeactivateTextAndImage()
	{
		dialogueTxt.gameObject.SetActive(false);
		m_playerPortrait.SetActive(false);
		m_boxImage.SetActive(false);
		currentDialogue.Triggerer.DestroyTrigger();
	}
}
