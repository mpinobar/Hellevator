using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuidedJumpPlatform : MonoBehaviour
{

    [SerializeField] AnimationCurve xMovement;
    [SerializeField] AnimationCurve yMovement;
    [SerializeField] Transform endPoint;
    [SerializeField] float transitionSpeed;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponentInParent<DemonBase>() != null && collision.GetComponentInParent<DemonBase>() == PossessionManager.Instance.ControlledDemon)
        {
            StartCoroutine(TransferToDestination(collision.transform.root));
            transform.GetChild(1).GetComponent<Animator>().SetTrigger("Active");
        }
    }

    IEnumerator TransferToDestination(Transform characterToMove)
    {
        float currentX = characterToMove.position.x;
        float currentY = characterToMove.position.y;
        characterToMove.GetComponent<DemonBase>().CanMove = false;
        Vector3 posToMove = Vector3.zero;
        float time = 0;
        while (Vector3.Distance(characterToMove.position,endPoint.position) > 0.2f)
        {
            time += Time.deltaTime * transitionSpeed;
            currentX = Mathf.LerpUnclamped(currentX, endPoint.position.x, xMovement.Evaluate(time));
            currentY = Mathf.LerpUnclamped(currentY, endPoint.position.y, yMovement.Evaluate(time));
            posToMove.x = currentX;
            posToMove.y = currentY;
            characterToMove.position = posToMove;
            yield return null;
        }
        characterToMove.position = endPoint.position;
        characterToMove.GetComponent<DemonBase>().CanMove = true;
        InputManager.Instance.ResetPlayerInput();
    }
}
