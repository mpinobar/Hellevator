using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
        public GameObject camera;


        private void OnTriggerEnter2D(Collider2D collision)
        {
            camera.SetActive(true);
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            camera.SetActive(false);
        }
    
}
