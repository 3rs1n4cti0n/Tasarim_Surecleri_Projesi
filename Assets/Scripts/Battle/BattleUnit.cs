using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleUnit : MonoBehaviour
{
    #region data

    [SerializeField] Teras _base;
    [SerializeField] int level;

    // to check if the teras belongs to player
    [SerializeField] bool isPlayerUnit;

    Vector3 originalPos;
    Color originalColor;

    // used in Setup function to set images for enemy or player teras
    public TerasCalcs teras { get; set; }

    Image image;
    private void Awake()
    {
        image = GetComponent<Image>();
        originalPos = image.transform.localPosition;
        originalColor = image.color;
    }
    #endregion

    // sets images for enemy or player teras
    public void Setup()
    {
        teras = new TerasCalcs(_base, level);

        // set image
        if (isPlayerUnit)
            image.sprite = teras._baseTeras.BackSprite;
        else
            image.sprite = teras._baseTeras.FrontSprite;

        image.color = originalColor;

        EnterToBattleAnim();
    }

    // animation when the battle start
    public void EnterToBattleAnim()
    {
        if (isPlayerUnit)
            image.transform.localPosition = new Vector3(-500f, originalPos.y);
        else
            image.transform.localPosition = new Vector3(500f, originalPos.y);

        image.transform.DOLocalMoveX(originalPos.x, 1f);
    }

    // animation for attacking
    public void AttackAnim()
    {
        var sequence = DOTween.Sequence();

        if (isPlayerUnit)
            sequence.Append(image.transform.DOLocalMoveX(originalPos.x + 50, 1f));
        else
            sequence.Append(image.transform.DOLocalMoveX(originalPos.x - 50, 1f));

        sequence.Append(image.transform.DOLocalMoveX(originalPos.x, 0.25f));

    }

    // animation for getting hit
    public void GetHitAnim()
    {
        var sequence = DOTween.Sequence();

        sequence.Append(image.DOColor(Color.gray, 0.1f));
        sequence.Append(image.DOColor(originalColor, 0.1f));
    }

    // animation for fainting
    public void FaintAnim()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.transform.DOLocalMoveY(originalPos.y - 150f, 0.5f));
        sequence.Join(image.DOFade(0f, 0.5f));
    }
}
