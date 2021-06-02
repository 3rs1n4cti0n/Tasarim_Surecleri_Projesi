using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Teras", menuName = "Teras/New Teras")]
public class Teras : ScriptableObject
{
    #region data
    /* SerializeField makes it so that we can see the data in the inspector */

    [SerializeField] string name;

    [TextArea]
    [SerializeField] string description;

    //Sprites to keep front and back sie of teras
    [SerializeField] Sprite frontSprite;
    [SerializeField] Sprite backSprite;

    // enumarator to keep track of teras elements
    [SerializeField] elements firstElement;
    [SerializeField] elements secondElement;

    // base stats
    [SerializeField] int health;
    [SerializeField] int attack;
    [SerializeField] int defense;
    [SerializeField] int speed;

    // Every learnable skill a teras can have
    [SerializeField] List<SkillSet> skillSets;

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
    #endregion
}

// class to hold a skills learn level
[System.Serializable]
public class SkillSet
{
    [SerializeField] SkillBase skillBase;
    [SerializeField] int level;

    // getters
    public SkillBase SkillBase
    {
        get { return skillBase; }
    }
    public int Level
    {
        get { return level; }
    }
}

// enumarator for types
public enum elements
{
    None,
    Fire,
    Water,
    Electric,
    Air,
    Earth,
    Light,
    Dark
}

public class ElementEffectiveness
{
    static float[][] chart =
    {
                                      //FIRE,WATER,ELECT,AIR,  EARTH,LIGHT,DARK
        /*FIRE*/        new float [] { 1f,   0.5f, 1f,   1f,   2f,   1f,   1f},
        /*WATER*/       new float [] { 2f,   1f,   0.5f, 1f,   1f,   1f,   1f},
        /*ELECTRIC*/    new float [] { 1f,   2f,   1f,   0.5f, 1f,   1f,   1f},
        /*AIR*/         new float [] { 1f,   1f,   2f,   1f,   0.5f, 1f,   1f},
        /*EARTH*/       new float [] { 0.5f, 1f,   1f,   2f,   1f,   1f,   1f},
        /*LIGHT*/       new float [] { 2f,   2f,   2f,   2f,   2f,   1f,   1f},
        /*DARK*/        new float [] { 2f,   2f,   2f,   2f,   2f,   1f,   1f},
    };

    // gets row and col and returns value in table chart for multiplication
    public static float GetEffectiveness(elements ATK_Element,elements DEF_Element)
    {
        // return neutral multiplier if there is no element on the skill
        // or the defender does not have an element
        if (ATK_Element == elements.None || DEF_Element == elements.None)
            return 1;

        // get row and column
        int row = (int)ATK_Element - 1;
        int col = (int)DEF_Element - 1;

        // debug to check if it returns correct multiplier
        Debug.Log(row);
        Debug.Log(col);

        return chart[row][col];
    }
}

public enum Stat
{
    attack,
    defense,
    speed
}