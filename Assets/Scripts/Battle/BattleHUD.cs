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
    [SerializeField] TextMeshProUGUI statusText;
    // to set health bar
    [SerializeField] HPbarScript HealthBar;
    [SerializeField] Color poisonClr;
    [SerializeField] Color paralyzeClr;
    [SerializeField] Color burnClr;
    [SerializeField] Color sleepClr;

    Dictionary<ConditionID, Color> StatusColors;

    TerasCalcs _teras;
    #endregion

    // sets name, level and health bar
    public void setData(TerasCalcs teras)
    {
        _teras = teras;

        nameText.text = teras._baseTeras.Name;
        levelText.text = "Level: " + teras.Level;
        HealthBar.setHealth((float) teras.Health / teras.CalculateMaxHealthStat);

        StatusColors = new Dictionary<ConditionID, Color>()
        {
            {ConditionID.poison, poisonClr},
            {ConditionID.paralysis, paralyzeClr},
            {ConditionID.burn, burnClr},
            {ConditionID.sleep, sleepClr},
        };

        SetStatusText();
        _teras.OnStatusChange += SetStatusText;
    }

    void SetStatusText()
    {
        if(_teras.Status == null)
        {
            statusText.text = "";
        }
        else
        {
            statusText.text = _teras.Status.Id.ToString().ToLower();
            statusText.color = StatusColors[_teras.Status.Id];
        }
    }

    // updates health bar slowly
    public IEnumerator UpdateHP()
    {
        if (_teras.HealthChanged)
        {
            yield return HealthBar.SetHPslowly((float)_teras.Health / _teras.CalculateMaxHealthStat);
            _teras.HealthChanged = false;
        }
    }
}
