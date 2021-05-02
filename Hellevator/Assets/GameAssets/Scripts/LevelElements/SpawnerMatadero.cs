using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class SpawnerMatadero : MonoBehaviour
{
    [SerializeField] DemonBase m_prefabCharacterToSpawn;
    [SerializeField] float m_timeToSpawnNewCharacter = 1f;
    float m_timeToSpawnNewCharacterTimer = 0f;

    [SerializeField] Transform m_startingPosition;
    [SerializeField] Transform m_endingPosition;
    [SerializeField] float m_movementSpeed = 2f;
    [SerializeField] float m_hookOffset = 2f;

    List<DemonBase> m_spawnedCharacters;
    List<Transform> m_attachedParts;
    List<Transform> m_hooks;
    [SerializeField] Transform m_hookPrefab;
    Vector2 m_movingDirection;
    [SerializeField] AudioClip m_spawnerClip;

    private void Awake()
    {
        m_spawnedCharacters = new List<DemonBase>();
        m_timeToSpawnNewCharacterTimer = m_timeToSpawnNewCharacter;
        m_hooks = new List<Transform>();
        for (int i = 0; i < 10; i++)
        {
            m_hooks.Add(Instantiate(m_hookPrefab, m_startingPosition.position + Vector3.up * m_hookOffset, Quaternion.identity));
        }

    }
    Camera cam;
    bool playingAudio;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        m_movingDirection = (m_endingPosition.position - m_startingPosition.position).normalized;
        AudioManager.Instance.PlayAudioSFX(m_spawnerClip, true);
        playingAudio = true;
    }

    // Update is called once per frame
    void LateUpdate()
    {

        m_timeToSpawnNewCharacterTimer -= Time.deltaTime;
        if (m_timeToSpawnNewCharacterTimer <= 0)
        {
            SpawnNewCharacter();
            m_timeToSpawnNewCharacterTimer = m_timeToSpawnNewCharacter;
        }

        MoveAttachedCharacters();

        MoveAttachedBodyParts();
        
        if (playingAudio)
        {
            Vector2 startPos = cam.WorldToViewportPoint(m_startingPosition.position);
            Vector2 endPos = cam.WorldToViewportPoint(m_endingPosition.position);
            //Debug.LogError("Start " + startPos + " end " + endPos);
            if ((startPos.x < 0 || startPos.y < 0 || startPos.x > 1 || startPos.y > 1) && (endPos.x < 0 || endPos.y < 0 || endPos.x > 1 || endPos.y > 1))
            {
                AudioManager.Instance.StopSFX(m_spawnerClip);
                playingAudio = false;
            }            
        }
        else
        {
            Vector2 startPos = cam.WorldToViewportPoint(m_startingPosition.position);
            Vector2 endPos = cam.WorldToViewportPoint(m_endingPosition.position);
            if ((startPos.x > 0 && startPos.y > 0 && startPos.x < 1 && startPos.y < 1) || (endPos.x > 0 && endPos.y > 0 && endPos.x < 1 && endPos.y < 1))
            {
                AudioManager.Instance.PlayAudioSFX(m_spawnerClip, true);
                playingAudio = true;
            }
        }
    }

    private void MoveAttachedCharacters()
    {
        if (m_spawnedCharacters.Count > 0)
        {
            //Debug.LogError(Vector2.Distance(m_spawnedCharacters[0].Torso.transform.position, m_endingPosition.position));
            for (int i = 0; i < m_spawnedCharacters.Count; i++)
            {
                if (m_spawnedCharacters[i].IsControlledByPlayer || !m_spawnedCharacters[i].enabled)
                {
                    DetachCharacter(m_spawnedCharacters[i]);
                    i--;
                }
                else if (Vector2.Distance(m_spawnedCharacters[i].Torso.transform.position, m_endingPosition.position) <= 2f)
                {
                    m_spawnedCharacters[i].gameObject.SetActive(false);
                    m_spawnedCharacters[i].IsPossessionBlocked = true;
                    ((BasicZombie)m_spawnedCharacters[i]).SkullIndicator();
                }
                else
                {
                    m_spawnedCharacters[i].Torso.position += (Vector3)m_movingDirection * m_movementSpeed * Time.deltaTime;
                    m_hooks[i].transform.position = m_spawnedCharacters[i].Torso.position + Vector3.up * m_hookOffset;
                }
            }


        }
    }

    private void MoveAttachedBodyParts()
    {
        if (m_attachedParts != null)
        {
            for (int i = 0; i < m_attachedParts.Count; i++)
            {
                m_attachedParts[i].position += (Vector3)m_movingDirection * m_movementSpeed * Time.deltaTime;
                m_hooks[i + m_spawnedCharacters.Count].transform.position = m_attachedParts[i].position + Vector3.up * m_hookOffset;
                if (Vector2.Distance(m_attachedParts[i].position, m_endingPosition.position) <= 2f)
                {
                    DestroyPartAndParentDemon(m_attachedParts[i]);
                }
            }
        }
    }

    public void DetachCharacter(DemonBase characterToDetach)
    {
        if (m_spawnedCharacters.Contains(characterToDetach))
        {
            characterToDetach.transform.parent = null;
            characterToDetach.SetRagdollNewGravity(5);
            m_spawnedCharacters.Remove(characterToDetach);
        }
        else
        {
            Debug.LogError("Trying to detach a character that isn't attached. Character is: " + characterToDetach.name);
        }
    }

    public void DetachPart(Transform partToDetach)
    {
        if (m_attachedParts.Contains(partToDetach))
        {
            partToDetach.transform.parent = null;
            //characterToDetach.SetRagdollNewGravity(1);
            m_attachedParts.Remove(partToDetach);
            partToDetach.GetComponent<Rigidbody2D>().isKinematic = false;
            //Debug.LogError("Detaching part: " + partToDetach.name);
        }
    }

    public void AttachCharacterPart(Transform part)
    {
        if (m_attachedParts == null)
        {
            m_attachedParts = new List<Transform>();
        }
        m_attachedParts.Add(part);
        part.parent = transform;
        part.GetComponent<Rigidbody2D>().isKinematic = true;
        //desactiva el game object del fuego de posesion
        part.GetChild(part.childCount - 1).gameObject.SetActive(false);
    }

    private void DestroyPartAndParentDemon(Transform partToDestroy)
    {
        Destroy(partToDestroy.gameObject);
        m_attachedParts.Remove(partToDestroy);
        BasicZombie parentDemon = (BasicZombie)partToDestroy.GetComponentInChildren<RagdollLogicalCollider>(true).ParentDemon;
        SpriteSkin [] skins = parentDemon.GetComponentsInChildren<SpriteSkin>();

        for (int i = 0; i < skins.Length; i++)
        {
            if (skins[i].boneTransforms[0] && skins[i].boneTransforms[0].parent != null)
            {
                skins[i].gameObject.SetActive(false);
            }
        }
        Destroy(parentDemon.gameObject, 10f);
        //.gameObject.SetActive(false);
        //partToDestroy.GetComponentInChildren<RagdollLogicalCollider>(true).ParentDemon.gameObject.SetActive(true);
        //Destroy(partToDestroy.GetComponentInChildren<RagdollLogicalCollider>(true).ParentDemon.gameObject);
    }

    public void SpawnNewCharacter()
    {
        if (m_spawnedCharacters.Count > 0)
        {
            for (int i = 0; i < m_spawnedCharacters.Count; i++)
            {
                if (!m_spawnedCharacters[i].gameObject.activeSelf)
                {
                    InitializeCharacterOnStartingPosition(m_spawnedCharacters[i]);
                    return;
                }
            }
        }
        DemonBase spawnedCharacter = Instantiate(m_prefabCharacterToSpawn,transform);
        m_spawnedCharacters.Add(spawnedCharacter);
        InitializeCharacterOnStartingPosition(spawnedCharacter);

    }

    public void InitializeCharacterOnStartingPosition(DemonBase characterToInitialize)
    {
        characterToInitialize.gameObject.SetActive(true);
        characterToInitialize.ResetRagdollTransforms();
        characterToInitialize.IsPossessionBlocked = false;
        ((BasicZombie)characterToInitialize).SkullIndicator();
        characterToInitialize.transform.position = m_startingPosition.position;
        characterToInitialize.SetRagdollNewGravity(0);
    }

}
