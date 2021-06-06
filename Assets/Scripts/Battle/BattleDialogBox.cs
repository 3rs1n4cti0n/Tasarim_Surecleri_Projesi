using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// Class to handle everything about our dialog boxes
public class BattleDialogBox : MonoBehaviour
{
    #region data

    // reference to dialog box text
    [SerializeField] TextMeshProUGUI dialogText;

    // how many letters will be displayed per second
    [SerializeField] int lettersPerSecond;

    // highlight color for selected item
    [SerializeField] Color highletedColor;

    // references to selection manus
    [SerializeField] GameObject actionSelector;
    [SerializeField] GameObject skillSelector;

    // skill use left, name etc.
    [SerializeField] GameObject skillDetails;

    // list of actions and skills
    [SerializeField] List<TextMeshProUGUI> actionText;
    [SerializeField] List<TextMeshProUGUI> skillText;

    // text to display useleft and element of a skill
    [SerializeField] TextMeshProUGUI useLeftText;
    [SerializeField] TextMeshProUGUI elementText;

    #endregion

    // sets dialog instantly
    public void SetDialog(string dialog)
    {
        dialogText.text = dialog;
    }

    // Coroutine to type the dialog slowly
    public IEnumerator TypeDialog(string dialog)
    {
        dialogText.text = "";

        Debug.Log(dialog);

        // for loop to write dialog string slowly
        foreach(var letter in dialog.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }

        yield return new WaitForSeconds(1f);
    }

    // enables or disables dialog text
    public void EnableDialogText(bool enabled)
    {
        dialogText.enabled = enabled;
    }
    // enables or disables action selector text
    public void EnableActionSelector(bool enabled)
    {
        actionSelector.SetActive(enabled);
    }
    // enables or disables skill selector text
    public void EnableSkillSelector(bool enabled)
    {
        skillSelector.SetActive(enabled);
        skillDetails.SetActive(enabled);
    }

    // function to highlight selected action
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

    // function to highlight skills and update skill details
    public void UpdateSkillSelection(int selectedSkill, SkillCalc skill)
    {
        // highlight selected skill
        for (int i = 0; i < skillText.Count; i++)
        {
            if (i == selectedSkill)
                skillText[i].color = highletedColor;
            else
                skillText[i].color = Color.black;
        }

        // update skill details
        useLeftText.text = $"Use Left: {skill.UseLeft}/{skill.baseSkill.UseLeft}";
        elementText.text = skill.baseSkill.SkillElement.ToString();

        // visual feedback for player that has no use left
        if (skill.UseLeft == 0)
            useLeftText.color = Color.red;
        else
            useLeftText.color = Color.black;
    }

    // function to set skill names into textmesh
    public void SetSkillNames(List<SkillCalc> skills)
    {
        // repeats until every skill is set and sets "-" for blank skills
        for(int i = 0; i < skillText.Count; i++)
        {
            if (i < skills.Count)
                skillText[i].text = skills[i].baseSkill.Name;
            else
                skillText[i].text = "-";
        }
    }
}