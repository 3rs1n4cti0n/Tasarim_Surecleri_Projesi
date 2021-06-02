using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PartyScreen : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI messageText;
    PartyMemberUI[] memberList;
    List<TerasCalcs> teras;

    public void Init()
    {
        memberList = GetComponentsInChildren<PartyMemberUI>();
    }

    public void SetPartyData(List<TerasCalcs> teras)
    {
        this.teras = teras;

        for(int i = 0; i < memberList.Length; i++)
        {
            if (i < teras.Count)
                memberList[i].setData(teras[i]);
            else
                memberList[i].gameObject.SetActive(false);
        }
        messageText.text = "Choose a Teras!";
    }
    public void UpdateMemberSelection(int selectedMember)
    {
        for (int i = 0; i < teras.Count;i++)
        {
            if (i == selectedMember)
                memberList[i].HighlightSelectedMember(true);
            else
                memberList[i].HighlightSelectedMember(false);
        }
    }

    public void SetMessageText(string message)
    {
        messageText.text = message;
    }
}
