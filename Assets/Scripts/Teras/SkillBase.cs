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
    #endregion
}
