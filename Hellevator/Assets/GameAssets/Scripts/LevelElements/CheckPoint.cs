using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Collider2D))]
public class CheckPoint : MonoBehaviour
{

    [SerializeField] private DemonBase demonToSpawn;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.transform.root.GetComponent<DemonBase>() == PosesionManager.Instance.ControlledDemon)
        {
            LevelManager.Instance.SetLastCheckPoint(this);
        }
    }

    /// <summary>
    /// Spawns player at checkpoint location, summoning and possessing the instantiated type of demon
    /// </summary>
    public void SpawnPlayer()
    {
        DemonBase spawnedDemon = Instantiate(demonToSpawn, transform.position, Quaternion.identity);
        spawnedDemon.enabled = true;
        spawnedDemon.SetControlledByPlayer();
        CameraManager.Instance.ChangeCamTarget();
        InputManager.Instance.UpdateDemonReference();
    }
}
