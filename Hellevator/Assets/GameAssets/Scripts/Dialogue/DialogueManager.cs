using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class DialogueManager : TemporalSingleton<DialogueManager>
{
    //[SerializeField] private TextMeshProUGUI npcNameTxt;
    private TextMeshProUGUI m_dialogueTxt;
    private GameObject m_playerCanvas;
    //[SerializeField] private GameObject m_boxImage;
    //[SerializeField] private Image m_playerIcon;

    List<AudioClip> m_dialogueClipsToPlay;
    private Queue<string> sentences;

    private Dialogue currentDialogue;
    private GameObject currentImage;

    private bool conversationEnded = true;

    public override void Awake()
    {
        base.Awake();
        sentences = new Queue<string>();
    }
    private void Start()
    {
        InputManager.Instance.OnInteract += NextSentence;
    }

    public void StartTalking(Dialogue dialogue, GameObject canvasText)
    {
        conversationEnded = false;
        m_playerCanvas = canvasText;
        m_playerCanvas.SetActive(true);
        m_dialogueTxt = canvasText.GetComponentInChildren<TextMeshProUGUI>();


        InputManager.Instance.IsInDialogue = true;
        PossessionManager.Instance.ControlledDemon.StopMovement();

        currentDialogue = dialogue;

        sentences.Clear();

        foreach (string sentence in currentDialogue.Sentences)
        {
            sentences.Enqueue(sentence);
        }
        NextSentence();
    }

    bool coroutineActive = false;
    string sentence;
    int index = 0;

    public List<AudioClip> DialogueClipsToPlay { get => m_dialogueClipsToPlay; set { index = 0; m_dialogueClipsToPlay = value; } }

    public void NextSentence()
    {
        if (!conversationEnded)
        {
            if (coroutineActive)
            {
                StopAllCoroutines();
                m_dialogueTxt.text = sentence;
                coroutineActive = false;
            }
            else
            {
                if (sentences.Count == 0)
                {
                    conversationEnded = true;
                    EndDialogue();
                    return;
                }

                sentence = sentences.Dequeue();
                StopAllCoroutines();
                coroutineActive = true;
                StartCoroutine(WriteSentence(sentence));
            }
        }
    }


    IEnumerator WriteSentence(string sentence)
    {
        if (DialogueClipsToPlay != null && index < DialogueClipsToPlay.Count)
        {
            AudioManager.Instance.PlayDialogueSFX(DialogueClipsToPlay[index]);
            index++;
        }
        m_dialogueTxt.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            for (int i = 0; i < 4; i++)// CAMBIAR EL 4 POR OTRO NUMERO, ES EL NUM DE FRAMES QUE HAY ENTRE CADA CHAR
            {
                yield return null;
            }
            m_dialogueTxt.text = m_dialogueTxt.text + letter;
            yield return null;
        }

        coroutineActive = false;
    }
    public void EndDialogue()
    {
        if (currentDialogue != null)
        {
            if (currentDialogue.Evento != null)
            {
                for (int i = 0; i < currentDialogue.Evento.Length; i++)
                {
                    currentDialogue.Evento[i].ActivateEvent();
                }
            }
            InputManager.Instance.IsInDialogue = false;
        }
    }
    public void DeactivateTextAndImage()
    {
        m_playerCanvas.SetActive(false);
    }
}
