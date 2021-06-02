using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TerasCalcs
{
    #region data

    [SerializeField] Teras _base;
    [SerializeField] int level;

    public Dictionary<Stat, int> Stats { get; private set; }
    public Dictionary<Stat, int> BoostStats { get; private set; }
    public Queue<string> StatusChanges { get; private set; } = new Queue<string>();
    public Condition Status { get; private set; }

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
        
        Skills = new List<SkillCalc>();
        foreach(var skill in _baseTeras.SkillSets)
        {
            if (skill.Level <= level)
                Skills.Add(new SkillCalc(skill.SkillBase));

            // break if skill count is greater then 4
            if (Skills.Count >= 4)
                break;
        }
        StatCalculate();

        Health = CalculateMaxHealthStat;
        if (Status == null)
            Status = ConditionsDB.Conditions[ConditionID.none];
        ResetStatBoost();
    }

    void ResetStatBoost()
    {
        BoostStats = new Dictionary<Stat, int>()
        {
            {Stat.attack, 0},
            {Stat.defense, 0},
            {Stat.speed, 0}
        };
    }
    void StatCalculate()
    {
        Stats = new Dictionary<Stat, int>();
        Stats.Add(Stat.attack, Mathf.FloorToInt((_baseTeras.Attack * level) / 30f) + 2);
        Stats.Add(Stat.defense, Mathf.FloorToInt((_baseTeras.Defense * level) / 30f) + 2);
        Stats.Add(Stat.speed, Mathf.FloorToInt((_baseTeras.Speed * level) / 30f) + 2);

        CalculateMaxHealthStat = Mathf.FloorToInt((_baseTeras.Health * level) / 30f) + 5;
    }

    int getStat(Stat stat)
    {
        int statValue = Stats[stat];

        int boost = BoostStats[stat];
        var BoosAmount = new float[] { 1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f };

        if (boost >= 0)
            statValue = Mathf.FloorToInt(statValue * BoosAmount[boost]);
        else
            statValue = Mathf.FloorToInt(statValue / BoosAmount[-boost]);

        return statValue;
    }

    public void ApplyBoost(List<StatBoost> statBoosts)
    {
        foreach (var statboost in statBoosts)
        {
            var stat = statboost.stat;
            var boost = statboost.boost;

            BoostStats[stat] = BoostStats[stat] + boost;
            BoostStats[stat] = Mathf.Clamp(BoostStats[stat] + boost, -6, 6);

            if (boost > 0)
                StatusChanges.Enqueue($"{_baseTeras.Name}'s {stat} rose!");
            else
                StatusChanges.Enqueue($"{_baseTeras.Name}'s {stat} fell!");
        }
    }

    #region LevelStatCalculations
    public int CalculateAttackStat
    {
        get{ return getStat(Stat.attack); }
    }
    public int CalculateDefenseStat
    {
        get { return getStat(Stat.defense); }
    }
    public int CalculateSpeedStat
    {
        get { return getStat(Stat.speed); }
    }
    public int CalculateMaxHealthStat
    {
        get; private set;
    }
    #endregion
    public void SetStatus(ConditionID conditionId)
    {
        Status = ConditionsDB.Conditions[conditionId];
        StatusChanges.Enqueue($"{_baseTeras.Name} {Status.StatMessage}!");
    }
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
        this.Health -= damage;

        // check if the teras fainted
        if(Health <= 0)
        {
            this.Health = 0;
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

    public void OnBattleOver()
    {
        ResetStatBoost();
    }
}

// class to check the details of damage
public class DamageDetails
{
    public bool Fainted { get; set; }
    public float Critical { get; set; }
    public float ElementEffectiveness { get; set; }
}