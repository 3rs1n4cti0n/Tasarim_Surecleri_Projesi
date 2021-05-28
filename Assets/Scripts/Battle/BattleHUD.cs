using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class BattleHUD : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] HPbarScript HealthBar;

    public void setData(TerasCalcs teras)
    {
        nameText.text = teras._baseTeras.Name;
        levelText.text = "Level: " + teras.level;
        HealthBar.setHealth((float) teras.Health / teras.CalculateMaxHealthStat);
    }
}
