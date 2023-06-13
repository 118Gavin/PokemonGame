using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleHub : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI statusText;
    [SerializeField] Image statusImageBg;
    [SerializeField] HPbar HPbar;
    [SerializeField] Color noneColor;
    [SerializeField] Color posColor;
    [SerializeField] Color burnColor;
    [SerializeField] Color sleepColor;
    [SerializeField] Color palsyColor;
    [SerializeField] Color freezeColor;

    private Dictionary<ConditionID, Color> statusColors;
    private Pokemon _pokemon;

    public void SetData(Pokemon pokemon)
    {
        this._pokemon = pokemon;
        nameText.text = pokemon.Base.Name;
        levelText.text = pokemon.Level.ToString();
        HPbar.SetHp((float)pokemon.Hp / pokemon.MaxHp);
        statusColors = new Dictionary<ConditionID, Color>()
        {
            {ConditionID.空, noneColor },
            {ConditionID.中毒, posColor },
            {ConditionID.燃烧, burnColor },
            {ConditionID.睡眠, sleepColor },
            {ConditionID.麻痹, palsyColor },
            {ConditionID.冻结, freezeColor }
        };

        //因为状态不接触就会一直存在，每次战斗开始前就会继续该状态
        SetStatusTextAndBg();

        // 将方法保存到事件里
        _pokemon.OnStatusChange += SetStatusTextAndBg;

    }

    public void SetStatusTextAndBg()
    {
        if (_pokemon.Status == null)
        {
            statusText.text = "";
            statusImageBg.color = statusColors[ConditionID.空];
        }

        else
        {
            statusText.text = _pokemon.Status.ID.ToString();
            statusImageBg.color = statusColors[_pokemon.Status.ID];
        }
    }

    public IEnumerator UpdateHP()
    {
        // 当生命值变化的时候才更新血条
        if (_pokemon.HpChange)
        {
            yield return HPbar.SetUpSmooth((float)_pokemon.Hp / _pokemon.MaxHp);
            _pokemon.HpChange = false;
        }
    }
}
