using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PartyScreen : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI messageText;
    PartyMemberUI[] memberList;

    public void Init()
    {
        memberList = GetComponentsInChildren<PartyMemberUI>();
    }

    public void SetPartyData(List<TerasCalcs> teras)
    {
        for(int i = 0; i <= memberList.Length; i++)
        {
            if (i < teras.Count)
                memberList[i].setData(teras[i]);
            else 
                memberList[i].gameObject.SetActive(false);
        }
        messageText.text = "Choose a Teras!";
    }
}
