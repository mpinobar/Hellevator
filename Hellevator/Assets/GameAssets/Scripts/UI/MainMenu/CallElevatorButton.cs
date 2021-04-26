using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallElevatorButton : MonoBehaviour
{
    bool active;
    [SerializeField] AudioClip m_buttonSoundClip;



    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit r;
        if (Physics.Raycast(ray, out r))
        {
            if (r.transform.GetComponent<CallElevatorButton>() == this)
            {
                Highlight();
                if (Input.GetMouseButtonDown(0))
                {
                    CallElevator();
                }
            }
        }
        else
        {
            HideHighlight();
        }
        //if (Input.GetMouseButtonDown(0))
        //{

        //    //RaycastHit r;
        //    if (Physics.Raycast(ray, out r))
        //    {
        //        if (r.transform.GetComponent<CallElevatorButton>() == this)
        //        {
        //            CallElevator();
        //        }
        //    }
        //}
    }

    public void Highlight()
    {

    }
    public void HideHighlight()
    {

    }
    public void CallElevator()
    {
        if (!active)
        {
            active = true;
            AudioManager.Instance.PlayAudioSFX(m_buttonSoundClip, false);
            //transform.GetChild(0).gameObject.SetActive(false);
            FindObjectOfType<IntroCanvas>().CallElevator();
        }
    }
}
