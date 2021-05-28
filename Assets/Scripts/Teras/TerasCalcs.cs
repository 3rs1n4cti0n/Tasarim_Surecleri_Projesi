using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerasCalcs
{
    Teras _baseTeras;
    int level;

    public int CurrentHealth { get; set; }
    public List<SkillCalc> Skills { get; set; }

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

            if (Skills.Count >= 4)
                break;
        }
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
