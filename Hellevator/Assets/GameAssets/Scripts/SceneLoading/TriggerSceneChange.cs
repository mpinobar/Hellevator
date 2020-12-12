using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSceneChange : MonoBehaviour
{
    [SerializeField] string m_linkedScene;
    [SerializeField] Transform m_positionToSetAfterEntering;

	[SerializeField] AudioClip m_changeSceneSFX = null;

	public string LinkedScene { get => m_linkedScene; set => m_linkedScene = value; }
    public Transform PositionToSetAfterEntering { get => m_positionToSetAfterEntering; set => m_positionToSetAfterEntering = value; }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        DemonBase demon = collision.GetComponent<DemonBase>();
        if (demon != null)
        {
            if (demon.IsControlledByPlayer)
            {
				MusicManager.Instance.PlayAudioSFX(m_changeSceneSFX, false, 2f);
				PossessionManager.Instance.ChangeMainCharacter(demon);
                LevelManager.Instance.SwitchToAdjacentScene(m_linkedScene);
                GetComponent<Collider2D>().enabled = false;
                System.GC.Collect();
            }
        }
    }
}
