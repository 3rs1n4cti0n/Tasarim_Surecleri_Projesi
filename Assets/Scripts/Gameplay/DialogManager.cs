using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class DialogManager : MonoBehaviour
{
    [SerializeField] GameObject dialogBox;
    [SerializeField] TextMeshProUGUI dialogText;
    [SerializeField] int lettersPerSecond;
    public event Action OnDialogOpen;
    public event Action OnDialogClose;
    public static DialogManager Instance { get; private set; }
    dialog Dialog;
    int currentLine;
    bool isTyping;
    private void Awake()
    {
        Instance = this;
    }

    public void HandleUpdate()
    {
        if(Input.GetKeyDown(KeyCode.E) && !isTyping)
        {
            currentLine++;
            if(currentLine < Dialog.Lines.Count)
            {
                StartCoroutine(TypeDialog(Dialog.Lines[currentLine]));
            }else
            {
                currentLine = 0;
                dialogBox.SetActive(false);
                OnDialogClose?.Invoke();
            }
        }
    }

    public IEnumerator showDialog(dialog dialog)
    {
        yield return new WaitForEndOfFrame();
        OnDialogOpen?.Invoke();
        this.Dialog = dialog;
        dialogBox.SetActive(true);
        StartCoroutine(TypeDialog(dialog.Lines[0]));
    }

    // Coroutine to type the dialog slowly
    public IEnumerator TypeDialog(string line)
    {
        isTyping = true;
        dialogText.text = "";

        // for loop to write dialog string slowly
        foreach (var letter in line.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }
        isTyping = false;
    }
}
