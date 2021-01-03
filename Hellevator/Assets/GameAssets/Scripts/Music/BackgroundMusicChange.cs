using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusicChange : MonoBehaviour
{
    [SerializeField] bool m_changesToKitchen;
    private void OnEnable()
    {
        if (m_changesToKitchen)
            AudioManager.Instance.SetBackgroundMusicToKitchen();
        else
            AudioManager.Instance.SetBackgroundMusicToRestaurant();

    }
}
