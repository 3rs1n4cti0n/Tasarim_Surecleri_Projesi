using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerasCalcs
{
    #region data

    // main teras that is used for fighting
    Teras _baseTeras;

    // teras level
    int level;

    // current health to calculate when it will go to zero
    public int CurrentHealth { get; set; }

    // skills that are known by teras
    public List<SkillCalc> Skills { get; set; }

    #endregion

    // Initialize teras, level, current health
    public TerasCalcs(Teras tBase,int tLevel)
    {
        _baseTeras = tBase;
        level = tLevel;
        CurrentHealth = _baseTeras.Health;
        
        Skills = new List<SkillCalc>();
        foreach(var skill in _baseTeras.SkillSets)
        {
            if (skill.Level <= level)
                Skills.Add(new SkillCalc(skill.SkillBase));

            // break if skill count is greater then 4
            if (Skills.Count >= 4)
                break;
        }
    }

    #region Calculations
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
    #endregion
}
