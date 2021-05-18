using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
public class AchievementsManager : PersistentSingleton<AchievementsManager>
{

     int deathNumber;
     bool killedBeelzebub;
     bool killedGardenKeeper;
     bool killedSatan;
     int collectibles;

     string ach_first_death = "ach_first_death";
     string ach_100_deaths = "ach_100_deaths";
     string ach_killed_beelzebub = "ach_killed_bz";
     string ach_kiled_garden_keeper = "ach_killed_gk";
     string ach_killed_satan = "ach_killed_satan";
     string ach_all_collectibles = "ach_all_collectibles";

     bool init;

    public override void Awake()
    {
        base.Awake();
        Initialize();
    }

    public void ClearAchievements()
    {
        deathNumber = 0;
        collectibles = 0;
        killedBeelzebub = false;
        killedGardenKeeper = false;
        killedSatan = false;
        PlayerPrefs.SetInt("deathNumber", 0);
        PlayerPrefs.SetInt("collectibles", 0);
        PlayerPrefs.SetInt("killedbz", 0);
        PlayerPrefs.SetInt("killedgk", 0);
        PlayerPrefs.SetInt("killedsatan", 0);
        SteamUserStats.ClearAchievement("ach_first_death");
        SteamUserStats.ClearAchievement("ach_100_deaths");
        SteamUserStats.ClearAchievement("ach_killed_bz");
        SteamUserStats.ClearAchievement("ach_killed_gk");
        SteamUserStats.ClearAchievement("ach_killed_satan");
        SteamUserStats.ClearAchievement("ach_all_collectibles");
        Save();

    }
    public void Initialize()
    {
        if (!init)
        {
            deathNumber = PlayerPrefs.GetInt("deathNumber");
            collectibles = PlayerPrefs.GetInt("collectibles");
            killedBeelzebub = PlayerPrefs.GetInt("killedbz") == 1;
            killedGardenKeeper = PlayerPrefs.GetInt("killedgk") == 1;
            killedSatan = PlayerPrefs.GetInt("killedsatan") == 1;
            init = true;
        }
    }

    public void AddCollectible()
    {
        Initialize();
        collectibles++;
        if (collectibles >= 9)
        {
            if (PlayerPrefs.GetInt(ach_all_collectibles) == 0)
            {
                PlayerPrefs.SetInt(ach_all_collectibles, 1);
                UnlockSteamAchievement(ach_all_collectibles);
            }
        }
        Save();
    }

    public void AddDeath()
    {
        Initialize();
        //Debug.LogError("-1" + ach_first_death);
        deathNumber++;
        if (deathNumber > 0)
        {
            //Debug.LogError("0" + ach_first_death);
            PlayerPrefs.SetInt(ach_first_death, 1);
            //call steam to unlock first death ach
            UnlockSteamAchievement(ach_first_death);

        }
        if (deathNumber >= 100)
        {

            PlayerPrefs.SetInt(ach_100_deaths, 1);
            //call steam to unlock 100 death ach
            UnlockSteamAchievement(ach_100_deaths);

        }
        Save();
    }

    public void UnlockKilledBz()
    {
        killedBeelzebub = true;
        UnlockSteamAchievement(ach_killed_beelzebub);
    }

    public void UnlockKilledGK()
    {
        killedGardenKeeper = true;
        UnlockSteamAchievement(ach_kiled_garden_keeper);
    }

    public void UnlockKilledSatan()
    {
        killedSatan = true;
        UnlockSteamAchievement(ach_killed_satan);
    }

    public void UnlockSteamAchievement(string id)
    {
        Initialize();
        //Debug.LogError("1" + ach_first_death);
        if (SteamManager.Initialized)
        {
            bool hasAchievement;
            SteamUserStats.GetAchievement(id, out hasAchievement);
            //Debug.LogError("2" + ach_first_death);
            if (!hasAchievement)
            {
                //Debug.LogError("3" + ach_first_death);
                SteamUserStats.SetAchievement(id);

            }
            Save();
        }
    }

    public void Save()
    {
        PlayerPrefs.SetInt("deathNumber", deathNumber);
        PlayerPrefs.SetInt("collectibles", collectibles);
        PlayerPrefs.SetInt("killedbz", killedBeelzebub ? 1 : 0);
        PlayerPrefs.SetInt("killedgk", killedGardenKeeper ? 1 : 0);
        PlayerPrefs.SetInt("killedsatan", killedSatan ? 1 : 0);

        if (SteamManager.Initialized)
            SteamUserStats.StoreStats();
    }

}
