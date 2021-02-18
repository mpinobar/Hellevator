using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Catapult : MonoBehaviour
{
    [SerializeField] float m_slowDownTime = 0.5f;
    [SerializeField] float m_throwVelocity = 5f;
    [SerializeField] Transform m_headTransform = null;
    DemonBase m_demon;

    private void Start()
    {
        m_demon = GetComponent<DemonBase>();
    }
    /// <summary>
    /// Empieza la corrutina de apuntado
    /// </summary>
    public void ShowAim()
    {
        m_demon.CanMove = false;
        StartCoroutine(SelectLandingSpot());
    }
    /// <summary>
    /// Primero ralentiza el tiempo y dice al input manager que se está apuntando
    /// </summary>
    /// <returns></returns>
    IEnumerator SelectLandingSpot()
    {
        yield return SlowDown();
        InputManager.Instance.ThrowingHead = true;

    }

    public void ThrowHead()
    {
        Time.timeScale = 1;
        InputManager.Instance.ThrowingHead = false;
        Vector2 direction = CameraManager.Instance.MainCamera.ScreenToWorldPoint(Input.mousePosition) - m_demon.Torso.position;
        m_headTransform.parent = null;
        m_headTransform.localScale = Vector3.one;
        m_headTransform.GetComponent<HingeJoint2D>().enabled = false;
        m_headTransform.GetComponent<Collider2D>().enabled = true;
        m_headTransform.GetComponent<Rigidbody2D>().isKinematic = false;
        m_headTransform.GetComponent<Rigidbody2D>().gravityScale = 8f;
        m_headTransform.GetComponent<Rigidbody2D>().velocity = direction.normalized * m_throwVelocity;
    }

    IEnumerator SlowDown()
    {
        float deltaDecrease = 1/m_slowDownTime;
        while (Time.timeScale > 0)
        {
            //Debug.LogError(Time.timeScale);
            Time.timeScale = Mathf.Clamp01(Time.timeScale - deltaDecrease * Time.unscaledDeltaTime);
            yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime);
        }
        Time.timeScale = 0f;
    }
}
