using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Catapult : MonoBehaviour
{
    [SerializeField] float m_slowDownTime = 0.5f;
    [SerializeField] float m_throwVelocity = 5f;
    [SerializeField] Transform m_headTransform = null;
    [SerializeField] float m_timeToThrowHead = 3f;
    DemonBase m_demon;
    Vector2 directionToThrowHead;
    //[SerializeField] int m_segments = 60;
    [SerializeField] Transform m_headSprite;
    LineRenderer lr;
    [SerializeField] LayerMask m_defaultMask;

    bool m_hasThrown;
    private void Awake()
    {
        m_demon = GetComponent<DemonBase>();
        lr = GetComponent<LineRenderer>();
    }
    /// <summary>
    /// Empieza la corrutina de apuntado
    /// </summary>
    public void ShowAim()
    {
        if (!m_hasThrown)
        {
            m_demon.StopLerpResetRagdoll();
            m_hasThrown = true;
            m_demon.CanMove = false;
            StartCoroutine(SelectLandingSpot());
        }
    }
    /// <summary>
    /// Primero ralentiza el tiempo y dice al input manager que se está apuntando
    /// </summary>
    /// <returns></returns>
    IEnumerator SelectLandingSpot()
    {
        yield return SlowDown();
        InputManager.Instance.ThrowingHead = true;
        float timeToThrowHead = m_timeToThrowHead;
        Vector3 lastMousePosition = Input.mousePosition;
        directionToThrowHead = CameraManager.Instance.MainCamera.ScreenToWorldPoint(Input.mousePosition) - m_demon.Torso.position;
        if (timeToThrowHead > 0)
        {

            while (timeToThrowHead > 0)
            {
                timeToThrowHead -= Time.unscaledDeltaTime;
                directionToThrowHead = CameraManager.Instance.MainCamera.ScreenToWorldPoint(Input.mousePosition) - m_demon.Torso.position;
                DrawTrajectory(directionToThrowHead);
                yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime);
            }
        }
        else
        {
            while (true)
            {
                if (Input.mousePosition != lastMousePosition)
                {
                    lastMousePosition = Input.mousePosition;
                    directionToThrowHead = CameraManager.Instance.MainCamera.ScreenToWorldPoint(Input.mousePosition) - m_demon.Torso.position;
                }
                else
                {
                    directionToThrowHead += (InputManager.Instance.VerticalInputValue * Vector2.up + InputManager.Instance.MoveInputValue * Vector2.right) /** Time.deltaTime*/;
                }
                DrawTrajectory(directionToThrowHead);
                yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime);
            }
        }        
    }


    private void DrawTrajectory(Vector2 direction)
    {
        //Vector2[] points = new Vector2[m_segments];
        //points[0] = m_demon.Torso.position;
        //for (int i = 1; i < m_segments; i++)
        //{
        //    float xIncrement = (direction.normalized.x * m_throwVelocity * (direction.x / direction.magnitude) * i);
        //    float yIncrement = direction.normalized.y * m_throwVelocity * (direction.y / direction.magnitude) * i - 4*i*i;
        //    points[i] = new Vector2(points[i - 1].x + xIncrement, points[i - 1].y + yIncrement);
        //}


        Vector3 [] drawnPoints = Ballistics.GetBallisticPath(m_headTransform.position, direction.normalized, m_throwVelocity, Time.unscaledDeltaTime,8f);
        //for (int i = 1; i < drawnPoints.Length; i++)
        //{
        //    Debug.DrawRay(drawnPoints[i - 1], drawnPoints[i] - drawnPoints[i - 1], Color.green);

        //}
        Vector3[] interr = Ballistics.GetBallisticPathInterrupted(drawnPoints,m_defaultMask);
        lr.positionCount = interr.Length;
        lr.SetPositions(interr);
    }

    public void ThrowHead()
    {
        StopAllCoroutines();
        m_demon.DeactivateAnimator();
        CameraManager.Instance.SetCameraFocus(m_headTransform);
        m_headSprite.transform.parent = null;
        Time.timeScale = 1;
        InputManager.Instance.ThrowingHead = false;
        m_headTransform.parent = null;
        m_headTransform.localScale = Vector3.one;
        m_headTransform.GetComponent<HingeJoint2D>().enabled = false;
        m_headTransform.GetComponent<Collider2D>().enabled = true;
        m_headTransform.GetComponent<CatapultHead>().m_active = true;
        m_headTransform.GetComponent<CatapultHead>().LastPosition = m_headTransform.position;
        Rigidbody2D headRigidbody = m_headTransform.GetComponent<Rigidbody2D>();
        headRigidbody.isKinematic = false;
        headRigidbody.gravityScale = 8f;
        headRigidbody.transform.position = m_headTransform.position;
        headRigidbody.velocity = directionToThrowHead.normalized * m_throwVelocity;
        //Debug.LogError("Set velocity as: " + directionToThrowHead.normalized * m_throwVelocity);
        lr.positionCount = 0;
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

    public static class Ballistics
    {

        // Note, doesn't take drag into account.

        /// <summary>
        /// Calculate the lanch angle.
        /// </summary>
        /// <returns>Angle to be fired on.</returns>
        /// <param name="start">The muzzle.</param>
        /// <param name="end">Wanted hit point.</param>
        /// <param name="muzzleVelocity">Muzzle velocity.</param>
        public static bool CalculateTrajectory(Vector3 start, Vector3 end, float muzzleVelocity, out float angle)
        {//, out float highAngle){

            Vector3 dir = end - start;
            float vSqr = muzzleVelocity * muzzleVelocity;
            float y = dir.y;
            dir.y = 0.0f;
            float x = dir.sqrMagnitude;
            float g = -Physics.gravity.y;

            float uRoot = vSqr * vSqr - g * (g * (x) + (2.0f * y * vSqr));


            if (uRoot < 0.0f)
            {

                //target out of range.
                angle = -45.0f;
                //highAngle = -45.0f;
                return false;
            }

            //        float r = Mathf.Sqrt (uRoot);
            //        float bottom = g * Mathf.Sqrt (x);

            angle = -Mathf.Atan2(g * Mathf.Sqrt(x), vSqr + Mathf.Sqrt(uRoot)) * Mathf.Rad2Deg;
            //highAngle = -Mathf.Atan2 (bottom, vSqr - r) * Mathf.Rad2Deg;
            return true;

        }

        /// <summary>
        /// Gets the ballistic path.
        /// </summary>
        /// <returns>The ballistic path.</returns>
        /// <param name="startPos">Start position.</param>
        /// <param name="forward">Forward direction.</param>
        /// <param name="velocity">Velocity.</param>
        /// <param name="timeResolution">Time from frame to frame.</param>
        /// <param name="maxTime">Max time to simulate, will be clamped to reach height 0 (aprox.).</param>

        public static Vector3[] GetBallisticPath(Vector3 startPos, Vector3 forward, float velocity, float timeResolution, float gravity, float maxTime = Mathf.Infinity)
        {

            maxTime = Mathf.Min(maxTime, GetTimeOfFlight(velocity, Vector3.Angle(forward, Vector3.right) * Mathf.Deg2Rad, startPos.y, gravity));
            //Debug.LogError(maxTime);
            Vector3[] positions = new Vector3[Mathf.CeilToInt(maxTime / timeResolution)];
            Vector3 velVector = forward * velocity;
            int index = 0;
            Vector3 curPosition = startPos;
            for (float t = 0.0f; t < maxTime; t += timeResolution)
            {

                if (index >= positions.Length)
                    break;//rounding error using certain values for maxTime and timeResolution

                positions[index] = curPosition;
                curPosition += velVector * timeResolution;
                velVector += Vector3.down * gravity * 9.8f * timeResolution;
                index++;
            }
            return positions;
        }

        /// <summary>
        /// Checks the ballistic path for collisions.
        /// </summary>
        /// <returns><c>false</c>, if ballistic path was blocked by an object on the Layermask, <c>true</c> otherwise.</returns>
        /// <param name="arc">Arc.</param>
        /// <param name="lm">Anything in this layer will block the path.</param>
        public static bool CheckBallisticPath(Vector3[] arc, LayerMask lm)
        {

            RaycastHit hit;
            for (int i = 1; i < arc.Length; i++)
            {

                if (Physics.Raycast(arc[i - 1], arc[i] - arc[i - 1], out hit, (arc[i] - arc[i - 1]).magnitude) && IsInLayerMask(hit.transform.gameObject.layer, lm))
                    return false;

                //            if (Physics.Raycast (arc [i - 1], arc [i] - arc [i - 1], out hit, (arc [i] - arc [i - 1]).magnitude) && GameMaster.IsInLayerMask(hit.transform.gameObject.layer, lm)) {
                //                Debug.DrawRay (arc [i - 1], arc [i] - arc [i - 1], Color.red, 10f);
                //                return false;
                //            } else {
                //                Debug.DrawRay (arc [i - 1], arc [i] - arc [i - 1], Color.green, 10f);
                //            }
            }
            return true;
        }

        public static Vector3[] GetBallisticPathInterrupted(Vector3[] arc, LayerMask lm)
        {
            List<Vector3> interruptedPath = new List<Vector3>();
            if (arc != null && arc.Length > 0)
            {
                interruptedPath.Add(arc[0]);
            }
            else
            {
                Debug.LogError("Calculated trajectory is empty");
                return null;
            }
            RaycastHit2D hit;
            for (int i = 1; i < arc.Length; i++)
            {
                hit = Physics2D.CircleCast(arc[i - 1], 1.06f, arc[i] - arc[i - 1], (arc[i] - arc[i - 1]).magnitude/*, lm*/);
                //Debug.LogError(hit.transform.name);
                if (hit.transform != null && !hit.collider.isTrigger && IsInLayerMask(hit.transform.gameObject.layer, lm))
                {
                    return interruptedPath.ToArray();
                }
                else
                {
                    interruptedPath.Add(arc[i]);
                }


                //            if (Physics.Raycast (arc [i - 1], arc [i] - arc [i - 1], out hit, (arc [i] - arc [i - 1]).magnitude) && GameMaster.IsInLayerMask(hit.transform.gameObject.layer, lm)) {
                //                Debug.DrawRay (arc [i - 1], arc [i] - arc [i - 1], Color.red, 10f);
                //                return false;
                //            } else {
                //                Debug.DrawRay (arc [i - 1], arc [i] - arc [i - 1], Color.green, 10f);
                //            }
            }
            //Debug.LogError("uninterrupted trajectory. Arc length: " + arc.Length);
            return interruptedPath.ToArray();
        }

        private static bool IsInLayerMask(LayerMask raylayer, LayerMask hitLayer)
        {
            return hitLayer == (hitLayer | 1 << raylayer);
        }
        public static Vector3 GetHitPosition(Vector3 startPos, Vector3 forward, float velocity, float gravity)
        {

            Vector3[] path = GetBallisticPath (startPos, forward, velocity, .35f,gravity);
            RaycastHit hit;
            for (int i = 1; i < path.Length; i++)
            {

                //Debug.DrawRay (path [i - 1], path [i] - path [i - 1], Color.red, 10f);
                if (Physics.Raycast(path[i - 1], path[i] - path[i - 1], out hit, (path[i] - path[i - 1]).magnitude))
                {
                    return hit.point;
                }
            }

            return Vector3.zero;
        }


        public static float CalculateMaxRange(float muzzleVelocity)
        {
            return (muzzleVelocity * muzzleVelocity) / (-Physics.gravity.y * 8);
        }

        public static float GetTimeOfFlight(float vel, float angle, float height, float gravity)
        {
            //Debug.LogError("Velocity: " + vel + ". Angle: " + angle + ".");
            return (2.0f * vel * Mathf.Sin(angle)) / (-Physics.gravity.y * gravity);
        }

    }
}
