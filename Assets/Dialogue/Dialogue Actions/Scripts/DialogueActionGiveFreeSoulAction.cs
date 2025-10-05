using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue Functions/Check for free option")]
public class DialogueActionGiveFreeSoulAction : DialogueAction
{
    public override void OnButtonPressed()
    {
        base.OnButtonPressed();

        PandorasManager.Instance.CheckIfGiveFreeSoulOption();
    }
}



