using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue Start Functions/Reaper Answered")]
public class DialogueActionDeathCall : DialogueStartAction
{
    public override void OnDialogueStarted()
    {
        base.OnDialogueStarted();

        ReaperManager.Instance.GoNextDialogue();
        ReaperManager.Instance.StopCalling();
        ReaperManager.Instance.HideCollider();
        FishingRodWorldObjectManager.Instance.enableCollider = true;
    }
}
