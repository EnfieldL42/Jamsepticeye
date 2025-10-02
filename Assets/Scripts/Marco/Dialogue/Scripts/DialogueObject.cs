using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class DialogueOption
{
    public string OptionText;
    public bool ClosesDialogue = false;

    public DialogueNode ProceedingDialogue;
}

public abstract class DialogueNode : ScriptableObject
{
    public string SpeakerName;
    [TextArea] public string DialogueText;
    public bool ClosesDialogue;
}

[CreateAssetMenu(menuName = "Dialogue/New Regular Dialogue")]
public class DialogueObject : DialogueNode
{
    public DialogueNode ProceedingDialogue;
}

[CreateAssetMenu(menuName = "Dialogue/New Dialogue with Options")]
public class DialogueOptionsObject : DialogueNode
{
    public List<DialogueOption> DialogueOptions = new List<DialogueOption>();
}