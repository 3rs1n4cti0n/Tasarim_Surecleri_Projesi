using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player : MonoBehaviour
{
    #region data
    // event for GameController (Observer design patter)
    public event Action OnEncounter;
    // check for input
    Vector2 input;
    // to animate our player
    Character character;
    #endregion

    // initialize animator for animation of player
    public void Awake()
    {
        character = GetComponent<Character>();
    }

    // Update is called once per frame
    // checks for inputs
    public void HandleUpdate()
    {
        if(!character.isMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            if (input.x != 0) input.y = 0;

            if(input != Vector2.zero)
            {
                StartCoroutine(character.Move(input,CheckForEncounters));
            }
        }

        character.HandleUpdate();

        if (Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    // function to check for encounters
    private void CheckForEncounters()
    {
        if(Physics2D.OverlapCircle(transform.position,0.1f,GameLayers.inst.GrassLayer) != null)
        {
            if(UnityEngine.Random.Range(1,101)<10)
            {
                // set isMoving to false to the player isn't moving in the background
                character.Animator.isMoving = false;
                // start encounter
                OnEncounter();
            }
        }
    }

    void Interact()
    {
        var faceDir = new Vector3(character.Animator.X, character.Animator.Y);
        var interactPos = transform.position + faceDir;

        //Debug.DrawLine(transform.position, interactPos, Color.black,0.5f);

        var collider = Physics2D.OverlapCircle(interactPos, 0.09f, GameLayers.inst.InteractablesLayer);

        if(collider != null)
        {
            collider.GetComponent<Interactable>()?.Interact();
        }
    }

}
