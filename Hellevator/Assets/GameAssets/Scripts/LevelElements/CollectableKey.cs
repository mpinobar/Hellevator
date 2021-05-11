using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableKey : MonoBehaviour
{
    [SerializeField] Key key;
    [SerializeField] bool m_checkPlayerPrefs;
	[SerializeField] AudioClip clip = null;

    [SerializeField] private float m_timeBeforeCanClosePopUp = 2f;
    [SerializeField] private float m_timeBeforeBigPopUp = 4f;
    [SerializeField] private float m_timeBeforeMiniPopUp = 2.5f;

    [SerializeField] private Cinemachine.CinemachineVirtualCamera m_collectibleCamera;
    [SerializeField] private Cinemachine.CinemachineVirtualCamera m_mainCamera;

    [SerializeField] private GameObject m_miniPrefab;
    GameObject prefab;

    bool canClose = false;

    bool canMove = false;

    private void Start()
    {
        if (m_checkPlayerPrefs)
        {
            if (PlayerPrefs.GetInt(key.ToString()) == 1)
            {
                Destroy(gameObject);
            }
            else
            {
                InputManager.Instance.OnInteract += CloseCollectible;
            }
        }
        else
        {
            InputManager.Instance.OnInteract += CloseCollectible;
        }
    }
    private void Update()
    {
        if (canMove)
        {
            PossessionManager.Instance.ControlledDemon.GetComponent<Rigidbody2D>().velocity = new Vector3(0, PossessionManager.Instance.ControlledDemon.GetComponent<Rigidbody2D>().velocity.y, 0);
            InputManager.Instance.ResetPlayerInput();
        }
    }


    public void CloseCollectible()
    {
        print("a");
        if (canClose)
        {
            print("b");
            CameraManager.Instance.SetCurrentLiveCamera(m_mainCamera);
            UIController.Instance.HideCollectibleInGame();
            InputManager.Instance.IsInInteactionTrigger = false;
            PossessionManager.Instance.ControlledDemon.GetComponent<Animator>().updateMode = AnimatorUpdateMode.Normal;
            Time.timeScale = 1;
            canMove = false;
            Destroy(prefab);
            Destroy(gameObject);
        }
    }

    private void OnDisable()
    {
        InputManager.Instance.OnInteract -= CloseCollectible;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponentInParent<DemonBase>())
        {
			AudioManager.Instance.PlayAudioSFX(clip, false, 2f);
            if (m_checkPlayerPrefs)
            {
                if (PlayerPrefs.GetInt(key.ToString()) == 0)
                {
                    PlayerPrefs.SetInt(key.ToString(), 1);
                    PlayerPrefs.Save();
                    Collect();
                }
            }
            else
            {
                LevelManager.Instance.HasKitchenKey = true;
                PlayerPrefs.SetInt(key.ToString(), 1);
                PlayerPrefs.Save();
                Collect();
            }
        }
    }

    public void Collect()
    {
        Time.timeScale = 0;

        InputManager.Instance.IsInInteactionTrigger = true;
        InputManager.Instance.ResetPlayerInput();

        canClose = false;
        canMove = true;

        CameraManager.Instance.SetCurrentLiveCamera(m_collectibleCamera);

        this.GetComponentInChildren<SpriteRenderer>().enabled = false;
        this.GetComponent<Collider2D>().enabled = false;

        StartCoroutine(waitBeforeClosing());
        StartCoroutine(spawnMiniAfterDelay());
    }

    IEnumerator spawnMiniAfterDelay()
    {
        yield return new WaitForSecondsRealtime(m_timeBeforeMiniPopUp);

        PossessionManager.Instance.ControlledDemon.GetComponent<Animator>().updateMode = AnimatorUpdateMode.UnscaledTime;
        prefab = Instantiate(m_miniPrefab, new Vector3(0, 1, 0), Quaternion.identity, PossessionManager.Instance.ControlledDemon.transform);
        if (PossessionManager.Instance.ControlledDemon.transform.localScale.x < 0)
        {
            prefab.GetComponent<Animator>().SetTrigger("Negativa");
        }
        else
        {
            prefab.GetComponent<Animator>().SetTrigger("Positiva");
        }
        yield return new WaitForSecondsRealtime(0.5f);
        PossessionManager.Instance.ControlledDemon.GetComponent<Animator>().SetTrigger("Jump");
    }

    IEnumerator waitBeforeClosing()
    {
        yield return new WaitForSecondsRealtime(m_timeBeforeCanClosePopUp);
        print("Hello");
        canClose = true;
    }
}
