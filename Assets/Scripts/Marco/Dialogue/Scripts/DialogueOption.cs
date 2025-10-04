using UnityEngine;
using System;

[Serializable]
public class DialogueOption
{
    public string OptionText;
    public bool ClosesDialogue = false;

    public DialogueNode ProceedingDialogue;
    public DialogueAction Function;
}