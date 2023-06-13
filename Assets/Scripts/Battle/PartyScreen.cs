using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PartyScreen : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI messageText;
    PartyMemberUI[] partyMemberUIs;
    List<Pokemon> pokemons;

    public List<Pokemon> Pokemons
    {
        get { return pokemons; }
    }

    public PartyMemberUI[] PartyMemberUIs
    {
        get { return partyMemberUIs; }
    }
    //[SerializeField] List<PartyMemberUI> m_PartyMembers;

    public void init()
    {
        partyMemberUIs = GetComponentsInChildren<PartyMemberUI>();
    }

    public void SetPartyData(List<Pokemon> pokemons)
    {
        this.pokemons = pokemons;
        for (int i = 0; i < partyMemberUIs.Length; i++)
        {
            if (i < pokemons.Count)
                partyMemberUIs[i].SetData(pokemons[i]);
            else
                partyMemberUIs[i].gameObject.SetActive(false);
        }

        messageText.text = "请选择一个宝可梦！";
    }

    public void UpdatePartyMember(int SelectedMember)
    {
        for (int i = 0; i < pokemons.Count; i++)
        {
            if (i == SelectedMember)
                partyMemberUIs[i].ChangeTextColor(true);
            else
                partyMemberUIs[i].ChangeTextColor(false);
        }
    }

    public void SetMessage(string messageText)
    {
        this.messageText.text = messageText;
    }
}
