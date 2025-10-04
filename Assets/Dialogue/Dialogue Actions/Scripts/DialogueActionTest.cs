using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue Start Functions/Calls test")]
public class DialogueActionTest : DialogueStartAction
{
    public override void OnDialogueStarted()
    {
        base.OnDialogueStarted();

        UIManager.Instance.Test();
    }
}
