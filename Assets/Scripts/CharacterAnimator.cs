using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    [SerializeField] List<Sprite> walkDownSprite;
    [SerializeField] List<Sprite> walkUpSprite;
    [SerializeField] List<Sprite> walkRightSprite;
    [SerializeField] List<Sprite> walkLeftSprite;

    // parameters
    public float X { get; set; }
    public float Y { get; set; }
    public bool isMoving { get; set; }

    bool wasMoving;

    // States
    SpriteAnimator walkDownAnim;
    SpriteAnimator walkUpAnim;
    SpriteAnimator walkRightAnim;
    SpriteAnimator walkLeftAnim;

    SpriteAnimator currenAnimation;

    // references
    SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        walkDownAnim = new SpriteAnimator(walkDownSprite, spriteRenderer);
        walkUpAnim = new SpriteAnimator(walkUpSprite, spriteRenderer);
        walkRightAnim = new SpriteAnimator(walkRightSprite, spriteRenderer);
        walkLeftAnim = new SpriteAnimator(walkLeftSprite, spriteRenderer);

        currenAnimation = walkDownAnim;
    }

    private void Update()
    {
        var previousAnimation = currenAnimation;
        if (X == 1)
            currenAnimation = walkRightAnim;
        else if (X == -1)
            currenAnimation = walkLeftAnim;
        else if (Y == 1)
            currenAnimation = walkUpAnim;
        else if (Y == -1)
            currenAnimation = walkDownAnim;

        if (currenAnimation != previousAnimation || isMoving != wasMoving)
            currenAnimation.Start();

        if (isMoving == true)
            currenAnimation.HandleUpdate();
        else
            spriteRenderer.sprite = currenAnimation.Frames[0];

        wasMoving = isMoving;
    }
}
