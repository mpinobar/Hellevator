using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    List<DemonBase> demonsInside;

    private void Start()
    {
        demonsInside = new List<DemonBase>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<BasicZombie>().TryingToGrabLadder)
        {
            collision.GetComponent<BasicZombie>().SetOnLadder(true);
        }
        demonsInside.Add(collision.GetComponent<BasicZombie>());
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        demonsInside.Remove(collision.GetComponent<BasicZombie>());
        collision.GetComponent<BasicZombie>().SetOnLadder(false);
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.GetComponent<BasicZombie>().TryingToGrabLadder)
        {
            collision.GetComponent<BasicZombie>().SetOnLadder(true);
        }     
    }
}
