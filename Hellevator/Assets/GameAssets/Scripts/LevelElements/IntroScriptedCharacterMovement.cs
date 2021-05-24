using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class IntroScriptedCharacterMovement : MonoBehaviour
{
    [SerializeField] DemonBase initialCharacter;
    [SerializeField]RuntimeAnimatorController fallController;
    [SerializeField] GameObject [] fondos;
    public UnityEvent triggerEvent;

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

        if (PossessionManager.Instance.ControlledDemon != initialCharacter)
        {
            initialCharacter = PossessionManager.Instance.ControlledDemon;
            
        }

        if (!hasHit)
        {
            initialCharacter.transform.position = new Vector3(xPosition, initialCharacter.transform.position.y, 0);
            for (int i = 0; i < fondos.Length; i++)
            {
                fondos[i].transform.position = new Vector3(fondos[i].transform.position.x, -298.1318f, 10);
            }
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out DemonBase character) && character == initialCharacter)
        {
            initialCharacter.GetComponent<Animator>().SetBool("falling", false);
            Invoke(nameof(AllowCharacterMovement), 1f);
            LevelManager.Instance.ClearSceneReference();
            
        }
    }

    private void AllowCharacterMovement()
    {
        triggerEvent?.Invoke();
        hasHit = true;
    }
}
