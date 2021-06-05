using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionsDB
{
    public static void Init()
    {
        foreach(var kvp in Conditions)
        {
            var conditionID = kvp.Key;
            var condition = kvp.Value;

            condition.Id = conditionID;
        }
    }

    public static Dictionary<ConditionID, Condition> Conditions { get; set; } = new Dictionary<ConditionID, Condition>()
    {
        {
            ConditionID.poison,
            new Condition(){
                Name="Poison",
                StatMessage = "has been poisoned!",
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
        {
            ConditionID.confusion,
            new Condition(){
                Name="Confusion",
                StatMessage = "has been confused.",
                OnStart = (TerasCalcs teras) =>
                {
                    teras.VolotileStatusTime = Random.Range(1,4);
                    Debug.Log($"Will be confused for: {teras.VolotileStatusTime}");
                },

                OnPerformSkill = (TerasCalcs teras) =>
                {
                    if(teras.VolotileStatusTime <= 0)
                    {
                        teras.CureVolotileStatusCondition();
                        teras.StatusChanges.Enqueue($"{teras._baseTeras.Name} snapped out of its confusion!");
                        return true;
                    }

                    teras.VolotileStatusTime--;
                    teras.StatusChanges.Enqueue($"{teras._baseTeras.Name} is confused!");

                    if(Random.Range(1,3) == 1)
                        return true;

                    teras.UpdateHP(((((2* teras.Level / 5 + 2) * teras.CalculateAttackStat * 40 ) / teras.CalculateDefenseStat ) / 50) + 2);
                    teras.StatusChanges.Enqueue($"{teras._baseTeras.Name} hurt itself while confused!");

                    return false;
                }
            }
        },
    };
}

public enum ConditionID
{
    none,poison,burn,sleep,paralysis,confusion
}