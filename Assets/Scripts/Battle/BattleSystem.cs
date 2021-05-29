using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState
{
    Start,
    PlayerAction,
    PlayerSkill,
    EnemySkill,
    Busy
}

public class BattleSystem : MonoBehaviour
{
    #region data

    // data for player
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleHUD playerHUD;
    // data for enemy bot
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleHUD enemyHUD;

    [SerializeField] BattleDialogBox dialogBox;

    BattleState state;
    int currentAction;
    int currentSkill;

    #endregion

    // When the object is created sets up UI for battle
    private void Start()
    {
        StartCoroutine(SetupBattle());
    }

    // function sets data for both player and enemy
    public IEnumerator SetupBattle()
    {
        playerUnit.Setup();
        enemyUnit.Setup();
        playerHUD.setData(playerUnit.teras);
        enemyHUD.setData(enemyUnit.teras);

        dialogBox.SetSkillNames(playerUnit.teras.Skills);

        yield return StartCoroutine(dialogBox.TypeDialog($"Encountered a wild {enemyUnit.teras._baseTeras.Name}."));

        yield return new WaitForSeconds(1f);

        PlayerAction();
    }

    void PlayerAction()
    {
        state = BattleState.PlayerAction;
        StartCoroutine(dialogBox.TypeDialog("What will you do?"));
        dialogBox.EnableActionSelector(true);
    }

    void PlayerSkill()
    {
        state = BattleState.PlayerSkill;
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);
        dialogBox.EnableSkillSelector(true);
    }

    private void Update()
    {
        if(state == BattleState.PlayerAction)
        {
            ActionSelectionHandler();
        }
        else if (state == BattleState.PlayerSkill)
        {
            SkillSelectorHandler();
        }
    }

    void ActionSelectionHandler()
    {
        if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentAction < 1)
                ++currentAction;
        }
        else if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentAction > 0)
                --currentAction;
        }

        dialogBox.UpdateActionSelection(currentAction);

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentAction == 0)
            {
                PlayerSkill();
            }
            else if (currentAction == 1)
            {

            }
        }
    }

    void SkillSelectorHandler()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentSkill < playerUnit.teras.Skills.Count - 1)
                ++currentSkill;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentSkill > 0)
                --currentSkill;
        }
        dialogBox.UpdateSkillSelection(currentSkill,playerUnit.teras.Skills[currentSkill]);
    }
}
