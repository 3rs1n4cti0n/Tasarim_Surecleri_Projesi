using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PartyMemberUI : MonoBehaviour
{
    #region data

    // to set up name and level in UI
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI levelText;

    // to set health bar
    [SerializeField] HPbarScript HealthBar;

    [SerializeField] Color HighlightColor;

    TerasCalcs _teras;
    #endregion

    // sets name, level and health bar
    public void setData(TerasCalcs teras)
    {
        _teras = teras;

        nameText.text = _teras._baseTeras.Name;
        levelText.text = "Level: " + _teras.Level;
        HealthBar.setHealth((float)_teras.Health / _teras.CalculateMaxHealthStat);
    }

    public void HighlightSelectedMember(bool selected)
    {
        if (selected)
            nameText.color = HighlightColor;
        else
            nameText.color = Color.black;
    }
}
