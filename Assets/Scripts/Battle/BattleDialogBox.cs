using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BattleDialogBox : MonoBehaviour
{
    //给一个值，1秒读lettersPerSecond数量的字
    [SerializeField] int lettersPerSecond;
    [SerializeField] Color changeColor;
    [SerializeField] Color initColor;

    [SerializeField] TextMeshProUGUI dialogBoxText;

    [SerializeField] GameObject actionSelector;
    [SerializeField] GameObject skillSelector;
    [SerializeField] GameObject skillDetails;

    [SerializeField] List<TextMeshProUGUI> actionTexts;
    [SerializeField] List<TextMeshProUGUI> skillTexts;

    [SerializeField] TextMeshProUGUI ppText;
    [SerializeField] TextMeshProUGUI typeText;

    public  List<TextMeshProUGUI> ActionTexts { get { return actionTexts; } }

    public void SetDialogText(string dialog)
    {
        dialogBoxText.text = dialog;
    }

    public IEnumerator TypeDialog(string dialog)
    {
        dialogBoxText.text = "";
        foreach (var letter in dialog.ToCharArray())
        {
            dialogBoxText.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }
        yield return new WaitForSeconds(1f);
    }

    // 控制DialogText的显示
    public void EnableDialogText(bool enable)
    {
        dialogBoxText.enabled = enable;
    }

    // 控制ActionSelector的显示
    public void EnableActionSelector(bool enable)
    {
        actionSelector.SetActive(enable);
    }

    // 控制skillSelector和skillDetails的显示
    public void EnableSkillSelector(bool enable)
    {
        skillSelector.SetActive(enable);
        skillDetails.SetActive(enable);
    }

    public void UpdateActionSelector(int actionSelectorIndex)
    {

        for (int i = 0; i < actionTexts.Count; i++)
        {
            if (i == actionSelectorIndex)
                actionTexts[i].color = changeColor;
            else
                actionTexts[i].color = initColor;
        }
    }

    public void UpdateSkillSelector(int skillSelectorIndex, Skill skill)
    {
        for (int i = 0; i < skillTexts.Count; i++)
        {
            if (i == skillSelectorIndex)
            {
                skillTexts[i].color = changeColor; 
            }
            else
                skillTexts[i].color = initColor;
        }

        ppText.text = "PP " + skill.SkillBase.Pp.ToString() + "/" + skill.pp;
        typeText.text = "属性/" + skill.SkillBase.Type.ToString();
    }


    public void SetSkillNames(List<Skill> skills)
    {
        for (int i = 0; i < skillTexts.Count; i++)
        {
            // 因为技能保存可能就三四个
            // 判定小于把存在的几个先保存，不存在的用"-"保存
            if (i < skills.Count)
                skillTexts[i].text = skills[i].SkillBase.Name;
            else
                skillTexts[i].text = "-";
        }
    }
}
