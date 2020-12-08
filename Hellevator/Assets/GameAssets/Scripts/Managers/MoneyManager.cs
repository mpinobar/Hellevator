using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyManager
{
    static int m_currentMoney;

    public static int CurrentMoney
    {
        get
        {
            m_currentMoney = PlayerPrefs.GetInt("Money");
            return m_currentMoney;
        }
        set
        {
            m_currentMoney = value;
            PlayerPrefs.SetInt("Money", m_currentMoney);
        }
    }

    public static void AddMoney(int numberToAdd)
    {
        CurrentMoney += numberToAdd;
    }

    public static bool CheckEnoughToSpend(int cost)
    {
        return cost <= CurrentMoney;
    }

    public static int GetMoney()
    {
        return CurrentMoney;
    }

    public static void SpendMoney(int cost)
    {
        if (CheckEnoughToSpend(cost))
        {
            CurrentMoney -= cost;
        }
    }
}
