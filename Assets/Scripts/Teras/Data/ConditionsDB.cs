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
            ConditionID.burn,
            new Condition(){
                Name="Burn",
                StatMessage = "has been burned.",
                OnAfterTurn = (TerasCalcs teras) =>
                {
                    teras.UpdateHP(teras.CalculateMaxHealthStat / 16);
                    teras.StatusChanges.Enqueue($"{teras._baseTeras.Name} took damage due to burn!");
                }

            }
        },
        {
            ConditionID.paralysis,
            new Condition(){
                Name="Paralysis",
                StatMessage = "has been paralized.",
                OnPerformSkill = (TerasCalcs teras) =>
                {
                    if(Random.Range(1,5) == 1)
                    {
                        teras.StatusChanges.Enqueue($"{teras._baseTeras.Name} can't move due to paralysis!");
                        return false;
                    }
                    return true;
                }
            }
        },
        {
            ConditionID.sleep,
            new Condition(){
                Name="Sleep",
                StatMessage = "has been put to sleep.",
                OnStart = (TerasCalcs teras) =>
                {
                    teras.StatusTurns = Random.Range(1,4);
                    Debug.Log($"Sleep turns: {teras.StatusTurns}");
                },

                OnPerformSkill = (TerasCalcs teras) =>
                {
                    if(teras.StatusTurns <= 0)
                    {
                        teras.CureStatusCondition();
                        teras.StatusChanges.Enqueue($"{teras._baseTeras.Name} woke up!");
                    }
                    else
                    {
                        teras.StatusTurns--;
                        teras.StatusChanges.Enqueue($"{teras._baseTeras.Name} is sleeping!");
                    }

                    return false;
                }
            }
        },
    };
}

public enum ConditionID
{
    none,poison,burn,sleep,paralysis
}