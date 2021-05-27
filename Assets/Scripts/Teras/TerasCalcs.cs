using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerasCalcs
{
    Teras _baseTeras;
    int level;

    public TerasCalcs(Teras tBase,int tLevel)
    {
        _baseTeras = tBase;
        level = tLevel;
    }

    public int CalculateAttackStat
    {
        get{ return Mathf.FloorToInt((_baseTeras.Attack * level) / 30f) + 2; }
    }
    public int CalculateDefenseStat
    {
        get { return Mathf.FloorToInt((_baseTeras.Defense * level) / 30f) + 2; }
    }
    public int CalculateSpeedStat
    {
        get { return Mathf.FloorToInt((_baseTeras.Speed * level) / 30f) + 2; }
    }
    public int CalculateMaxHealthStat
    {
        get { return Mathf.FloorToInt((_baseTeras.Health * level) / 30f) + 5; }
    }
}
