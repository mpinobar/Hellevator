using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroScriptedCharacterMovement : MonoBehaviour
{
    [SerializeField] DemonBase initialCharacter;
    [SerializeField]RuntimeAnimatorController fallController;    

    bool hasHit = false;
    float xPosition;
    private void Start()
    {
        xPosition = initialCharacter.transform.position.x;
        initialCharacter.GetComponent<Animator>().runtimeAnimatorController = fallController;
        initialCharacter.GetComponent<Animator>().SetBool("falling", true);
    }

    private void LateUpdate()
    {
        if (!hasHit)
            initialCharacter.transform.position = new Vector3(xPosition, initialCharacter.transform.position.y, 0);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out DemonBase character) && character == initialCharacter)
        {
            initialCharacter.GetComponent<Animator>().SetBool("falling", false);
            Invoke(nameof(AllowCharacterMovement), 1f);
        }
    }

    private void AllowCharacterMovement()
    {        
        hasHit = true;
    }
}
