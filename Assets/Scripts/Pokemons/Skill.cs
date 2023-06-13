using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill
{
    public SkillBase SkillBase { get; set; }

    public int pp { get; set; }

    public Skill(SkillBase skillBase)
    {
        SkillBase = skillBase;
        pp =skillBase.Pp;
    }


}
