using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionsDB
{
    public static Dictionary<ConditionID, Condition> Conditions { get; set; } = new Dictionary<ConditionID, Condition>()
    {
        {
            ConditionID.poison, 
            new Condition(){
                Name="Poison",
                StatMessage = "has been poisoned.",
                OnAfterTurn = (TerasCalcs teras) =>
                {
                    teras.UpdateHP(teras.CalculateMaxHealthStat / 8);
                    teras.StatusChanges.Enqueue($"{teras._baseTeras.Name} took damage due to poison!");
                }
            
            }
        },
        {
            ConditionID.none, 
            new Condition()
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