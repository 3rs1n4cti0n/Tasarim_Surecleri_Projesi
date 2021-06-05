using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { FreeRoam, Battle }

// class to check if we are in battle or not and give control battle or player (State Design Pattern)
public class GameController : MonoBehaviour
{
    #region data
    // references to player, battlesystem and camera
    [SerializeField] Player _player;
    [SerializeField] BattleSystem _battleSystem;
    [SerializeField] Camera worldCamera;

    // check our current game state;
    GameState state;
    #endregion

    private void Awake()
    {
        ConditionsDB.Init();
    }

    // Observe events and call functions to give controls to either the player or battle
    private void Start()
    {
        // give it to player
        _player.OnEncounter += StartBattle;
        // give it to battle
        _battleSystem.OnBattleOver += EndBattle;
    }

    // gives control to battle
    void StartBattle()
    {
        // change game state to battle
        state = GameState.Battle;

        // activate battle system
        _battleSystem.gameObject.SetActive(true);
        // change current camera to battle by disabling main world camera
        worldCamera.gameObject.SetActive(false);

        var playerParty = _player.GetComponent<TerasParty>();
        var wild = FindObjectOfType<MapArea>().GetComponent<MapArea>().GetRandomWildTeras();

        // start the battle
        _battleSystem.StartBattle(playerParty,wild);
    }

    // ends battle and gives control to main world
    void EndBattle(bool won)
    {
        // change game state to FreeRoam
        state = GameState.FreeRoam;

        // disables battle system
        _battleSystem.gameObject.SetActive(false);
        // change the current camera to main world camera
        worldCamera.gameObject.SetActive(true);
    }

    // updates every deltaTime changes to battle or main world depending on the state
    private void Update()
    {
        if(state == GameState.FreeRoam)
        {
            _player.HandleUpdate();
        }
        else if (state == GameState.Battle)
        {
            _battleSystem.HandleUpdate();
        }
    }
}
