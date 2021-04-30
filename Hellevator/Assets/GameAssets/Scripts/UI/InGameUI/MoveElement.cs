using System.Collections;
using UnityEngine;

public class MoveElement : MonoBehaviour
{
    [SerializeField] float offsetToMove = 50;
    [SerializeField] Transform elementToMove;
    [SerializeField] float m_transitionSpeed = 2.5f;

    AnimationOnEnable hotel;
    [SerializeField] bool movesHotel;

    Vector2 startPos;
    private void Awake()
    {
        if (movesHotel)
        {
            if (!hotel)
                hotel = CameraManager.Instance.hotel;
            elementToMove = hotel.transform;
        }
        startPos = elementToMove.position;
    }
    private void OnEnable()
    {
        if (movesHotel)
        {
            if (!hotel)
                hotel = CameraManager.Instance.hotel;
            hotel.enabled = false;
            elementToMove = hotel.transform;
        }
        Move();
    }
    private void OnDisable()
    {
        if (movesHotel)
            hotel.enabled = true;
    }
    public void Move()
    {
        if (hotel)
            hotel.Stop();
        StartCoroutine(MovementCoroutine());
    }
    IEnumerator MovementCoroutine()
    {        
        Vector2 endPosition = startPos + Vector2.right*offsetToMove;
        while (Vector2.Distance(elementToMove.position, endPosition) > 1f)
        {
            elementToMove.position = Vector2.Lerp(elementToMove.position, endPosition, Time.unscaledDeltaTime * m_transitionSpeed);
            yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime);
        }
        //gameObject.SetActive(false);
    }

}
