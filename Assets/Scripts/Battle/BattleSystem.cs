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
    ActionSelection,
    // player skill select
    SkillSelection,
    // enemy skill select
    UseSkill,
    // switch current battling teras
    PartyScreen,
    // battle is over
    BattleOver,
    // extra state for waiting
    Busy
}

public class BattleSystem : MonoBehaviour
{
    #region data

    // data for player
    [SerializeField] BattleUnit playerUnit;
    // data for enemy bot
    [SerializeField] BattleUnit enemyUnit;

    // handles all the dialog on battle
    [SerializeField] BattleDialogBox dialogBox;

    // event that is used by GameController to check if the battle is over or not
    public event Action<bool> OnBattleOver;

    // state of battle
    BattleState state;

    [SerializeField] PartyScreen partyScreen;

    // indexes for current action and skill
    int currentAction;
    int currentSkill;
    int currentMember;

    TerasParty playersParty;
    TerasCalcs wild;

    #endregion

    // When the object is created sets up UI for battle
    public void StartBattle(TerasParty PLAYER, TerasCalcs WILD_TERAS)
    {
        playersParty = PLAYER;
        wild = WILD_TERAS;
        StartCoroutine(SetupBattle());
    }

    // function sets data for both player and enemy
    public IEnumerator SetupBattle()
    {
        // setup player
        playerUnit.Setup(playersParty.GetHealthyTeras());
        
        // setup enemy
        enemyUnit.Setup(wild);

        partyScreen.Init();

        // set skills of player
        dialogBox.SetSkillNames(playerUnit.teras.Skills);

        // encounter message
        yield return StartCoroutine(dialogBox.TypeDialog($"Encountered a wild {enemyUnit.teras._baseTeras.Name}."));

        // action selection
        ChooseFirstTurn();
    }

    // function for selecting action
    void PlayerAction()
    {
        // change state to PlayerAction
        state = BattleState.ActionSelection;
        StartCoroutine(dialogBox.TypeDialog("What will you do?"));

        // shows action selector menu
        dialogBox.EnableActionSelector(true);
    }

    void OpenPartyScreen()
    {
        state = BattleState.PartyScreen;
        partyScreen.gameObject.SetActive(true);
        partyScreen.SetPartyData(playersParty.Party_List);
    }

    // function for player teras to use a skill
    void OpenPlayerSkills()
    {
        // change state to PlayerSkill
        state = BattleState.SkillSelection;
        // disable action selection and dialog text
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);
        // show skill selector
        dialogBox.EnableSkillSelector(true);
    }

    // coroutine to deal damage, take damage, play battle animations for player teras
    IEnumerator PlayerUseSkill()
    {
        state = BattleState.UseSkill;
        
        var skill = playerUnit.teras.Skills[currentSkill];
        
        yield return RunSkill(playerUnit,enemyUnit,skill);

        // if runSkill didnt change state to BattleOver continue battle
        if (state == BattleState.UseSkill)
            StartCoroutine(EnemyUseSkill());
    }
    // coroutine to deal damage, take damage, play battle animations for enemy teras
    IEnumerator EnemyUseSkill()
    {
        state = BattleState.UseSkill;

        var skill = enemyUnit.teras.RandomSkill();

        yield return RunSkill(enemyUnit, playerUnit, skill);

        // if runSkill didnt change state to BattleOver continue battle
        if (state == BattleState.UseSkill)
            PlayerAction();
    }

    IEnumerator RunSkill(BattleUnit source, BattleUnit target, SkillCalc skill)
    {
        skill.UseLeft--;
        yield return dialogBox.TypeDialog($"{source.teras._baseTeras.Name} used {skill.baseSkill.Name}");

        source.AttackAnim();
        yield return new WaitForSeconds(1f);

        target.GetHitAnim();

        Debug.Log(skill.baseSkill.Category);

        if (skill.baseSkill.Category.CompareTo(SkillCategory.Status) > 0)
        {
            RunSkillEffects(skill, source.teras, target.teras);
        }
        if (skill.baseSkill.Category.CompareTo(SkillCategory.Status) <= 0)
        {
            var damageDetails = target.teras.TakeDamage(skill, source.teras);
            yield return target.Hud.UpdateHP();
            yield return ShowDamageDetails(damageDetails);
        }
        // if the opposing teras fainted make battle over event true to end battle
        if (target.teras.Health <= 0)
        {
            yield return dialogBox.TypeDialog($"{target.teras._baseTeras.Name} fainted");
            target.FaintAnim();

            yield return new WaitForSeconds(2f);

            CheckBattleOver(target);
        }
    }

    IEnumerator ShowStatusChanges(TerasCalcs teras)
    {
        while(teras.StatusChanges.Count > 0)
        {
            var message = teras.StatusChanges.Dequeue();
            yield return dialogBox.TypeDialog(message);
        }
    }

    IEnumerator RunSkillEffects(SkillCalc skill,TerasCalcs source, TerasCalcs target)
    {
        var effects = skill.baseSkill.Effects;
        // stat boosting
        if (effects.Boosts != null)
        {
            if (skill.baseSkill.SkillTarget == SkillTarget.self)
                source.ApplyBoost(effects.Boosts);
            else
                target.ApplyBoost(effects.Boosts);
        }
        //Status condition
        if (effects.Status != ConditionID.none)
        {
            target.SetStatus(effects.Status);
        }

        yield return ShowStatusChanges(source);
        yield return ShowStatusChanges(target);
    }

    void CheckBattleOver(BattleUnit faintedTeras)
    {
        if(faintedTeras.IsPlayerUnit)
        {
            var nextTeras = playersParty.GetHealthyTeras();
            if (nextTeras != null)
            {
                OpenPartyScreen();
            }
            else
            {
                BattleOver(false);
            }
        }
        else
        {
            BattleOver(true);
        }
    }

    void ChooseFirstTurn()
    {
        if (playerUnit.teras.CalculateSpeedStat >= enemyUnit.teras.CalculateSpeedStat)
            ActionSelectionHandler();
        else
            StartCoroutine(EnemyUseSkill());
    }

    void BattleOver(bool won)
    {
        state = BattleState.BattleOver;
        // shorter way of writing foreach
        playersParty.Party_List.ForEach(t => t.OnBattleOver());
        OnBattleOver(won);
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
        if(state == BattleState.ActionSelection)
        {
            ActionSelectionHandler();
        }
        else if (state == BattleState.SkillSelection)
        {
            SkillSelectorHandler();
        }
        else if (state == BattleState.PartyScreen)
        {
            PartySelectorHandler();
        }
    }

    // handles action selection
    void ActionSelectionHandler()
    {
        //choosing a new action
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++currentAction;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --currentAction;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            currentAction += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentAction -= 2;

        // limit it to 0-3 index
        currentAction = Mathf.Clamp(currentAction, 0, 3);

        dialogBox.UpdateActionSelection(currentAction);

        // handles input and calls for the apropriate function
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentAction == 0)
            {
                OpenPlayerSkills();
            }
            else if (currentAction == 1)
            {
                // items
            }
            else if (currentAction == 2)
            {
                // party
                OpenPartyScreen();
            }
            else if (currentAction == 3)
            {
                // run
            }
        }
    }

    // function to handle skill selection
    void SkillSelectorHandler()
    {
        // choosing a skill
        if (Input.GetKeyDown(KeyCode.DownArrow))
            ++currentSkill;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            --currentSkill;

        currentSkill = Mathf.Clamp(currentSkill, 0, playerUnit.teras.Skills.Count - 1);

        dialogBox.UpdateSkillSelection(currentSkill,playerUnit.teras.Skills[currentSkill]);

        // handle input
        if (Input.GetKeyDown(KeyCode.E))
        {
            // disable skill selection
            dialogBox.EnableSkillSelector(false);
            // enable dialog box
            dialogBox.EnableDialogText(true);
            // use skill
            StartCoroutine(PlayerUseSkill());
        }else if (Input.GetKeyDown(KeyCode.R))
        {
            dialogBox.EnableSkillSelector(false);
            dialogBox.EnableDialogText(true);
            PlayerAction();
        }
    }

    void PartySelectorHandler()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++currentMember;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --currentMember;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            currentMember += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentMember -= 2;

        // limit it to 0-(number of teras in party) index
        currentMember = Mathf.Clamp(currentMember, 0, playersParty.Party_List.Count - 1);

        partyScreen.UpdateMemberSelection(currentMember);

        if (Input.GetKeyDown(KeyCode.E))
        {
            var selectedMember = playersParty.Party_List[currentMember];
            if (selectedMember.Health <= 0)
            {
                partyScreen.SetMessageText("Can't send out Fainted Teras!");
                return;
            }
            if (selectedMember == playerUnit.teras)
            {
                partyScreen.SetMessageText("Can't send out same Teras!");
                return;
            }
            partyScreen.gameObject.SetActive(false);
            state = BattleState.Busy;
            StartCoroutine(SwitchTeras(selectedMember));
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            partyScreen.gameObject.SetActive(false);
            PlayerAction();
        }
    }

    IEnumerator SwitchTeras(TerasCalcs nextTeras)
    {
        bool isFainted = true;
        if (playerUnit.teras.Health > 0)
        {
            isFainted = false;
            yield return dialogBox.TypeDialog($"Return {playerUnit.teras._baseTeras.Name}");
            playerUnit.FaintAnim();
            yield return new WaitForSeconds(2f);
        }
        // setup player
        playerUnit.Setup(nextTeras);

        // set skills of player
        dialogBox.SetSkillNames(nextTeras.Skills);

        // encounter message
        yield return StartCoroutine(dialogBox.TypeDialog($"Fight {nextTeras._baseTeras.Name}!"));

        if (isFainted)
            ChooseFirstTurn();
        else
            StartCoroutine(EnemyUseSkill());
    }

}
