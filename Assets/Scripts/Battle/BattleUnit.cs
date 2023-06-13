using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; //引用DOTween插件

public class BattleUnit : MonoBehaviour
{
    [SerializeField] bool isPlayerUnit; // 用来判断是否是玩家的宝可梦
    [SerializeField] BattleHub hub;

    public BattleHub Hub { get { return hub; } }

    public bool IsPlayerUnit { get { return isPlayerUnit; } }

    private Image image;
    private Vector3 orginalPos; //用来保存宝可梦的初始位置
    private Color orginalColor;

    public Pokemon Pokemon { get; set; }

    private void Awake()
    {
        image = GetComponent<Image>();
        orginalPos = transform.localPosition; // 获取loclPosition是根据画布的pos
        orginalColor = image.color; // 获取图片默认颜色
    }

    public void Setup(Pokemon pokomon)
    {
        Pokemon = pokomon;
        if (isPlayerUnit)
            image.sprite = Pokemon.Base.BackSprite;
        else
            image.sprite = Pokemon.Base.FrontSprite;

        hub.SetData(pokomon);
        image.color = orginalColor;

        PlayEnterAnimation();
    }

    public bool IsHavePP(Skill skill)
    {
        return Pokemon.IsHavePP(skill);
    }

    public void PlayEnterAnimation()
    {
        if (isPlayerUnit)
            image.transform.localPosition = new Vector3(-500, orginalPos.y, orginalPos.z);
        else
            image.transform.localPosition = new Vector3(500, orginalPos.y, orginalPos.z);

        image.transform.DOLocalMoveX(orginalPos.x, 1.5f); // 第一个参数是到达的位置
                                                          // 第二个参数表示当前位置移动到该位置的时间
    }

    public void AttackAnimation()
    {
        var sequence = DOTween.Sequence();
        if (isPlayerUnit)
            sequence.Append(image.transform.DOLocalMoveX(orginalPos.x + 50, 0.25f));
        else
            sequence.Append(image.transform.DOLocalMoveX(orginalPos.x - 50, 0.25f));
        sequence.Append(image.transform.DOLocalMoveX(orginalPos.x, 0.25f));
    }

    public void HitAnimation()
    {
        var sequence = DOTween.Sequence();


        sequence.Append(image.DOFade(0f, 0.1f));
        sequence.Append(image.DOFade(1f, 0.1f));
        sequence.SetLoops<Sequence>(3);
    }

    public void DeadAnimation()
    {
        var sequence = DOTween.Sequence();

        sequence.Append(image.transform.DOLocalMoveY(orginalPos.y - 150f, 0.5f));
        sequence.Join(image.DOFade(0, 0.15f));

    }
}
