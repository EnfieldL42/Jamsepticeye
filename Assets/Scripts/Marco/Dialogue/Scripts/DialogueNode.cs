using UnityEngine;

public class DialogueNode : ScriptableObject
{
    public string SpeakerName;
    [TextArea] public string DialogueText;
    public bool ClosesDialogue;
}