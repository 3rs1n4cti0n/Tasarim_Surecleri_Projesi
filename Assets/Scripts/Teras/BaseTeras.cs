using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Teras", menuName ="Teras/New Teras")]
public class Teras : ScriptableObject
{
    [SerializeField] string name;
    
    [TextArea]
    [SerializeField] string description;

    [SerializeField] Sprite frontSprite;
    [SerializeField] Sprite backSprite;

    [SerializeField] elements firstElement;
    [SerializeField] elements secondElement;

    // base stats
    [SerializeField] int health;
    [SerializeField] int attack;
    [SerializeField] int defense;
    [SerializeField] int speed;

    [SerializeField] List<SkillSet> skillSets;

    public string Name
    {
        get { return name; }
    }
    public string Description
    {
        get { return description; }
    }
    public Sprite FrontSprite
    {
        get { return frontSprite; }
    }
    public Sprite BackSprite
    {
        get { return backSprite; }
    }
    public elements FirstElement
    {
        get { return firstElement; }
    }
    public elements SecondElement
    {
        get { return secondElement; }
    }
    public int Health
    {
        get { return health; }
    }
    public int Attack
    {
        get { return attack; }
    }
    public int Defense
    {
        get { return defense; }
    }
    public int Speed
    {
        get { return speed; }
    }
    public List<SkillSet> SkillSets
    {
        get { return skillSets; }
    }
}

[System.Serializable]
public class SkillSet
{
    [SerializeField] SkillBase skillBase;
    [SerializeField] int level;

    public SkillBase SkillBase
    {
        get { return skillBase; }
    }
    public int Level
    {
        get { return level; }
    }
}
public enum elements
{
    None,
    Fire,
    Water,
    Electric,
    Air,
    Grass,
    Light,
    Dark
}