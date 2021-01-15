using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSceneChange : MonoBehaviour
{
    [SerializeField] string m_linkedScene;
    [SerializeField] Transform m_positionToSetAfterEntering;

	[SerializeField] AudioClip m_changeSceneSFX = null;
    [SerializeField] float m_delayToActivateCollider = 0f;

	public string LinkedScene { get => m_linkedScene; set => m_linkedScene = value; }
    public Transform PositionToSetAfterEntering { get => m_positionToSetAfterEntering; set => m_positionToSetAfterEntering = value; }

    private IEnumerator Start()
    {
        if(m_delayToActivateCollider > 0)
        {
            GetComponent<Collider2D>().enabled = false;
            yield return new WaitForSeconds(m_delayToActivateCollider);
            GetComponent<Collider2D>().enabled = true;
        }
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        DemonBase demon = collision.GetComponent<DemonBase>();
        if (demon != null)
        {
            if (demon.IsControlledByPlayer)
            {
                Debug.LogError("Loading scene " + m_linkedScene);
				AudioManager.Instance.PlayAudioSFX(m_changeSceneSFX, false, 2f);
				PossessionManager.Instance.ChangeMainCharacter(demon);
                LevelManager.Instance.SwitchToAdjacentScene(m_linkedScene);
                GetComponent<Collider2D>().enabled = false;
                System.GC.Collect();
            }
        }
    }
}
