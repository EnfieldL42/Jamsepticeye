using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Dialogue/New Dialogue with Options")]
public class DialogueOptionsObject : DialogueNode
{
    public List<DialogueOption> DialogueOptions = new List<DialogueOption>();
}