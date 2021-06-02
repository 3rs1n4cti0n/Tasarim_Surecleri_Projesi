using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Skill", menuName ="Teras/New Skill")]
public class SkillBase : ScriptableObject
{
    #region data
    /* SerializeField makes it so that we can see the data in the inspector */

    [SerializeField] string name;

    [TextArea]
    [SerializeField] string description;

    // keep track of what element is the skill
    [SerializeField] elements skillElement;

    // how much damage does it do
    [SerializeField] int damage;

    // how accurate it is
    [SerializeField] int accuracy;

    // how many times it can be used
    [SerializeField] int useLeft;

    [SerializeField] SkillCategory category;

    [SerializeField] SkillEffects effects;
    [SerializeField] SkillTarget skillTarget;

    #endregion

    #region getters
    public string Name
    {
        get { return name; }
    }
    public string Description
    {
        get { return description; }
    }
    public elements SkillElement
    {
        get { return skillElement; }
    }
    public int Damage
    {
        get { return damage; }
    }
    public int Accuracy
    {
        get { return accuracy; }
    }
    public int UseLeft
    {
        get { return useLeft; }
    }
    public SkillCategory Category
    {
        get { return category; }
    }
    public SkillTarget SkillTarget
    {
        get { return skillTarget; }
    }
    public SkillEffects Effects
    {
        get { return effects; }
    }
    #endregion
}

[System.Serializable]
public class SkillEffects
{
    [SerializeField] List<StatBoost> boosts;
    [SerializeField] ConditionID status;
    public List<StatBoost> Boosts
    {
        get { return boosts; }
    }

    public ConditionID Status
    {
        get { return status; }
    }
}

[System.Serializable]
public class StatBoost
{
    public Stat stat;
    public int boost;
}

public enum SkillCategory
{
    Damaging,
    Status
}

public enum SkillTarget
{
    self,
    target
}
