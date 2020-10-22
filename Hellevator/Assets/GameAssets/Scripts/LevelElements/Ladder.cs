using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour
{
   
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<BasicZombie>().TryingToGrabLadder)
        {
            collision.GetComponent<BasicZombie>().SetOnLadder(true);
            collision.GetComponent<BasicZombie>().ResetVelocity();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
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
