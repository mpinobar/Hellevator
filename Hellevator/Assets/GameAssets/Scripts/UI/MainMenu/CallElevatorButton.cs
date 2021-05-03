using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallElevatorButton : MonoBehaviour
{
    bool active;
    [SerializeField] AudioClip m_buttonSoundClip;

    [SerializeField] GameObject highlight;
    bool counting;
    float time;

    private void Start()
    {
        IntroCanvas.OnBegin += () => counting = true;
    }

    private void OnDisable()
    {
        IntroCanvas.OnBegin -= () => counting = true;
    }
    private void Update()
    {
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit r))
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


        if (counting)
        {
            time += Time.deltaTime;
            if (time >= 4f)
                if (Input.anyKeyDown)
                    CallElevator();
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
        if (!highlight.activeSelf)
            highlight.SetActive(true);
    }
    public void HideHighlight()
    {
        highlight.SetActive(false);
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
