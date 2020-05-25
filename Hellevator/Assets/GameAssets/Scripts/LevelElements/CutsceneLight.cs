using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneLight : TemporalSingleton<CutsceneLight>
{

	[SerializeField] private Transform[] m_lightRoute = new Transform[0];
	[SerializeField] private float m_lightSpeed = 0f;
	[SerializeField] private DemonBase m_demonToPosesAfterCutscene = null;
	[SerializeField] private float m_stopingDistance = 0f;
    [SerializeField] private float m_angularVelocity = 30f;

    private Vector3 m_currentDirection;
	private int m_currentDestination = 0;


	private void Start()
    {
		if (LevelManager.Instance.IsRestarting)
		{
			if (LevelManager.Instance.CheckPoints.Count != 0)
			{
				DestroyCutscene();
			}
			else
			{
				CameraManager.Instance.SetCameraFocus(this.transform);
				ParalaxManager.Instance.SetUpSceneParalax();
			}
		}
		else
		{
			CameraManager.Instance.SetCameraFocus(this.transform);
			ParalaxManager.Instance.SetUpSceneParalax();
		}
    }


    // Update is called once per frame
    void Update()
    {
        if(LevelManager.Instance.LastCheckPoint == null)
        {
			if (InputManager.Instance.CanMove)
			{
				if (m_currentDestination == m_lightRoute.Length)
				{
					PosesionManager.Instance.PossessNewDemon(m_demonToPosesAfterCutscene);
					Destroy(this.gameObject);
				}

                m_currentDirection = Vector3.MoveTowards(m_currentDirection, m_lightRoute[m_currentDestination].position - transform.position, m_angularVelocity * Time.deltaTime);
				//this.transform.position = Vector3.MoveTowards(this.transform.position, m_lightRoute[m_currentDestination].position, m_lightSpeed * Time.deltaTime);
                this.transform.position += m_currentDirection.normalized * m_lightSpeed * Time.deltaTime;

				if (Vector3.Distance(this.transform.position, m_lightRoute[m_currentDestination].position) <= m_stopingDistance)
				{
					m_currentDestination = m_currentDestination + 1;
					if (m_currentDestination == m_lightRoute.Length)
					{
						PosesionManager.Instance.PossessNewDemon(m_demonToPosesAfterCutscene);
						CameraManager.Instance.ChangeFocusOfMainCameraTo(m_demonToPosesAfterCutscene.transform);
						Destroy(this.gameObject);
					}
				}
			}
        }
        else
        {
            DestroyCutscene();
        }


		
    }

	public void DestroyCutscene()
	{
		Destroy(this.gameObject);
	}
}
