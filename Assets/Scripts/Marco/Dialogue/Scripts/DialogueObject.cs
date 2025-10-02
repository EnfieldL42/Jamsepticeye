using UnityEngine;
using System.Collections.Generic;

public enum DialogueType
{
    RegularDialogue,
    DialogueButton,
    DialogueOptions
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

[CreateAssetMenu(menuName = "Dialogue/New Dialogue Option")]
public class DialogueOption : ScriptableObject
{
    public string OptionText;
    public bool ClosesDialogue = false;

    public DialogueNode ProceedingDialogue;
}

[CreateAssetMenu(menuName = "Dialogue/New Dialogue with Options")]
public class DialogueOptionsObject : DialogueNode
{
    public List<DialogueOption> DialogueOptions = new List<DialogueOption>();
}