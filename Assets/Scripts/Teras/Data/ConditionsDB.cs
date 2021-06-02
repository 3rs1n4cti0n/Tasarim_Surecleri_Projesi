using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionsDB
{
    public static Dictionary<ConditionID, Condition> Conditions { get; set; } = new Dictionary<ConditionID, Condition>()
    {
        {
            ConditionID.poison, new Condition()
        {
            Name="Poison",
            StatMessage = "has been poisoned."
        }},
        {
            ConditionID.none, new Condition()
        {
            Name="",
            StatMessage = ""
        }}
    };
}

public enum ConditionID
{
    none,poison,burn,sleep,paralysis
}