using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour, Interactable
{
    [SerializeField] dialog Dialog;
    public void Interact()
    {
        StartCoroutine(DialogManager.Instance.showDialog(Dialog));
    }
}
