using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapArea : MonoBehaviour
{
    [SerializeField] List<TerasCalcs> wildEncounters;

    public TerasCalcs GetRandomWildTeras()
    {
        var wildEncounter = wildEncounters[Random.Range(0,wildEncounters.Count)];
        wildEncounter.Init();
        return wildEncounter;
    }
}
