using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    #region data

    // data for player
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleHUD playerHUD;
    // data for enemy bot
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleHUD enemyHUD;

    #endregion

    // When the object is created sets up UI for battle
    private void Start()
    {
        SetupBattle();
    }

    // function sets data for both player and enemy
    public void SetupBattle()
    {
        playerUnit.Setup();
        enemyUnit.Setup();
        playerHUD.setData(playerUnit.teras);
        enemyHUD.setData(enemyUnit.teras);
    }
}
