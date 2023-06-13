using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/*
 * 存储条件类的数据
 * 运用字典存储状态名 和 影响数值
 */
public class ConditionsDB : MonoBehaviour
{
    public static void Init()
    {
        foreach (var kvp in conditions)
        {
            var conditionId = kvp.Key;
            var condition = kvp.Value;

            condition.ID = conditionId;
        }
    }

    public static Dictionary<ConditionID, Condition> conditions = new Dictionary<ConditionID, Condition>()
    {
        {
            ConditionID.中毒,
            new Condition()
            {
                Name = "毒",
                StartMessage = "已经中毒了！",
                OnAfterTurn = (Pokemon pokemon) =>
                {
                    // 表示中毒每次扣整体生命值的八分之一
                    pokemon.UpdateHp(pokemon.MaxHp/8);
                    pokemon.StatusMessage.Enqueue($"{pokemon.Base.Name}因中毒受到了伤害！");
                }
            }
        },

        {
            ConditionID.燃烧,
            new Condition()
            {
                Name = "灼伤",
                StartMessage = "被灼伤了！",
                OnAfterTurn = (Pokemon pokemon) =>
                {
                    // 表示中毒每次扣整体生命值的十六分之一
                    pokemon.UpdateHp(pokemon.MaxHp/16);
                    pokemon.StatusMessage.Enqueue($"{pokemon.Base.Name}因灼伤受到了伤害！");
                }
            }
        },

        {
            ConditionID.麻痹,
            new Condition()
            {
                Name = "麻痹",
                StartMessage = "被麻痹了！",
                OnBeforeTurn = (Pokemon pokemon) =>
                {
                    /*当宝可梦被麻痹有四分之一的几率不能行动，
                     *true : 表示宝可梦可以移动
                     *false : 表示宝可梦不能移动
                     */
                    if(Random.Range(1,5) == 1)
                    {
                        pokemon.StatusMessage.Enqueue($"{pokemon.Base.Name}因麻痹不能行动了！");
                        return false;
                    }

                    return true;
                }
            }
        },

        {
            ConditionID.冻结,
            new Condition()
            {
                ID = ConditionID.冻结,
                Name = "冻结",
                StartMessage = "被冻住了！",
                OnBeforeTurn = (Pokemon pokemon) =>
                {
                    /*当宝可梦被麻痹有四分之一的痊愈，
                     *true : 表示有解除了冻结状态
                     *false : 表示没有解除
                     */
                    if(Random.Range(1,5) == 1)
                    {
                        // 如果痊愈了，就解除冻结状态
                        pokemon.CureStatus();
                        pokemon.StatusMessage.Enqueue($"{pokemon.Base.Name}解除了冻结状态！");
                        return true;
                    }

                    pokemon.StatusMessage.Enqueue($"{pokemon.Base.Name} 因冻住而不能行动！");
                    return false;
                }
            }
        },

         {
            ConditionID.睡眠,
            new Condition()
            {
                Name = "睡眠",
                StartMessage = "睡着了！",
                // 当宝可梦进入睡眠，获取1-3之间的睡眠随机数
                OnStart = (Pokemon pokemon) =>
                {
                    pokemon.StatusTime = Random.Range(1,4);
                },
                OnBeforeTurn = (Pokemon pokemon) =>
                {
                    if(pokemon.StatusTime <= 0)
                    {
                        pokemon.CureStatus();
                        pokemon.StatusMessage.Enqueue($"{pokemon.Base.Name}醒来了！");
                        return true;
                    }

                    pokemon.StatusTime --;
                    pokemon.StatusMessage.Enqueue($"{pokemon.Base.Name}进入了梦乡！");
                    return false;
                }
            }
        },

        {
            ConditionID.混乱,
            new Condition()
            {
                Name = "混乱",
                StartMessage = "混乱了！",
                OnStart = (Pokemon pokemon) =>
                {
                   pokemon.VolatileStatusTime = Random.Range(1,4);
                },
                OnBeforeTurn = (Pokemon pokemon) =>
                {
                    if(pokemon.VolatileStatusTime <=0)
                    {
                        pokemon.CureVolatileStatus();
                        pokemon.StatusMessage.Enqueue($"{pokemon.Base.Name}解除了混乱状态！");
                        return true;
                    }
                    //50%的机会攻击
                    if(Random.Range(1,4) == 1)
                        return true;

                    // 混乱攻击自己
                    pokemon.VolatileStatusTime--;
                    pokemon.StatusMessage.Enqueue($"{pokemon.Base.Name}陷入了混乱状态！");
                    pokemon.StatusMessage.Enqueue($"{pokemon.Base.Name}因混乱而攻击自己！");
                    pokemon.UpdateHp(pokemon.MaxHp/8);
                    return false;
                }

            }
        }


    };
}

public enum ConditionID
{
    空, 中毒, 燃烧, 睡眠, 麻痹, 冻结, 混乱
}
