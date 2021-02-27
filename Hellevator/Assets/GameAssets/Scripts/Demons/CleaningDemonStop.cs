using UnityEngine;

public class CleaningDemonStop: MonoBehaviour
{


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<DestructionCart>())
        {
            //Debug.LogError("WALL STOPS CLEANING DEMON");
            collision.GetComponentInParent<CleaningDemon>().Stop();
        }
    }

}
