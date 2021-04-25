using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusicChange : MonoBehaviour
{

    private enum MusicToChangeTo
    {
        Menu, Storage, Restaurant, Kitchen
    }

    [SerializeField] MusicToChangeTo m_newMusic = MusicToChangeTo.Restaurant;
    //[SerializeField] bool m_changesToKitchen;
    private void Start()
    {
        if (m_newMusic == MusicToChangeTo.Kitchen)
        {
            AudioManager.Instance.SetBackgroundMusicToKitchen();
        }
        else if (m_newMusic == MusicToChangeTo.Restaurant)
        {
            AudioManager.Instance.SetBackgroundMusicToRestaurant();
        }
        else if (m_newMusic == MusicToChangeTo.Menu)
        {
            AudioManager.Instance.SetBackgroundMusicToMenu();
        }
        else if (m_newMusic == MusicToChangeTo.Storage)
        {
            AudioManager.Instance.SetBackgroundMusicToStorage();
        }

    }
}
