using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue Start Functions/Calls ButtonFunctionExample")]
public class DialogueActionExample : DialogueStartAction
{
    public override void OnDialogueStarted()
    {
        base.OnDialogueStarted();

        UIManager.Instance.ButtonFunctionExample();
    }
}
