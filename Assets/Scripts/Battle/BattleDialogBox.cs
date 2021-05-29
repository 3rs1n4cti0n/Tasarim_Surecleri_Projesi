using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class BattleDialogBox : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI dialogText;
    [SerializeField] int lettersPerSecond;
    [SerializeField] Color highletedColor;

    [SerializeField] GameObject actionSelector;
    [SerializeField] GameObject skillSelector;
    [SerializeField] GameObject skillDetails;

    [SerializeField] List<TextMeshProUGUI> actionText;
    [SerializeField] List<TextMeshProUGUI> skillText;

    [SerializeField] TextMeshProUGUI useLeftText;
    [SerializeField] TextMeshProUGUI elementText;

    public void SetDialog(string dialog)
    {
        dialogText.text = dialog;
    }

    public IEnumerator TypeDialog(string dialog)
    {
        dialogText.text = "";
        foreach(var letter in dialog.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }
    }

    public void EnableDialogText(bool enabled)
    {
        dialogText.enabled = enabled;
    }

    public void EnableActionSelector(bool enabled)
    {
        actionSelector.SetActive(enabled);
    }

    public void EnableSkillSelector(bool enabled)
    {
        skillSelector.SetActive(enabled);
        skillDetails.SetActive(enabled);
    }

    public void UpdateActionSelection(int selectedAction)
    {
        for(int i = 0; i<actionText.Count; i++)
        {
            if(i == selectedAction)
                actionText[i].color = highletedColor;
            else
                actionText[i].color = Color.black;
        }
    }

    public void UpdateSkillSelection(int selectedSkill, SkillCalc skill)
    {
        for (int i = 0; i < skillText.Count; i++)
        {
            if (i == selectedSkill)
                skillText[i].color = highletedColor;
            else
                skillText[i].color = Color.black;
        }

        useLeftText.text = $"Use Left: {skill.UseLeft}/{skill.baseSkill.UseLeft}";
        elementText.text = skill.baseSkill.SkillElement.ToString();
    }

    public void SetSkillNames(List<SkillCalc> skills)
    {
        for(int i = 0; i < skillText.Count; i++)
        {
            if (i < skills.Count)
                skillText[i].text = skills[i].baseSkill.Name;
            else
                skillText[i].text = "-";
        }
    }
}