using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyActivatedDoor : MonoBehaviour
{
    [SerializeField] Key key;       

    private void OnTriggerEnter2D(Collider2D collision)
    {        
        if (collision.GetComponentInParent<DemonBase>())
        {
            CheckOpenDoor();
        }
    }

    private void OnEnable()
    {
        CheckOpenDoor();
    }

    private void CheckOpenDoor()
    {
        if (PlayerPrefs.GetInt(key.ToString()) == 1 /*|| LevelManager.Instance.HasKitchenKey*/)
        {
            OpenDoor();
        }
    }

    public void OpenDoor()
    {
        gameObject.SetActive(false);
    }

}
