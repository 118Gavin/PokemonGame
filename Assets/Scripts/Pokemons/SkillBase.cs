using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 *宝可梦技能 
 */
[CreateAssetMenu(fileName = "Move", menuName = "Pokemon/Creat a skill")]
public class SkillBase : ScriptableObject
{
    [SerializeField] string skillName;//技能名字

    [TextArea]
    [SerializeField] string description;//技能描述

    [SerializeField] PokemonType type;//技能属性
    [SerializeField] int power;//技能伤害
    [SerializeField] int accuracy;//技能的命中率
    [SerializeField] int pp; //技能的使用次数
    [SerializeField] SkillCategory category;
    [SerializeField] SkillTarget target;
    [SerializeField] Effects effects; // 技能提升削弱属性
    public SkillCategory Category
    {
        get { return category; }
    }

    public SkillTarget Target
    {
        get { return target; }
    }

    public Effects Effects
    {
        get { return effects; }
    }

    public string Name
    {
        get { return skillName; }
    }

    public string Description
    {
        get { return description; }
    }

    public PokemonType Type
    {
        get { return type; }
    }

    public int Power
    {
        get { return power; }
    }

    public int Accuracy
    {
        get { return accuracy; }
    }

    public int Pp
    {
        get { return pp; }
    }
}

// 技能种类，特殊，物理，状态（增益或削弱）
public enum SkillCategory { Special, Phsical, Status }

// 增益的对象
public enum SkillTarget { enemy, self }

[System.Serializable]
public class Effects
{
    [SerializeField] List<BaseStatBoost> boosts;
    [SerializeField] ConditionID status;
    [SerializeField] ConditionID volatileStatus;

    public List<BaseStatBoost> Boosts
    {
        get { return boosts; }
    }

    public ConditionID Status
    {
        get { return status; }
    }

    public ConditionID VolatileStatus
    {
        get { return volatileStatus; }
    }
}

[System.Serializable]
public class BaseStatBoost
{
    [SerializeField] Stats stat;
    [SerializeField] int boost;

    public Stats Stat
    {
        get { return stat; }
    }

    public int Boost
    {
        get { return boost; }
    }
}
