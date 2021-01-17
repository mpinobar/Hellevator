using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VendingMachineCheckpoint : ActivatedBase
{

    [SerializeField] DemonBase[] m_charactersToSpawn;
    [SerializeField] Sprite [] m_masks;
    [SerializeField] Color [] m_colors;
    [SerializeField] SpriteRenderer m_sprRenderer;
    [SerializeField] Transform m_spawnPoint;
    [SerializeField] Transform m_acidCover;

    DemonBase m_spawnedCharacter;

    private enum BodyTypeShown
    {
        Normal, Petrified, Explosive
    }

    private BodyTypeShown m_typeShown = BodyTypeShown.Normal;

    /// <summary>
    /// Override del metodo base, cambia el cuerpo que se muestra en la maquina vendedora
    /// </summary>
    public override void Activate()
    {
        m_typeShown = (BodyTypeShown)(((int)m_typeShown + 1) % 3);
        SwitchShownCharacterVisual();
    }

    /// <summary>
    /// Spawnea el personaje que se muestra
    /// </summary>
    public void SpawnCharacter()
    {
        m_spawnedCharacter = Instantiate(m_charactersToSpawn[(int)m_typeShown], m_spawnPoint.position, Quaternion.identity, transform);
        m_spawnedCharacter.transform.parent = null;
    }

    /// <summary>
    /// Actualiza el cuerpo que se muestra en la maquina vendedora
    /// </summary>
    public void SwitchShownCharacterVisual()
    {
        m_sprRenderer.sprite = m_masks[(int)m_typeShown];
        m_sprRenderer.color = m_colors[(int)m_typeShown];
    }


    public void OnTriggerEnter2D(Collider2D collision)
    {
        collision.TryGetComponent(out DemonBase character);

        if (character && character.IsControlledByPlayer)
        {
            if (character == m_spawnedCharacter)
            {
                m_spawnedCharacter = null;
            }
            else
            {
                SpawnCharacter();
                character.CanMove = false;
                ((BasicZombie)character).ResetVelocity();
                character.Move(0);
                StartCoroutine(ShowAcid());
            }
        }
    }


    private void Start()
    {
        SwitchShownCharacterVisual();
    }

    IEnumerator ShowAcid()
    {
        float time = 0;
        Vector3 acidInitialPosition = m_acidCover.position;
        while (time < 1)
        {
            time += Time.deltaTime;
            m_acidCover.position -= Vector3.right * Time.deltaTime * 8;
            yield return null;
        }

        time = 0;
        while (time < 0.5f)
        {
            time += Time.deltaTime;
            m_acidCover.position = Vector3.Lerp(m_acidCover.position, acidInitialPosition + Vector3.right, Time.deltaTime * 5);
            yield return null;
        }

    }
}
