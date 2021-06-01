using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player : MonoBehaviour
{
    #region data
    // how fast the player will move per DeltaTime
    public float moveSpeed;

    // event for GameController (Observer design patter)
    public event Action OnEncounter;

    // Layers
    public LayerMask solid;
    public LayerMask grass;

    // check for animation
    bool isMoving;

    // check for input
    Vector2 input;
    
    // to animate our player
    Animator animator;

    #endregion

    // initialize animator for animation of player
    public void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    // checks for inputs
    public void HandleUpdate()
    {
        if(!isMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            if (input.x != 0) input.y = 0;

            if(input != Vector2.zero)
            {
                // give results to animator to show animation
                animator.SetFloat("X", input.x);
                animator.SetFloat("Y", input.y);

                var targetPos = transform.position;
                targetPos.x += input.x;
                targetPos.y += input.y;

                if (isWalkable(targetPos))
                {
                    // coroutine to move the player in the tilemap
                    StartCoroutine(Move(targetPos));
                }
            }
        }
        // For animations
        animator.SetBool("isMoving", isMoving);
    }

    // Move the player using Coroutine
    IEnumerator Move(Vector3 targetPos)
    {
        isMoving = true;

        while((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;

        isMoving = false;

        CheckForEncounters();
    }

    // check if the target tile is walkable
    private bool isWalkable(Vector3 targetPos)
    {
        if(Physics2D.OverlapCircle(targetPos,0.1f,solid) != null)
        {
            return false;
        }
        return true;
    }

    // function to check for encounters
    private void CheckForEncounters()
    {
        if(Physics2D.OverlapCircle(transform.position,0.2f,grass) != null)
        {
            if(UnityEngine.Random.Range(1,101)<10)
            {
                // set isMoving to false to the player isn't moving in the background
                animator.SetBool("isMoving",false);
                // start encounter
                OnEncounter();
            }
        }
    }
}
