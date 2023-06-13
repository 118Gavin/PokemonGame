using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PartyMemberUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] HPbar HPbar;
    [SerializeField] Color highColor;
    [SerializeField] Color originColor;

    private Pokemon _pokemon;

    public void SetData(Pokemon pokemon)
    {
        this._pokemon = pokemon;
        nameText.text = pokemon.Base.Name;
        levelText.text = "Lv " + pokemon.Level;
        HPbar.SetHp((float)pokemon.Hp / pokemon.MaxHp);
    }

    public void ChangeTextColor(bool selected)
    {
        if (selected)
            nameText.color = highColor;
        else
            nameText.color = originColor;
    }
}
