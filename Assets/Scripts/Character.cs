using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    CharacterAnimator animator;
    public bool isMoving { get; set; }

    public CharacterAnimator Animator
    {
        get { return animator; }
    }

    // how fast the player will move per DeltaTime
    public float moveSpeed;

    private void Awake()
    {
        animator = GetComponent<CharacterAnimator>();
    }

    // Move the player using Coroutine
    public IEnumerator Move(Vector2 moveVec, Action OnMoveOver = null)
    {
        // give results to animator to show animation
        animator.X = Mathf.Clamp(moveVec.x, -1f, 1f);
        animator.Y = Mathf.Clamp(moveVec.y, -1f, 1f);

        var targetPos = transform.position;
        targetPos.x += moveVec.x;
        targetPos.y += moveVec.y;

        if (!isWalkable(targetPos))
            yield break;

        isMoving = true;

        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;

        isMoving = false;

        OnMoveOver?.Invoke();
    }

    public void HandleUpdate()
    {
        animator.isMoving = isMoving;
    }

    // check if the target tile is walkable
    private bool isWalkable(Vector3 targetPos)
    {
        if (Physics2D.OverlapCircle(targetPos, 0.3f, GameLayers.inst.SolidLayer | GameLayers.inst.InteractablesLayer) != null)
        {
            return false;
        }
        return true;
    }

}
