using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuidedJumpPlatform : MonoBehaviour
{

    [SerializeField] AnimationCurve m_xMovement;
    [SerializeField] AnimationCurve m_yMovement;
    [SerializeField] Transform m_endPoint;
    [SerializeField] float m_transitionSpeed;
    Collider2D m_collider;
    [SerializeField] bool m_disappearsAfterOneSecond;

    private void Awake()
    {
        m_collider = GetComponent<Collider2D>();
        if (m_disappearsAfterOneSecond)
        {
            Destroy(gameObject, 2f);
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponentInParent<DemonBase>() != null && collision.GetComponentInParent<DemonBase>() == PossessionManager.Instance.ControlledDemon)
        {
            if (m_disappearsAfterOneSecond || PossessionManager.Instance.ControlledDemon.CanMove)
            {
                StopAllCoroutines();
                StartCoroutine(TransferToDestination(PossessionManager.Instance.ControlledDemon.transform));

                transform.GetChild(1).GetComponent<Animator>().SetTrigger("Active");
            }

        }
    }

    IEnumerator TransferToDestination(Transform characterToMove)
    {
        characterToMove.GetComponent<DemonBase>().CanMove = false;
        Rigidbody2D rgb = characterToMove.GetComponent<Rigidbody2D>();

        rgb.gravityScale = 0;
        rgb.isKinematic = true;
        rgb.velocity = Vector2.zero;
        if (m_disappearsAfterOneSecond)
        {
            yield return new WaitForSeconds(0.75f);
        }
        Vector3 posToMove = Vector3.zero;
        Vector3 initPos = characterToMove.position;
        float time = 0;
        m_collider.enabled = false;

        while (time < 1)
        {
            time += Time.deltaTime * m_transitionSpeed;
            posToMove.x = initPos.x + ((m_endPoint.position.x - initPos.x) * m_xMovement.Evaluate(time));
            posToMove.y = initPos.y + ((m_endPoint.position.y - initPos.y) * m_yMovement.Evaluate(time));
            characterToMove.position = posToMove;
            yield return null;
        }

        characterToMove.GetComponent<DemonBase>().CanMove = true;
        rgb.isKinematic = false;
        m_collider.enabled = true;
        if (m_disappearsAfterOneSecond)
        {
            Destroy(gameObject);
        }
    }
}
