using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSpawner : MonoBehaviour
{

    [SerializeField] GameObject projectileToShoot;
    [SerializeField] float initialWaitTimeBeforeShooting = 0f;
    [SerializeField] float timeIntervalBetweenShots = 2f;
    [SerializeField] float projectileSpeed = 4f;

    int maxNumberOfProjectiles = 5;
    List<GameObject> projectilePool;

    // Start is called before the first frame update
    void Start()
    {
        projectilePool = new List<GameObject>();
        for (int i = 0; i < maxNumberOfProjectiles; i++)
        {
            projectilePool.Add(Instantiate(projectileToShoot, transform.position, Quaternion.identity, transform));
            projectilePool[i].SetActive(false);
        }

        InvokeRepeating(nameof(ShootProjectile), initialWaitTimeBeforeShooting, timeIntervalBetweenShots);
    }

    private void ShootProjectile()
    {
        for (int i = 0; i < projectilePool.Count; i++)
        {
            if (!projectilePool[i].activeSelf)
            {
                projectilePool[i].SetActive(true);
                projectilePool[i].GetComponent<Projectile>().Speed = projectileSpeed;
                return;
            }
        }
        GameObject newProjectile = Instantiate(projectileToShoot, transform.position, Quaternion.identity,transform);
        projectilePool.Add(newProjectile);
        newProjectile.GetComponent<Projectile>().Speed = projectileSpeed;
    }

}
