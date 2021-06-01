using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TerasCalcs
{
    #region data

    [SerializeField] Teras _base;
    [SerializeField] int level;

    // main teras that is used for fighting
    public Teras _baseTeras {
        get
        {
            return _base;
        }
    }

    // teras level
    public int Level {
        get
        {
            return level;
        }
    }

    // current health to calculate when it will go to zero
    public int Health { get; set; }

    // skills that are known by teras
    public List<SkillCalc> Skills { get; set; }

    #endregion

    // Initialize teras, level, current health
    public void Init()
    {
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

    #region LevelStatCalculations
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

    // Function to take damage
    public DamageDetails TakeDamage(SkillCalc skill, TerasCalcs attacker)
    {
        // check for critical hit
        float critical = 1f;
        if (Random.value * 100f <= 6.25)
            critical = 2f;

        // check element effectiveness
        float element = ElementEffectiveness.GetEffectiveness(skill.baseSkill.SkillElement, attacker._baseTeras.FirstElement) * ElementEffectiveness.GetEffectiveness(skill.baseSkill.SkillElement, attacker._baseTeras.SecondElement);

        // set details of damage
        var DMG_Details = new DamageDetails()
        {
            Critical = critical,
            ElementEffectiveness = element,
            Fainted = false
        };

        // calculate damage
        float modifiers = 1 * element * critical;
        float a = (2 * attacker.level + 10) / 250f;
        float d = a * skill.baseSkill.Damage * ((float)attacker._baseTeras.Attack / _baseTeras.Defense);
        int damage = Mathf.FloorToInt(d * modifiers);

        // substract health
        Health -= damage;

        // check if the teras fainted
        if(Health <= 0)
        {
            Health = 0;
            DMG_Details.Fainted = true;
        }
        return DMG_Details;
    }
    // uses a random skill
    public SkillCalc RandomSkill()
    {
        // return random index of skill
        int r = Random.Range(0, Skills.Count);
        return Skills[r];
    }
}

// class to check the details of damage
public class DamageDetails
{
    public bool Fainted { get; set; }
    public float Critical { get; set; }
    public float ElementEffectiveness { get; set; }
}