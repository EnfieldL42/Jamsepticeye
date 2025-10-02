using UnityEngine;

[CreateAssetMenu(menuName = "Interaction/New Interaction Data")]
public class InteractionData : ScriptableObject
{
    public string Name = "";
    public float HoldTime = 0f;
    [TextArea] public string Description;
}
