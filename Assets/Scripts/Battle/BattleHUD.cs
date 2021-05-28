using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class BattleHUD : MonoBehaviour
{
    #region data
    
    // to set up name and level in UI
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI levelText;

    // to set health bar
    [SerializeField] HPbarScript HealthBar;

    #endregion

    // sets name, level and health bar
    public void setData(TerasCalcs teras)
    {
        nameText.text = teras._baseTeras.Name;
        levelText.text = "Level: " + teras.level;
        HealthBar.setHealth((float) teras.Health / teras.CalculateMaxHealthStat);
    }
}
