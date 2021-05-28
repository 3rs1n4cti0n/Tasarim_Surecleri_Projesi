using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Skill", menuName ="Teras/New Skill")]
public class SkillBase : ScriptableObject
{
    [SerializeField] string name;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] elements skillElement;
    [SerializeField] int damage;
    [SerializeField] int accuracy;
    [SerializeField] int useLeft;
    private SkillBase skillBase;

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
}
