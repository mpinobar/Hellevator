using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConversationDeactivator : MonoBehaviour
{
    private void Start()
    {
        if(PlayerPrefs.GetInt("EndConversationSatan") == 1)
        {
            transform.parent.gameObject.SetActive(false);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out DemonBase character))
        {
            if (character.IsControlledByPlayer)
                PlayerPrefs.SetInt("EndConversationSatan", 1);
        }
    }
}
