using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Condition
{
    public ConditionID Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string StatMessage { get; set; }

    public Action<TerasCalcs> OnStart { get; set; }

    public Action<TerasCalcs> OnAfterTurn { get; set; } 
    public Func<TerasCalcs, bool> OnPerformSkill { get; set; }
}
