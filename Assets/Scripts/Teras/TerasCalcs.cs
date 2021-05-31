using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerasCalcs
{
    #region data

    // main teras that is used for fighting
    public Teras _baseTeras { get; set; }

    // teras level
    public int level { get; set; }

    // current health to calculate when it will go to zero
    public int Health { get; set; }

    // skills that are known by teras
    public List<SkillCalc> Skills { get; set; }

    #endregion

    // Initialize teras, level, current health
    public TerasCalcs(Teras tBase,int tLevel)
    {
        _baseTeras = tBase;
        level = tLevel;
        Health = CalculateMaxHealthStat;
        
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

    public DamageDetails TakeDamage(SkillCalc skill, TerasCalcs attacker)
    {
        float critical = 1f;
        if (Random.value * 100f <= 6.25)
            critical = 2f;

        float element = ElementEffectiveness.GetEffectiveness(skill.baseSkill.SkillElement, attacker._baseTeras.FirstElement) * ElementEffectiveness.GetEffectiveness(skill.baseSkill.SkillElement, attacker._baseTeras.SecondElement);

        var DMG_Details = new DamageDetails()
        {
            Critical = critical,
            ElementEffectiveness = element,
            Fainted = false
        };

        float modifiers = 1 * element * critical;
        float a = (2 * attacker.level + 10) / 250f;
        float d = a * skill.baseSkill.Damage * ((float)attacker._baseTeras.Attack / this._baseTeras.Defense);
        int damage = Mathf.FloorToInt(d * modifiers);

        Health -= damage;
        if(Health <= 0)
        {
            Health = 0;
            DMG_Details.Fainted = true;
        }
        return DMG_Details;
    }

    public SkillCalc RandomSkill()
    {
        int r = Random.Range(0, Skills.Count);
        return Skills[r];
    }
}

public class DamageDetails
{
    public bool Fainted { get; set; }
    public float Critical { get; set; }
    public float ElementEffectiveness { get; set; }
}