using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


/*
 * 记录状态技能的所有数据（ 毒, 燃烧, 睡眠, 麻痹, 冻结）
 */
public class Condition
{
    // 名字
    public string Name { get; set; }
    // 描述
    public string Description { get; set; }
    // 对话框显示信息
    public string StartMessage { get; set; }

    public ConditionID ID { get; set; }


    // 记录睡眠状态持续的回合
    public Action<Pokemon> OnStart { get; set; }

    //判定麻痹是否攻击事件，返回值为bool
    public Func<Pokemon, bool> OnBeforeTurn { get; set; }

    // 回调方法
    public Action<Pokemon> OnAfterTurn { get; set; }
}
