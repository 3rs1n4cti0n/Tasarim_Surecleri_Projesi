using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// battle states
public enum BattleState
{
    // beginning of battle
    Start,
    // player action select
    PlayerAction,
    // player skill select
    PlayerSkill,
    // enemy skill select
    EnemySkill,
    // extra state for waiting
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

    // handles all the dialog on battle
    [SerializeField] BattleDialogBox dialogBox;

    // event that is used by GameController to check if the battle is over or not
    public event Action<bool> OnBattleOver;

    // state of battle
    BattleState state;

    // indexes for current action and skill
    int currentAction;
    int currentSkill;

    #endregion

    // When the object is created sets up UI for battle
    public void StartBattle()
    {
        StartCoroutine(SetupBattle());
    }

    // function sets data for both player and enemy
    public IEnumerator SetupBattle()
    {
        // setup player
        playerUnit.Setup();
        playerHUD.setData(playerUnit.teras);
        // setup enemy
        enemyUnit.Setup();
        enemyHUD.setData(enemyUnit.teras);

        // set skills of player
        dialogBox.SetSkillNames(playerUnit.teras.Skills);

        // encounter message
        yield return StartCoroutine(dialogBox.TypeDialog($"Encountered a wild {enemyUnit.teras._baseTeras.Name}."));

        // action selection
        PlayerAction();
    }

    // function for selecting action
    void PlayerAction()
    {
        // change state to PlayerAction
        state = BattleState.PlayerAction;
        StartCoroutine(dialogBox.TypeDialog("What will you do?"));

        // shows action selector menu
        dialogBox.EnableActionSelector(true);
    }

    // function for player teras to use a skill
    void PlayerSkill()
    {
        // change state to PlayerSkill
        state = BattleState.PlayerSkill;
        // disable action selection and dialog text
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);
        // show skill selector
        dialogBox.EnableSkillSelector(true);
    }
    // coroutine to deal damage, take damage, play battle animations for player teras
    IEnumerator UseSkill()
    {
        state = BattleState.Busy;
        var skill = playerUnit.teras.Skills[currentSkill];
        yield return dialogBox.TypeDialog($"{playerUnit.teras._baseTeras.Name} used {skill.baseSkill.Name}");

        playerUnit.AttackAnim();
        yield return new WaitForSeconds(1f);

        enemyUnit.GetHitAnim();
        var damageDetails = enemyUnit.teras.TakeDamage(skill, enemyUnit.teras);
        yield return enemyHUD.UpdateHP();
        yield return ShowDamageDetails(damageDetails);

        // if the opposing teras fainted make battle over event true to end battle
        if (damageDetails.Fainted)
        {
            yield return dialogBox.TypeDialog($"{enemyUnit.teras._baseTeras.Name} fainted");
            enemyUnit.FaintAnim();

            yield return new WaitForSeconds(2f);
            OnBattleOver(true);
        }
        // otherwise it is enemys' turn to attack
        else
        {
            StartCoroutine(EnemyUseSkill());
        }
    }
    // coroutine to deal damage, take damage, play battle animations for enemy teras
    IEnumerator EnemyUseSkill()
    {
        state = BattleState.EnemySkill;

        var skill = enemyUnit.teras.RandomSkill();
        yield return dialogBox.TypeDialog($"{enemyUnit.teras._baseTeras.Name} used {skill.baseSkill.Name}");

        enemyUnit.AttackAnim();
        yield return new WaitForSeconds(1f);

        playerUnit.GetHitAnim();
        var damageDetails = playerUnit.teras.TakeDamage(skill, playerUnit.teras);
        yield return playerHUD.UpdateHP();
        yield return ShowDamageDetails(damageDetails);
        // if the player teras fainted make battle over event true to end battle
        if (damageDetails.Fainted)
        {
            yield return dialogBox.TypeDialog($"{playerUnit.teras._baseTeras.Name} fainted");
            playerUnit.FaintAnim();

            yield return new WaitForSeconds(2f);
            OnBattleOver(false);
        }
        // otherwise it is players' turn to attack
        else
        {
            PlayerAction();
        }
    }

    // displays damage details such as: critical hit, super effective or not effective
    IEnumerator ShowDamageDetails(DamageDetails damageDetails)
    {
        if (damageDetails.Critical > 1f)
            yield return dialogBox.TypeDialog("It's a critical hit!");

        if (damageDetails.ElementEffectiveness > 1f)
            yield return dialogBox.TypeDialog("It's a super effective hit!");

        else if (damageDetails.ElementEffectiveness < 1f)
            yield return dialogBox.TypeDialog("It was not effective!");
    }

    // checks for battle state and call function for that battle state
    public void HandleUpdate()
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

    // handles action selection
    void ActionSelectionHandler()
    {
        //choosing a new action
        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (currentAction < 1)
                ++currentAction;
        }
        else if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currentAction > 0)
                --currentAction;
        }

        dialogBox.UpdateActionSelection(currentAction);

        // handles input and calls for the apropriate function
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

    // function to handle skill selection
    void SkillSelectorHandler()
    {
        // choosing a skill
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

        // handle input
        if (Input.GetKeyDown(KeyCode.E))
        {
            // disable skill selection
            dialogBox.EnableSkillSelector(false);
            // enable dialog box
            dialogBox.EnableDialogText(true);
            // use skill
            StartCoroutine(UseSkill());
        }
    }
}
