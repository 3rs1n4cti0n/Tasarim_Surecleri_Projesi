using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUnit : MonoBehaviour
{
    #region data

    [SerializeField] Teras _base;
    [SerializeField] int level;

    // to check if the teras belongs to player
    [SerializeField] bool isPlayerUnit;

    // used in Setup function to set images for enemy or player teras
    public TerasCalcs teras { get; set; }

    // sets images for enemy or player teras
    #endregion
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
