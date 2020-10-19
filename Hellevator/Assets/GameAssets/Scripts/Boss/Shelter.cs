using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Shelter : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Boss.Instance.SetSeeingPlayer();           
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Boss.Instance.SetNotSeeingPlayer();
    }
}
