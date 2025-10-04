using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue Functions/Calls test")]
public class DialogueActionTest : DialogueAction
{
    public override void OnButtonPressed()
    {
        base.OnButtonPressed();

        UIManager.Instance.Test();
    }
}
