using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour, Interactable
{
    [SerializeField] dialog Dialog;
    [SerializeField] List<Vector2> pattern;
    [SerializeField] float TimeBetweenPatterns;

    NPCState state;
    float idleTimer = 0;
    Character character;
    int currentPattern = 0;

    private void Awake()
    {
        character = GetComponent<Character>();
    }

    public void Interact()
    {
        if(state == NPCState.Idle)
            StartCoroutine(DialogManager.Instance.showDialog(Dialog));
    }
    private void Update()
    {
        if (DialogManager.Instance.isShowing) return;

        if(state == NPCState.Idle)
        {
            idleTimer += Time.deltaTime;
            if(idleTimer > TimeBetweenPatterns)
            {
                idleTimer = 0;
                if(pattern.Count > 0)
                {
                    StartCoroutine(Walk());
                }
            }
        }
        character.HandleUpdate();
    }

    IEnumerator Walk()
    {
        state = NPCState.Walking;

        yield return character.Move(pattern[currentPattern]);
        currentPattern += (currentPattern + 1) % pattern.Count;


        state = NPCState.Idle;
    }
}

public enum NPCState
{
    Idle,
    Walking
}