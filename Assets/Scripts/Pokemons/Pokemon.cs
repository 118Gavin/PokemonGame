using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

/*
 * 用来计算当前宝可梦的等级成员变量
 */
[System.Serializable]
public class Pokemon
{
    [SerializeField] PokemonBase _base;
    [SerializeField] int level;

    public PokemonBase Base { get { return _base; } }
    public int Level { get { return level; } }

    public List<Skill> Skills { get; set; }

    public int Hp { get; set; }

    public Dictionary<Stats, int> PokemonStats { get; private set; }
    public Dictionary<Stats, string> ChineseStats { get; private set; }
    public Dictionary<Stats, int> StatBoosts { get; private set; } // 记录状态提升的等级
    public Queue<string> StatusMessage { get; private set; } = new Queue<string>();// 记录状态攻击的信息

    public Condition Status { get; private set; } // 用来保存当前的状态攻击数据

    public int StatusTime { get; set; }

    public Condition VolatileStatus { get; private set; } // 用来保存不确定状态的数据

    public int VolatileStatusTime { get; set; }



    public bool HpChange { get; set; }

    // 状态更新事件
    public event Action OnStatusChange;

    // 宝可梦的初始化
    public void Init()
    {
        Skills = new List<Skill>();
        foreach (var skill in Base.LearnSkills) //遍历宝可梦的技能
        {
            if (skill.Level <= Level)//当宝可梦的技能等级限制是在当前宝可梦的等级内则保存到skills里
                Skills.Add(new Skill(skill.SkillBase));
            if (Skills.Count >= 4)//如果宝可梦的技能有四个及以上则结束
                break;
        }

        CalcuateStats();
        Hp = MaxHp;

        // 初始化状态等级
        StatBoosts = new Dictionary<Stats, int>()
        {
            {Stats.Attack, 0},
            {Stats.Defense,0},
            {Stats.SpAttack,0},
            {Stats.SpDefense,0},
            {Stats.Speed,0},
        };

        ChineseStats = new Dictionary<Stats, string>()
        {
            {Stats.Attack, "攻击"},
            {Stats.Defense,"防御"},
            {Stats.SpAttack,"特殊攻击"},
            {Stats.SpDefense,"特殊防御"},
            {Stats.Speed,"速度"},
        };

        //游戏重新开始时重置状态
        Status = null;
        VolatileStatus = null;
    }

    // 根据宝可梦属性的枚举值，获取对应的数值
    int GetStat(Stats stat)
    {
        // 获取宝可梦当前的属性数值
        int statValue = PokemonStats[stat];
        // 获取宝可梦提升的等级（正数为增益，负数为削弱）
        int boost = StatBoosts[stat];
        // 提升等级一共六级
        float[] boostValue = new float[] { 1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f };

        // 如果是增益则 * ，削弱是 /
        if (boost >= 0)
            statValue = Mathf.FloorToInt(statValue * boostValue[boost]);
        else
            statValue = Mathf.FloorToInt(statValue / boostValue[-boost]);
        return statValue;
    }


    // 如果是状态攻击，更新StatBoosts[Stat]对应的值
    public void AppleBoost(List<BaseStatBoost> baseStatBoosts)
    {
        foreach (var baseStatBoost in baseStatBoosts)
        {
            var stat = baseStatBoost.Stat;
            var boost = baseStatBoost.Boost;

            if (boost > 0)
            {
                StatusMessage.Enqueue($"{_base.Name}的{ChineseStats[stat]}提高！");
            }
            else
                StatusMessage.Enqueue($"{_base.Name}的{ChineseStats[stat]}降低！");

            // 保证提升等级次数最多是六次
            StatBoosts[stat] = Mathf.Clamp(StatBoosts[stat] + boost, -6, 6);
        }
    }


    void CalcuateStats()
    {
        PokemonStats = new Dictionary<Stats, int>();

        PokemonStats.Add(Stats.Attack, Mathf.FloorToInt(Base.Attack * Level / 100f) + 5);
        PokemonStats.Add(Stats.Defense, Mathf.FloorToInt(Base.Defense * Level / 100f) + 5);
        PokemonStats.Add(Stats.SpAttack, Mathf.FloorToInt(Base.SpAttack * Level / 100f) + 5);
        PokemonStats.Add(Stats.SpDefense, Mathf.FloorToInt(Base.SpDefense * Level / 100f) + 5);
        PokemonStats.Add(Stats.Speed, Mathf.FloorToInt(Base.Speed * Level / 100f) + 5);

        MaxHp = Mathf.FloorToInt(Base.MaxHp * Level / 100f) + 10 + level;
    }


    public void UpdateHp(int damage)
    {
        HpChange = true;
        Hp = Mathf.Clamp(Hp - damage, 0, MaxHp);
    }

    // 判断宝可梦有没有因为状态攻击不能行动
    public bool OnBeforeTurn()
    {
        bool canMove = true;
        // 判断普通状态是否可以移动
        if (Status?.OnBeforeTurn != null)
        {
            if (!Status.OnBeforeTurn(this))
                canMove = false;
        }

        // 判断不确定状态是否可以移动
        if (VolatileStatus?.OnBeforeTurn != null)
        {
            if (!VolatileStatus.OnBeforeTurn(this))
                canMove = false;
        }

        return canMove;
    }

    public void SetVolatileStatus(ConditionID conditionID)
    {
        if (VolatileStatus != null) return;

        VolatileStatus = ConditionsDB.conditions[conditionID];
        // 在受到状态攻击后执行OnStart 保存状态持续回合的随机数
        VolatileStatus?.OnStart?.Invoke(this);
        StatusMessage.Enqueue($"{Base.Name}{VolatileStatus.StartMessage}");

    }

    // 痊愈方法
    public void CureVolatileStatus()
    {
        VolatileStatus = null;
    }


    // 给宝可梦添加状态
    public void SetStatus(ConditionID conditionID)
    {
        // 保存每次只有一个状态
        if (Status != null) return;

        Status = ConditionsDB.conditions[conditionID];
        Status?.OnStart?.Invoke(this);
        StatusMessage.Enqueue($"{Base.Name}{Status.StartMessage}");

        // 施加状态调用事件 ？.表示非空判断
        OnStatusChange?.Invoke();
    }

    // 痊愈方法
    public void CureStatus()
    {
        Status = null;

        //痊愈也要调用事件更新状态
        OnStatusChange?.Invoke();
    }

    public void OnAfterTurn()
    {
        // 空条件运算符，判断数据用 ？. ， 判断方法？.invoke("...")
        Status?.OnAfterTurn?.Invoke(this);

        VolatileStatus?.OnAfterTurn?.Invoke(this);
    }

    // 更新PP
    public bool IsHavePP(Skill skill)
    {

        if (skill.pp <= 0)
        {
            return false;
        }
        else
        {
            skill.pp -= 1;
            return true;
        }


    }

    // 返回一个随机的宝可梦技能
    public Skill GetRandomSkill()
    {
        int index = Random.Range(0, Skills.Count);
        return Skills[index];
    }

    // 在战斗结束后，把状态技能的提升等级次数清零
    void ResetStatus()
    {
        // 初始化状态等级
        StatBoosts = new Dictionary<Stats, int>()
        {
            {Stats.Attack, 0},
            {Stats.Defense,0},
            {Stats.SpAttack,0},
            {Stats.SpDefense,0},
            {Stats.Speed,0},
        };
    }

    public void OnBattleOver()
    {
        // 战斗结束后不确定状态清空
        VolatileStatus = null;
        ResetStatus();
    }


    // 返回DamageDetails类
    public DamageDetails TakeDamage(Skill skill, Pokemon attacker)
    {
        // 当触发暴击，伤害为默认的一倍
        float critical = 1f;

        //random.value默认表示0-1.0f * 100f 表示0-100之间的浮点数几率很小
        if (Random.value * 100f <= 6.25f)
        {
            critical = 2f;
        }

        // 宝可梦默认俩个属性，所以使用的技能属性要和俩个宝可梦的属性都要判断，返回值相乘获得最后的属性加成值
        float type = TypeChart.GetEffectiveness(skill.SkillBase.Type, this.Base.Type1) * TypeChart.GetEffectiveness(skill.SkillBase.Type, this.Base.Type2);

        var damageDetails = new DamageDetails()
        {
            Critical = critical,
            TypeEffectiveness = type,
            Painted = false
        };

        //判断是否是特殊技能
        float attack = (skill.SkillBase.Category == SkillCategory.Special) ? attacker.SpAttack : attacker.Attack;
        float defense = (skill.SkillBase.Category == SkillCategory.Special) ? SpDefense : Defense;


        float modifiers = Random.Range(0.85f, 1f) * type * critical;
        float a = (2 * attacker.Level + 10) / 50f;
        float d = a * skill.SkillBase.Power * ((float)attack / defense) + 2;
        int damage = Mathf.FloorToInt(d * modifiers);

        UpdateHp(damage);

        return damageDetails;
    }


    /*
    * 返回对应等级的宝可梦的各种属性
    * 根据Mathf.FloorToInt((例如生命值*level)/100f + 10)
    * 其他则默认初始Mathf.FloorToInt((其他属性*level)/100f + 5)
    * 这样可以根据等级越高升一级加的属性越多
    */

    public int MaxHp
    {
        get;
        private set;
    }

    public int Attack
    {
        get { return GetStat(Stats.Attack); }
    }

    public int Defense
    {
        get { return GetStat(Stats.Defense); }
    }

    public int SpAttack
    {
        get { return GetStat(Stats.SpAttack); ; }
    }

    public int SpDefense
    {
        get { return GetStat(Stats.SpDefense); ; }
    }

    public int Speed
    {
        get { return GetStat(Stats.Speed); ; }
    }


}

// 用来保存暴击，属性克制，宝可梦是否血量为0这三个值的类
public class DamageDetails
{
    public float Critical { get; set; }
    public float TypeEffectiveness { get; set; }
    public bool Painted { get; set; }
}
