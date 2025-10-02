using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Regular Dialogue", menuName = "Dialogue/New Regular Dialogue")]
public class DialogueObject : ScriptableObject
{
    public string SpeakerName;
    [TextArea] public string DialogueText;
    public DialogueObject ProceedingDialogue;
}

[CreateAssetMenu(fileName = "New Dialogue Option", menuName = "Dialogue/New Dialogue Option")]
public class DialogueOption : ScriptableObject
{
    public string OptionText;
    public bool ClosesDialogue = false;

    public DialogueObject ProceedingDialogue;
}

[CreateAssetMenu(fileName = "New Dialogue with Options", menuName = "Dialogue/New Dialogue with Options")]
public class DialogueOptionsObject : ScriptableObject
{
    public string SpeakerName;
    [TextArea] public string DialogueText;
    public List<DialogueOption> DialogueOptions = new List<DialogueOption>();
}