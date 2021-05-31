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

    TerasCalcs _teras;
    #endregion

    // sets name, level and health bar
    public void setData(TerasCalcs teras)
    {
        _teras = teras;

        nameText.text = teras._baseTeras.Name;
        levelText.text = "Level: " + teras.level;
        HealthBar.SetHPslowly((float) teras.Health / teras.CalculateMaxHealthStat);
    }

    public IEnumerator UpdateHP()
    {
        yield return HealthBar.SetHPslowly((float)_teras.Health / _teras.CalculateMaxHealthStat);
    }
}
