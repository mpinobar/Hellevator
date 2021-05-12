using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    [SerializeField] Sprite m_bigSprite;
    [SerializeField] AudioClip clip;

    [SerializeField]
    string m_ID;

    [SerializeField] private float m_timeBeforeCanClosePopUp = 2f;
    [SerializeField] private float m_timeBeforeBigPopUp = 4f;
    [SerializeField] private float m_timeBeforeMiniPopUp = 2.5f;

    [SerializeField] private Cinemachine.CinemachineVirtualCamera m_collectibleCamera;
    [SerializeField] private Cinemachine.CinemachineVirtualCamera m_mainCamera;

    [SerializeField] private GameObject m_miniPrefab;
    GameObject prefab;

    bool canClose = false;
    bool canOpenBig = false;
    bool canMove = false;


    void Start()
    {
        if (PlayerPrefs.HasKey(m_ID) && PlayerPrefs.GetInt(m_ID) > 0)
        {

            gameObject.SetActive(false);
        }
        else
        {
            if (prefab)
                InputManager.Instance.OnInteract += CloseCollectible;
        }

    }

    private void OnDisable()
    {
        if (prefab)
            InputManager.Instance.OnInteract -= CloseCollectible;
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
        if (canClose)
        {
            CameraManager.Instance.SetCurrentLiveCamera(m_mainCamera);
            UIController.Instance.HideCollectibleInGame();
            InputManager.Instance.IsInInteactionTrigger = false;
            canMove = false;
            PossessionManager.Instance.ControlledDemon.GetComponent<Animator>().updateMode = AnimatorUpdateMode.Normal;
            Time.timeScale = 1;
            Destroy(prefab);            
            gameObject.SetActive(false);

        }
        else if (canOpenBig)
        {
            UIController.Instance.ShowCollectibleInGame(m_bigSprite);
            this.GetComponent<SpriteRenderer>().enabled = false;
            StartCoroutine(waitBeforeClosing());
            canOpenBig = false;
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        DemonBase character = collision.GetComponentInParent<DemonBase>();
        if (character && character.IsControlledByPlayer)
        {
            Collect();
        }
    }
    public void Collect()
    {
        if (prefab)
        {
            Time.timeScale = 0;
            InputManager.Instance.IsInInteactionTrigger = true;
            InputManager.Instance.ResetPlayerInput();

            canClose = false;
            canOpenBig = false;
            canMove = true;

            CameraManager.Instance.SetCurrentLiveCamera(m_collectibleCamera);

            this.GetComponent<SpriteRenderer>().enabled = false;
            this.GetComponent<Collider2D>().enabled = false;

            if (clip)
                AudioManager.Instance.PlayAudioSFX(clip, false);

            AchievementsManager.AddCollectible();
            PlayerPrefs.SetInt(m_ID, 1);

            StartCoroutine(waitBeforePopUp());
            StartCoroutine(spawnMiniAfterDelay());
        }
        else
        {
            this.GetComponent<SpriteRenderer>().enabled = false;
            this.GetComponent<Collider2D>().enabled = false;
            if (clip)
                AudioManager.Instance.PlayAudioSFX(clip, false);

            AchievementsManager.AddCollectible();
            PlayerPrefs.SetInt(m_ID, 1);
        }

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

    IEnumerator waitBeforePopUp()
    {
        yield return new WaitForSecondsRealtime(m_timeBeforeBigPopUp);
        canOpenBig = true;
    }

    IEnumerator waitBeforeClosing()
    {
        yield return new WaitForSecondsRealtime(m_timeBeforeCanClosePopUp);
        canClose = true;
    }
}
