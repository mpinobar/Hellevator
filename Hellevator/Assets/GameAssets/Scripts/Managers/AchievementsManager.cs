using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
public class AchievementsManager
{
    static int deathNumber;
    static bool killedBeelzebub;
    static bool killedGardenKeeper;
    static bool killedSatan;
    static int collectibles;

    static string ach_first_death = "ach_first_death";
    static string ach_100_deaths = "ach_100_deaths";
    static string ach_killed_beelzebub = "ach_killed_bz";
    static string ach_kiled_garden_keeper = "ach_killed_gk";
    static string ach_killed_satan = "ach_killed_satan";
    static string ach_all_collectibles = "ach_all_collectibles";

    public static void Initialize()
    {
        deathNumber = PlayerPrefs.GetInt("deathNumber");
        collectibles = PlayerPrefs.GetInt("collectibles");
        killedBeelzebub = PlayerPrefs.GetInt("killedbz") == 1;
        killedGardenKeeper = PlayerPrefs.GetInt("killedgk") == 1;
        killedSatan = PlayerPrefs.GetInt("killedsatan") == 1;
    }

    public static void AddCollectible()
    {
        collectibles++;
        if (collectibles >= 6)
        {
            if (PlayerPrefs.GetInt(ach_all_collectibles) == 0)
            {
                PlayerPrefs.SetInt(ach_all_collectibles, 1);
                UnlockSteamAchievement(ach_all_collectibles);
            }
        }
    }

    public static void AddDeath()
    {
        deathNumber++;
        if (deathNumber > 0)
        {
            if (PlayerPrefs.GetInt(ach_first_death) == 1)
            {
                PlayerPrefs.SetInt(ach_first_death, 1);
                //call steam to unlock first death ach
                UnlockSteamAchievement(ach_first_death);
            }
        }
        if (deathNumber >= 100)
        {
            if (PlayerPrefs.GetInt(ach_100_deaths) == 1)
            {
                PlayerPrefs.SetInt(ach_100_deaths, 1);
                //call steam to unlock 100 death ach
                UnlockSteamAchievement(ach_100_deaths);
            }
        }
        Save();
    }

    public static void UnlockKilledBz()
    {
        killedBeelzebub = true;
        UnlockSteamAchievement(ach_killed_beelzebub);        
    }

    public static void UnlockKilledGK()
    {
        killedGardenKeeper = true;
        UnlockSteamAchievement(ach_kiled_garden_keeper);        
    }

    public static void UnlockKilledSatan()
    {
        killedSatan = true;
        UnlockSteamAchievement(ach_killed_satan);        
    }

    public static void UnlockSteamAchievement(string id)
    {
        bool hasAchievement;
        SteamUserStats.GetAchievement(id, out hasAchievement);
        if (!hasAchievement)
        {
            SteamUserStats.SetAchievement(id);            
        }
        Save();
    }

    public static void Save()
    {
        PlayerPrefs.SetInt("deathNumber", deathNumber);
        PlayerPrefs.SetInt("collectibles", collectibles);
        PlayerPrefs.SetInt("killedbz", killedBeelzebub ? 1 : 0);
        PlayerPrefs.SetInt("killedgk", killedGardenKeeper ? 1 : 0);
        PlayerPrefs.SetInt("killedsatan", killedSatan ? 1 : 0);
        SteamUserStats.StoreStats();
    }
}
