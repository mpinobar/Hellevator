using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeightedPreassurePlate : MonoBehaviour
{
    [Header("Weight and type variables")]
    //Weight and type variables
    [SerializeField] private TypeOfPreassurePlate m_type = TypeOfPreassurePlate.None;
    [SerializeField] private float m_weightNeeded;
    [Tooltip("Check this if the pressure plate blocks possession of bodies on this pressure plate")]
    [SerializeField] private bool m_blocksPossession;
    private float m_currentWeight;
    private List<DemonBase> m_enemiesOnPreassurePlate;
    [SerializeField] private LayerMask m_enemyLayerMask;

    [Header("Position variables")]
    //Position Variables
    [SerializeField] private float m_speed;
    [SerializeField] private Transform m_pressurePlateEndPosition;
    [SerializeField] private Transform m_parent;
    private Vector3 m_startingPosition;
    private float m_distanceToEndPosition;

    Vector3 m_endPosition;


    [Header("Linked objects variables")]
    //Linked Object variables (just for platform type preasureplates)
    [SerializeField] private Transform m_linkedObjectPosition;
    private Vector3 m_linkedObjectStartingPosition;
    [SerializeField] private Transform m_linkedObjectEndPosition;
    [SerializeField] private float m_linkedObjectSpeed;

    private float m_linkedObjectDistanceToEndPosition;

    [Header("Button variables")]
    //Button variables
    private bool m_preassurePlateActivated;
    private bool m_preassurePlateIsAtLocation;
    [SerializeField] private ActivatedBase m_buttonActivatedObject;
    List<SpikesWeightData> m_spikesData;
    [SerializeField] private bool m_activatesProjectileSpawner;


    //Sound variables
    [SerializeField] private AudioClip m_machineClip;
    private AudioSource m_audioSource;
    Vector3 m_lastLOPosition;

    private float m_percentage = 0f;
    private float m_positionY = 0f;
    private float m_LOpositionY = 0f;
    private float m_LOpositionX = 0f;


    private void Awake()
    {
        m_enemiesOnPreassurePlate = new List<DemonBase>(0);
        m_currentWeight = 0;
        m_audioSource = GetComponent<AudioSource>();
        if (m_audioSource)
        {
            m_audioSource.clip = m_machineClip;
            m_audioSource.loop = true;
            m_audioSource.volume = MusicManager.SfxVolume;
            m_audioSource.Stop();
        }

        if (m_type == TypeOfPreassurePlate.Elevator)
        {
            m_startingPosition = m_parent.localPosition;
            m_distanceToEndPosition = Vector3.Distance(m_startingPosition, m_pressurePlateEndPosition.localPosition);
        }
        else
        {
            m_startingPosition = m_parent.transform.position;
            m_distanceToEndPosition = Vector3.Distance(m_startingPosition, m_pressurePlateEndPosition.position);
        }
        m_spikesData = new List<SpikesWeightData>();
        if (m_linkedObjectPosition != null)
        {
            m_linkedObjectStartingPosition = m_linkedObjectPosition.position;
            m_endPosition = m_linkedObjectEndPosition.position;
            m_linkedObjectDistanceToEndPosition = Vector3.Distance(m_linkedObjectStartingPosition, m_linkedObjectEndPosition.position);
        }
        else if ((m_linkedObjectPosition == null && !m_activatesProjectileSpawner) && (m_type == TypeOfPreassurePlate.PaltformRaiser || m_type == TypeOfPreassurePlate.PlatformLowerer))
        {
            print("The preassureplate " + this.gameObject.name + " needs to have a Linked Object");
        }
    }

    private void Update()
    {
        //print(m_enemiesOnPreassurePlate.Count);
        CalculateAccumulatedWeight();
        switch (m_type)
        {
            //The linked Objects final positions needs to be higher than it's starting One;
            case TypeOfPreassurePlate.PaltformRaiser:
                {
                    if (!m_preassurePlateActivated)
                    {
                        m_percentage = m_currentWeight / m_weightNeeded;
                        if (m_percentage > 1f)
                        {
                            m_percentage = 1f;
                        }

                        if (m_activatesProjectileSpawner)
                        {
                            if (m_percentage >= 1)
                            {
                                m_buttonActivatedObject.Activate();
                            }
                            else
                            {
                                m_buttonActivatedObject.GetComponent<ProjectileSpawner>().Deactivate();
                            }
                        }
                        else
                        {
                            m_positionY = m_distanceToEndPosition * m_percentage;
                            m_parent.transform.position = Vector3.MoveTowards(m_parent.transform.position, new Vector3(m_startingPosition.x, m_startingPosition.y - m_positionY, m_startingPosition.z), m_speed * Time.deltaTime);

                            m_LOpositionY = m_linkedObjectDistanceToEndPosition * m_percentage;
                            m_LOpositionX = m_linkedObjectDistanceToEndPosition * m_percentage;


                            Vector3 destination = new Vector3(m_linkedObjectStartingPosition.x + m_LOpositionX, m_linkedObjectStartingPosition.y + m_LOpositionY, m_linkedObjectStartingPosition.z);

                            m_linkedObjectPosition.position = Vector3.MoveTowards(m_linkedObjectPosition.position, m_endPosition, m_linkedObjectSpeed * Time.deltaTime);

                            if (m_linkedObjectPosition.position == m_lastLOPosition)
                            {
                                if (m_audioSource)
                                    m_audioSource.Stop();
                            }
                            else if (!m_audioSource.isPlaying)
                            {
                                m_audioSource.Play();
                            }
                            //if (Vector3.Distance(m_linkedObjectPosition.position, m_endPosition/*destination*/) < 0.1f)
                            //{
                            //    if (m_audioSource)
                            //        m_audioSource.Stop();
                            //}
                            //else if (!m_audioSource.isPlaying)
                            //{
                            //    m_audioSource.Play();
                            //}

                            m_lastLOPosition = m_linkedObjectPosition.position;
                            //m_linkedObjectPosition.position = Vector3.MoveTowards(m_linkedObjectPosition.position, destination, m_linkedObjectSpeed * Time.deltaTime);
                        }
                    }
                }
                break;

            //The linked Objects final positions needs to be lower than it's starting One;
            case TypeOfPreassurePlate.PlatformLowerer:
                {
                    if (!m_preassurePlateActivated)
                    {
                        m_percentage = m_currentWeight / m_weightNeeded;

                        if (m_percentage > 1f)
                        {
                            m_percentage = 1f;
                        }
                        if (m_activatesProjectileSpawner)
                        {
                            if (m_percentage >= 1)
                            {
                                m_buttonActivatedObject.Activate();

                                if (m_audioSource && !m_audioSource.isPlaying)
                                    m_audioSource.Play();
                            }
                            else
                            {
                                if (m_audioSource)
                                    m_audioSource.Stop();
                                m_buttonActivatedObject.Deactivate();
                            }
                            m_positionY = m_distanceToEndPosition * m_percentage;
                            m_parent.transform.position = Vector3.MoveTowards(m_parent.transform.position, new Vector3(m_startingPosition.x, m_startingPosition.y - m_positionY, m_startingPosition.z), m_speed * Time.deltaTime);
                        }
                        else
                        {
                            m_positionY = m_distanceToEndPosition * m_percentage;
                            m_parent.transform.position = Vector3.MoveTowards(m_parent.transform.position, new Vector3(m_startingPosition.x, m_startingPosition.y - m_positionY, m_startingPosition.z), m_speed * Time.deltaTime);

                            m_LOpositionY = m_linkedObjectDistanceToEndPosition * m_percentage;
                            m_LOpositionX = m_linkedObjectDistanceToEndPosition * m_percentage;
                            //print(LOpositionY);

                            //Vector3 destination = new Vector3(m_linkedObjectStartingPosition.x - m_LOpositionX, m_linkedObjectStartingPosition.y - m_LOpositionY, m_linkedObjectStartingPosition.z);
                            Vector3 direction = m_endPosition - m_linkedObjectStartingPosition;
                            Vector3 destination = m_linkedObjectStartingPosition + direction * m_percentage;
                            m_linkedObjectPosition.position = Vector3.MoveTowards(m_linkedObjectPosition.position, destination, m_linkedObjectSpeed * Time.deltaTime);
                            //if (Vector3.Distance(m_linkedObjectPosition.position, m_endPosition/*destination*/) < 0.1f)
                            //{
                            //    if(m_audioSource)
                            //        m_audioSource.Stop();
                            //}
                            //else if (m_audioSource && !m_audioSource.isPlaying && m_percentage > 0 && m_percentage < 1)
                            //{
                            //    if (m_audioSource)
                            //        m_audioSource.Play();
                            //}
                            if (m_linkedObjectPosition.position == m_lastLOPosition)
                            {
                                if (m_audioSource)
                                    m_audioSource.Stop();
                            }
                            else if (!m_audioSource.isPlaying)
                            {
                                m_audioSource.Play();
                            }
                            m_lastLOPosition = m_linkedObjectPosition.position;

                            //m_linkedObjectPosition.position = Vector3.MoveTowards(m_linkedObjectPosition.position, destination, m_linkedObjectSpeed * Time.deltaTime);
                        }
                    }
                }
                break;
            //Once the preassure plate reaches it's end position it will be activated and won't move anymore;
            case TypeOfPreassurePlate.ButtonLike:
                {
                    if (!m_preassurePlateActivated)
                    {
                        if (m_currentWeight >= m_weightNeeded)
                        {
                            m_preassurePlateActivated = true;
                            if (m_audioSource && !m_audioSource.isPlaying)
                            {
                                //m_audioSource.Play();
                            }
                        }
                        else if (!m_activatesProjectileSpawner)
                        {
                            m_percentage = m_currentWeight / m_weightNeeded;
                            if (m_percentage > 1f)
                            {
                                m_percentage = 1f;
                            }
                            m_positionY = m_distanceToEndPosition * m_percentage;

                            Vector3 destination = new Vector3(m_linkedObjectStartingPosition.x - m_LOpositionX, m_linkedObjectStartingPosition.y - m_LOpositionY, m_linkedObjectStartingPosition.z);
                            if (Vector3.Distance(m_linkedObjectPosition.position, destination) < 0.1f && m_audioSource)
                            {
                                if (m_audioSource)
                                    m_audioSource.Stop();
                            }

                            m_parent.transform.position = Vector3.MoveTowards(m_parent.transform.position, new Vector3(m_startingPosition.x, m_startingPosition.y - m_positionY, m_startingPosition.z), m_speed * Time.deltaTime);
                        }
                    }
                    else if (!m_preassurePlateIsAtLocation || m_activatesProjectileSpawner)
                    {
                        if (Vector3.Distance(m_parent.transform.position, m_pressurePlateEndPosition.position) != 0 && !m_activatesProjectileSpawner)
                        {
                            m_parent.transform.position = Vector3.MoveTowards(m_parent.transform.position, m_pressurePlateEndPosition.position, m_speed * Time.deltaTime);
                        }
                        else
                        {
                            m_buttonActivatedObject.Activate();
                            m_preassurePlateIsAtLocation = true;
                            gameObject.SetActive(false);
                        }
                    }
                }
                break;
            case TypeOfPreassurePlate.Elevator:
                {
                    m_percentage = m_currentWeight / m_weightNeeded;
                    if (m_percentage > 1f)
                    {
                        m_percentage = 1f;
                    }
                    m_positionY = m_distanceToEndPosition * m_percentage;

                    if (m_percentage != 0)
                    {
                        if (m_preassurePlateActivated)
                        {
                            //Mover la plataforma
                            m_linkedObjectPosition.position = Vector2.MoveTowards(m_linkedObjectPosition.position, m_linkedObjectEndPosition.position, m_speed * Time.deltaTime);
                        }

                        else
                        {
                            //Mover el boton
                            m_parent.localPosition = Vector2.MoveTowards(m_parent.localPosition, new Vector2(m_parent.localPosition.x, m_startingPosition.y - m_positionY), m_speed * Time.deltaTime);
                            if ((m_startingPosition.y - m_parent.localPosition.y) >= m_distanceToEndPosition)
                            {
                                m_preassurePlateActivated = true;
                            }
                        }
                    }
                    else
                    {
                        if (m_audioSource)
                        {
                            m_audioSource.Stop();
                        }
                        m_preassurePlateActivated = false;
                        //Mover el boton a su posicion local original
                        m_parent.localPosition = Vector2.MoveTowards(m_parent.localPosition, m_startingPosition, m_speed * Time.deltaTime);
                        //Mover el ascensor a su posición original del mundo
                        m_linkedObjectPosition.position = Vector2.MoveTowards(m_linkedObjectPosition.position, m_linkedObjectStartingPosition, m_speed * Time.deltaTime);
                    }
                }
                break;
            case TypeOfPreassurePlate.Activator:
                {
                    m_percentage = m_currentWeight / m_weightNeeded;
                    if (m_percentage >= 1f)
                    {
                        m_percentage = 1f;
                        if (!m_preassurePlateActivated)
                        {
                            m_preassurePlateActivated = true;
                            m_buttonActivatedObject.Activate();
                        }
                    }
                    else
                    {
                        if (m_preassurePlateActivated)
                        {
                            m_preassurePlateActivated = false;
                            m_buttonActivatedObject.Deactivate();
                        }
                    }
                    m_positionY = m_distanceToEndPosition * m_percentage;
                    m_parent.transform.position = Vector3.MoveTowards(m_parent.transform.position, new Vector3(m_startingPosition.x, m_startingPosition.y - m_positionY, m_startingPosition.z), m_speed * Time.deltaTime);

                }
                break;
            case TypeOfPreassurePlate.None:
                {
                    print("Type of preasureplate needs to be indicated to: " + this.gameObject.name);
                }
                break;
            default:
                break;
        }
    }


    // On trigger enter kill the character that collided. 
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.LogError("Layer of GO " + collision.gameObject.name + ": " + (LayerMask.LayerToName(collision.gameObject.layer)));
        //Debug.LogError("Layer 9 of GO " + collision.gameObject.name + ": " + (collision.gameObject.layer == 1 << 9));
        if (LayerMask.LayerToName(collision.gameObject.layer) != "Player" && LayerMask.LayerToName(collision.gameObject.layer) != "Body")
            return;
        DemonBase cmpDemon = collision.GetComponentInParent<DemonBase>();
        //Debug.LogError(cmpDemon);
        if (cmpDemon != null)
        {
            //bool isCounted = false;

            //for (int i = 0; i < m_spikesData.Count; i++)
            //{
            //    //if the demon is already inside the spikes
            //    if (cmpDemon == m_spikesData[i].AssociatedDemon)
            //    {
            //        isCounted = true;

            //        //add the collider to the associated demon's collider list if it isnt already included
            //        if (!m_spikesData[i].Colliders.Contains(collision) && collision.gameObject.tag != "BodyCollider")
            //        {
            //            m_spikesData[i].Colliders.Add(collision);
            //        }
            //    }
            //}
            //if (!isCounted)
            //{
            //m_spikesData.Add(new SpikesWeightData(cmpDemon, collision));
            if (!m_enemiesOnPreassurePlate.Contains(cmpDemon))
            {
                //Debug.LogError("Added demon " + cmpDemon.name + " collider is " + collision.name);
                m_enemiesOnPreassurePlate.Add(cmpDemon);
                m_currentWeight += cmpDemon.Weight;
            }

            //}

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (LayerMask.LayerToName(collision.gameObject.layer) != "Player" && LayerMask.LayerToName(collision.gameObject.layer) != "Body")
            return;
        if (collision.GetComponentInParent<DemonBase>() != null /*&& collision.gameObject.tag != "BodyCollider"*/)
        {
            DemonBase cmpDemon = collision.GetComponentInParent<DemonBase>();

            //for (int i = 0; i < m_spikesData.Count; i++)
            //{
            //    //if the demon is already inside the spikes
            //    if (cmpDemon == m_spikesData[i].AssociatedDemon)
            //    {
            //        //remove the collider from the associated demon's collider list 
            //        if (m_spikesData[i].Colliders.Contains(collision))
            //        {
            //            m_spikesData[i].Colliders.Remove(collision);

            //            //all the limbs have exited the spikes
            //            if (m_spikesData[i].Colliders.Count == 0)
            //            {

            //                m_currentWeight -= cmpDemon.Weight;
            //                m_spikesData.RemoveAt(i);
            //                m_enemiesOnPreassurePlate.Remove(cmpDemon);
            //                cmpDemon.IsPossessionBlocked = false;
            //            }
            //            else if (m_spikesData[i].Colliders.Count == 1 && m_spikesData[i].Colliders[0].tag == "BodyCollider")
            //            {
            //                m_enemiesOnPreassurePlate.Remove(cmpDemon);
            //                m_currentWeight -= cmpDemon.Weight;
            //                m_spikesData.RemoveAt(i);
            //                cmpDemon.IsPossessionBlocked = false;
            //            }
            //        }
            //    }
            //}
            if (m_enemiesOnPreassurePlate.Contains(cmpDemon))
            {
                //Debug.LogError("Removed demon " + cmpDemon.name + " collider is " + collision.name);
                m_currentWeight -= cmpDemon.Weight;
                //m_spikesData.RemoveAt(i);
                m_enemiesOnPreassurePlate.Remove(cmpDemon);
                cmpDemon.IsPossessionBlocked = false;
            }
        }
    }

    /// <summary>
    /// Calls recurring method to calculate accumulated weight of all stacked bodies
    /// </summary>
    private void CalculateAccumulatedWeight()
    {

        float totalWeight = 0;
        for (int i = 0; i < m_enemiesOnPreassurePlate.Count; i++)
        {
            totalWeight += CalculateWeightOnTop(m_enemiesOnPreassurePlate[i]);
        }
        m_currentWeight = totalWeight;
    }

    /// <summary>
    /// Recurring method that returns the weight of all bodies on top of the specified demon
    /// </summary>
    /// <param name="demon">The demon upon which the funcion will be called again</param>
    /// <returns>The sum of the weights on top</returns>
    private float CalculateWeightOnTop(DemonBase demon)
    {
        float totalWeight = demon.Weight;

        if (m_blocksPossession)
        {
            demon.IsPossessionBlocked = true;
        }

        List<DemonBase> demonsOnTop = new List<DemonBase>();
        float distanceToCast = 0.5f;

        //cast a circle upwards to see if there is a different body on top
        RaycastHit2D[] hits = Physics2D.CircleCastAll(demon.Torso.transform.position, 0.3f, Vector2.up, distanceToCast, m_enemyLayerMask);

        for (int j = 0; j < hits.Length; j++)
        {
            //check if the collider on top is a demon
            DemonBase demonOnTop = hits[j].transform.GetComponentInParent<DemonBase>();
            if (demonOnTop != null)
            {
                //add it to a list of demons that are on top of this one, making sure not to add it twice
                if (demonOnTop != demon && !demonsOnTop.Contains(demonOnTop) && !m_enemiesOnPreassurePlate.Contains(demonOnTop))
                {
                    demonsOnTop.Add(demonOnTop);

                    //block possession of the demons on top if it should be done
                    if (m_blocksPossession)
                    {
                        demonOnTop.IsPossessionBlocked = true;
                    }
                }
            }
        }
        //recurring method for the demons on top of this one
        for (int i = 0; i < demonsOnTop.Count; i++)
        {
            totalWeight += CalculateWeightOnTop(demonsOnTop[i]);
        }
        return totalWeight;
    }
}

