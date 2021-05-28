using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] Teras _base;
    [SerializeField] int level;
    [SerializeField] bool isPlayerUnit;

    public TerasCalcs teras { get; set; }

    public void Setup()
    {
        teras = new TerasCalcs(_base, level);

        // set image
        if (isPlayerUnit)
            GetComponent<Image>().sprite = teras._baseTeras.BackSprite;
        else
            GetComponent<Image>().sprite = teras._baseTeras.FrontSprite;
    }
}
