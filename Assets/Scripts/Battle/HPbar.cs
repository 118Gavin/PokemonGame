using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPbar : MonoBehaviour
{
    [SerializeField] GameObject health;
    [SerializeField] Sprite yellowHp;
    [SerializeField] Sprite redHp;

    // 扣血方法
    // 参数是受到的伤害
    public void SetHp(float buckleBlood)
    {
        health.transform.localScale = new Vector3(buckleBlood, 1f, 1f);
    }

    public IEnumerator SetUpSmooth(float newHp)
    {
        float curHp = health.transform.localScale.x; // 获取当前hp的Scale的x大小

        while(curHp - newHp > Mathf.Epsilon) // 如果当前hp - 扣血之后的hp 是无限等于0时，循环结束
                                             // （因为是俩个浮点数比较，这样的比较之后准确度更高）
        {
            curHp -= Time.deltaTime; // 每帧都减去Time.deltaTime的大小
            health.transform.localScale = new Vector3(curHp, 1f, 1f); // 每次都在场景中更新
            yield return null; // 保存当前的协程在下一帧调用
        }

        health.transform.localScale = new Vector3(newHp, 1f, 1f);
    }

    public void ChangeHPColor(float hp)
    {
        if(hp <=0.5 && hp>= 1.0)
        {

        }
    }
}
