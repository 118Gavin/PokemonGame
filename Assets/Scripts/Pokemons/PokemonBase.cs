using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Pokemon", menuName = "Pokemon/Create a pokemon")]
public class PokemonBase : ScriptableObject
{
    [SerializeField] string pokemonName;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] Sprite backSprite;
    [SerializeField] Sprite frontSprite;

    //先默认一个精灵是俩个属性
    [SerializeField] PokemonType type1; // 主属性
    [SerializeField] PokemonType type2; // 副属性

    [SerializeField] int maxHp;
    [SerializeField] int attack;
    [SerializeField] int defense;
    [SerializeField] int spAttack;
    [SerializeField] int spDefense;
    [SerializeField] int speed;

    [SerializeField] List<learnSkill> learnSkills;//用来保存当前宝可梦的所有可以学习的技能


    //运用属性设置对应的成员变量的属性
    public string Name
    {
        get { return pokemonName; }
    }

    public string Description
    {
        get { return description; }
    }

    public Sprite BackSprite
    {
        get { return backSprite; }
    }

    public Sprite FrontSprite
    {
        get { return frontSprite; }
    }

    public PokemonType Type1
    {
        get { return type1; }
    }

    public PokemonType Type2
    {
        get { return type2; }
    }

    public int MaxHp
    {
        get { return maxHp; }
    }

    public int Attack
    {
        get { return attack; }
    }

    public int Defense
    {
        get { return defense; }
    }

    public int SpAttack
    {
        get { return spAttack; }
    }

    public int SpDefense
    {
        get { return spDefense; }
    }

    public int Speed
    {
        get { return speed; }
    }

    public List<learnSkill> LearnSkills
    {
        get { return learnSkills; }
    }
}


[System.Serializable]
public class learnSkill
{
    [SerializeField] SkillBase skillBase;
    [SerializeField] int level;

    public SkillBase SkillBase
    {
        get { return skillBase; }
    }

    public int Level
    {
        get { return level; }
    }

}
public enum PokemonType
{
    一般,
    火,
    水,
    草,
    电,
    冰,
    虫,
    飞行,
    地面,
    岩石,
    格斗,
    超能力,
    幽灵,
    毒,
    恶,
    钢,
    龙,
    妖精
}

// 战斗时能被削弱或加强的属性枚举
public enum Stats { Attack, Defense, SpAttack, SpDefense, Speed }

// 属性克制表
public class TypeChart
{


    static float[][] chart =
    {  //                   一般   火     水    草    电    冰    虫   飞行   地面  岩石  格斗  超能力  幽灵   毒    恶    钢    龙    妖精
      /*一般*/ new float[] { 1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f, 0.5f,   1f,   1f,   0f,   1f,   1f, 0.5f,   1f,   1f  },
       /*火*/  new float[] { 1f, 0.5f, 0.5f,   2f,   1f,   2f,   2f,   1f,   1f, 0.5f,   1f,   1f,   1f,   1f,   1f,   2f, 0.5f,   1f  },
       /*水*/  new float[] { 1f,   2f, 0.5f, 0.5f,   1f,   1f,   1f,   1f,   2f,   2f,   1f,   1f,   1f,   1f,   1f,   1f, 0.5f,   1f  },
       /*草*/  new float[] { 1f, 0.5f,   2f, 0.5f,   1f,   1f, 0.5f, 0.5f,   2f,   2f,   1f,   1f,   1f, 0.5f,   1f, 0.5f, 0.5f,   1f  },
       /*电*/  new float[] { 1f,   1f,   2f, 0.5f, 0.5f,   1f,   1f,   2f,   0f,   1f,   1f,   1f,   1f,   1f,   1f,   1f, 0.5f,   1f  },
       /*冰*/  new float[] { 1f, 0.5f, 0.5f,   2f,   1f, 0.5f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f  },
       /*虫*/  new float[] { 1f, 0.5f,   1f,   2f,   1f,   1f,   1f, 0.5f,   1f,   1f, 0.5f,   2f, 0.5f, 0.5f,   2f, 0.5f,   1f, 0.5f  },
     /*飞行*/  new float[] { 1f,   1f,   1f,   2f, 0.5f,   1f,   1f,   1f,   1f, 0.5f,   2f,   1f,   1f,   1f,   1f, 0.5f,   1f,   1f  },
     /*地面*/  new float[] { 1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f, 0.5f,   1f,   1f,   1f,   1f,   1f, 0.5f,   1f,   1f  },
     /*岩石*/  new float[] { 1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f, 0.5f,   1f,   1f,   0f,   1f,   1f, 0.5f,   1f,   1f  },
     /*格斗*/  new float[] { 1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f, 0.5f,   1f,   1f,   0f,   1f,   1f, 0.5f,   1f,   1f  },
   /*超能力*/  new float[] { 1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f, 0.5f,   1f,   1f,   0f,   1f,   1f, 0.5f,   1f,   1f  },
     /*幽灵*/  new float[] { 1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f, 0.5f,   1f,   1f,   0f,   1f,   1f, 0.5f,   1f,   1f  },
       /*毒*/  new float[] { 1f,   1f,   1f,   2f,   1f,   1f,   1f,   1f,   1f, 0.5f,   1f,   1f, 0.5f, 0.5f,   1f,   0f,   1f,   2f  },  
       /*恶*/  new float[] { 1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f, 0.5f,   1f,   1f,   0f,   1f,   1f, 0.5f,   1f,   1f  },
       /*钢*/  new float[] { 1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f, 0.5f,   1f,   1f,   0f,   1f,   1f, 0.5f,   1f,   1f  },
       /*龙*/  new float[] { 1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f, 0.5f,   1f,   1f,   0f,   1f,   1f, 0.5f,   1f,   1f  },
     /*妖精*/  new float[] { 1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f, 0.5f,   1f,   1f,   0f,   1f,   1f, 0.5f,   1f,   1f  },
    };

    public static float GetEffectiveness(PokemonType attacktype, PokemonType defenseType)
    {
        if (attacktype == PokemonType.一般 || defenseType == PokemonType.一般)
            return 1f;

        int row = (int)attacktype;
        int col = (int)defenseType;

        return chart[row][col];
    }
}