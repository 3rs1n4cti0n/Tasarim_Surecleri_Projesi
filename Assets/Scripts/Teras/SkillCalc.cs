using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillCalc
{
    public SkillBase baseSkill { get; set; }
    public int UseLeft { get; set; }

    // initialize store data
    public SkillCalc(SkillBase tBase)
    {
        baseSkill = tBase;
        UseLeft = tBase.UseLeft;
    }
}
