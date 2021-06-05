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
    RunningTurn,
    // switch current battling teras
    PartyScreen,
    // battle is over
    BattleOver,
    // extra state for waiting
    Busy
}

public enum BattleAction { Skill,SwitchTeras,UseItem,Run}

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
    BattleState? previousState;

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
        yield return StartCoroutine(dialogBox.TypeDialog($"Encountered a wild {enemyUnit.teras._baseTeras.Name}!"));

        PlayerAction();
        ActionSelectionHandler();
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

    IEnumerator RunSkill(BattleUnit source, BattleUnit target, SkillCalc skill)
    {
        bool canMove = source.teras.OnBeforeMove();

        if(!canMove)
        {
            yield return ShowStatusChanges(source.teras);
            yield return source.Hud.UpdateHP();
            yield break;
        }

        skill.UseLeft--;
        yield return dialogBox.TypeDialog($"{source.teras._baseTeras.Name} used {skill.baseSkill.Name}!");

        if (CheckAccHit(skill, source.teras, target.teras) == true)
        {
            source.AttackAnim();
            yield return new WaitForSeconds(1f);

            target.GetHitAnim();

            // Check for status condition
            if (skill.baseSkill.Category == SkillCategory.Status)
            {
                yield return RunSkillEffects(skill.baseSkill.Effects, source.teras, target.teras,skill.baseSkill.SkillTarget);
            }
            // if its not a damaging move deal damage
            if (skill.baseSkill.Category.CompareTo(SkillCategory.Status) <= 0)
            {
                var damageDetails = target.teras.TakeDamage(skill, source.teras);
                yield return target.Hud.UpdateHP();
                yield return ShowDamageDetails(damageDetails);
            }

            if(skill.baseSkill.SecondaryEffects != null && skill.baseSkill.SecondaryEffects.Count > 0 && target.teras.Health > 0)
            {
                foreach(var secondary in skill.baseSkill.SecondaryEffects)
                {
                    var random = UnityEngine.Random.Range(1,101);
                    if (random <= secondary.Chance)
                        yield return RunSkillEffects(secondary, source.teras, target.teras, secondary.Target);
                }
            }

            // if the opposing teras fainted make battle over event true to end battle
            if (target.teras.Health <= 0)
            {
                yield return dialogBox.TypeDialog($"{target.teras._baseTeras.Name} fainted!");
                target.FaintAnim();

                yield return new WaitForSeconds(2f);

                CheckBattleOver(target);
            }
        }
        else
        {
            yield return dialogBox.TypeDialog($"{source.teras._baseTeras.Name} missed its attack!");
        }

    }
    IEnumerator RunAfterTurn(BattleUnit source)
    {
        if (state == BattleState.BattleOver) yield break;

        yield return new WaitUntil(()=> state == BattleState.RunningTurn);

        // damage check for Status
        source.teras.OnAfterTurn();
        yield return ShowStatusChanges(source.teras);
        yield return source.Hud.UpdateHP();

        if (source.teras.Health <= 0)
        {
            yield return dialogBox.TypeDialog($"{source.teras._baseTeras.Name} fainted!");
            source.FaintAnim();

            yield return new WaitForSeconds(2f);

            CheckBattleOver(source);
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

    IEnumerator RunSkillEffects(SkillEffects effects,TerasCalcs source, TerasCalcs target, SkillTarget targetedAt)
    {
        // stat boosting
        if (effects.Boosts != null)
        {
            if (targetedAt == SkillTarget.self)
                source.ApplyBoost(effects.Boosts);
            else
                target.ApplyBoost(effects.Boosts);
        }
        //Status condition
        if (effects.Status != ConditionID.none)
        {
            target.SetStatus(effects.Status);
        }

        //Volotile status condition
        if (effects.VolotileStatus != ConditionID.none)
        {
            target.SetVolotileStatus(effects.VolotileStatus);
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
                previousState = state;
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
            StartCoroutine(RunTurns(BattleAction.Skill));
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

            if(previousState == BattleState.ActionSelection)
            {
                previousState = null;
                StartCoroutine(RunTurns(BattleAction.SwitchTeras));
            }
            else
            {
                state = BattleState.Busy;
                StartCoroutine(SwitchTeras(selectedMember));
            }
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            partyScreen.gameObject.SetActive(false);
            PlayerAction();
        }
    }

    IEnumerator SwitchTeras(TerasCalcs nextTeras)
    {
        if (playerUnit.teras.Health > 0)
        {
            yield return dialogBox.TypeDialog($"Return {playerUnit.teras._baseTeras.Name}");
            playerUnit.FaintAnim();
            yield return new WaitForSeconds(2f);
        }
        // setup player
        playerUnit.Setup(nextTeras);

        // set skills of player
        dialogBox.SetSkillNames(nextTeras.Skills);

        yield return StartCoroutine(dialogBox.TypeDialog($"Fight {nextTeras._baseTeras.Name}!"));

        state = BattleState.RunningTurn;
    }

    bool CheckAccHit(SkillCalc skill,TerasCalcs source,TerasCalcs target)
    {
        if (skill.baseSkill.AlwaysHit == true)
            return true;

        float skillAccuracy = skill.baseSkill.Accuracy;

        int accuracy = source.BoostStats[Stat.accuracy];
        int evasion = target.BoostStats[Stat.evasion];

        var boostValues = new float[] { 1f, 4f / 3f, 5f / 3f, 2f, 7f / 3f, 8f / 3f, 3f };

        if (accuracy > 0)
            skillAccuracy *= boostValues[accuracy];
        else
            skillAccuracy /= boostValues[-accuracy];

        if (evasion > 0)
            skillAccuracy /= boostValues[evasion];
        else
            skillAccuracy *= boostValues[-evasion];

        return UnityEngine.Random.Range(1, 101) <= skillAccuracy;
    }

    IEnumerator RunTurns(BattleAction playerAction)
    {
        if (state == BattleState.BattleOver) yield break;

        state = BattleState.RunningTurn;
        if(playerAction == BattleAction.Skill)
        {
            playerUnit.teras.currentSkill = playerUnit.teras.Skills[currentSkill];
            enemyUnit.teras.currentSkill = enemyUnit.teras.RandomSkill();

            //check who goes first
            bool playerfirst = playerUnit.teras.CalculateSpeedStat >= enemyUnit.teras.CalculateSpeedStat;

            var firstUnit = (playerfirst) ? playerUnit : enemyUnit;
            var secondUnit = (playerfirst) ? enemyUnit : playerUnit;

            var secondTeras = secondUnit.teras;

            //First turn
            yield return RunSkill(firstUnit, secondUnit, firstUnit.teras.currentSkill);
            yield return RunAfterTurn(firstUnit);
            if(state == BattleState.BattleOver) yield break;

            if (secondTeras.Health > 0)
            {
                //second turn
                yield return RunSkill(secondUnit, firstUnit, secondUnit.teras.currentSkill);
                yield return RunAfterTurn(secondUnit);
                if (state == BattleState.BattleOver) yield break;
            }
        }
        else
        {
            if(playerAction == BattleAction.SwitchTeras)
            {
                var selectedTeras = playersParty.Party_List[currentMember];
                state = BattleState.Busy;
                yield return SwitchTeras(selectedTeras);
            }
            // Enemy Turn
            var enemySkill = enemyUnit.teras.RandomSkill();
            yield return RunSkill(enemyUnit, playerUnit, enemySkill);
            yield return RunAfterTurn(enemyUnit);
            if (state == BattleState.BattleOver) yield break;
        }

        if (state != BattleState.BattleOver)
        {
            PlayerAction();
            ActionSelectionHandler();
        }
    }
}
