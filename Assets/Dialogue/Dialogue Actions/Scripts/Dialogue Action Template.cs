using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue Functions/Template")]
public class DialogueActionTemplate : DialogueAction
{
    public override void OnButtonPressed()
    {
        base.OnButtonPressed();

        UIManager.Instance.ButtonFunctionExample();
    }
}
