using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerMatadero : MonoBehaviour
{
    [SerializeField] DemonBase m_prefabCharacterToSpawn;
    [SerializeField] float m_timeToSpawnNewCharacter = 1f;
    float m_timeToSpawnNewCharacterTimer = 0f;

    [SerializeField] Transform m_startingPosition;
    [SerializeField] Transform m_endingPosition;
    [SerializeField] float m_movementSpeed = 2f;

    List<DemonBase> m_spawnedCharacters;

    Vector2 m_movingDirection;

    private void Awake()
    {
        m_spawnedCharacters = new List<DemonBase>();
        m_timeToSpawnNewCharacterTimer = m_timeToSpawnNewCharacter;
        
    }

    // Start is called before the first frame update
    void Start()
    {
        m_movingDirection = (m_endingPosition.position - m_startingPosition.position).normalized;
    }

    // Update is called once per frame
    void LateUpdate()
    {

        m_timeToSpawnNewCharacterTimer -= Time.deltaTime;
        if(m_timeToSpawnNewCharacterTimer <= 0)
        {
            SpawnNewCharacter();
            m_timeToSpawnNewCharacterTimer = m_timeToSpawnNewCharacter;
        }

        if(m_spawnedCharacters.Count > 0)
        {
            //Debug.LogError(Vector2.Distance(m_spawnedCharacters[0].Torso.transform.position, m_endingPosition.position));
            for (int i = 0; i < m_spawnedCharacters.Count; i++)
            {
                if (m_spawnedCharacters[i].IsControlledByPlayer)
                {
                    m_spawnedCharacters[i].transform.parent = null;
                    m_spawnedCharacters[i].SetRagdollNewGravity(1);
                    m_spawnedCharacters.Remove(m_spawnedCharacters[i]);
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
                    //Debug.LogError("movingCharacter" + i);
                    //m_spawnedCharacters[i].SetTorsoKinematic(true);
                    m_spawnedCharacters[i].Torso.position += (Vector3) m_movingDirection * m_movementSpeed * Time.deltaTime;
                }
            }
        }
        
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
