using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TerasParty : MonoBehaviour
{
    [SerializeField] List<TerasCalcs> party_list;

    private void Start()
    {
        foreach(var t in party_list)
        {
            t.Init();
        }
    }

    public TerasCalcs GetHealthyTeras()
    {
        return party_list.Where(x => x.Health > 0).FirstOrDefault();
    }
}
