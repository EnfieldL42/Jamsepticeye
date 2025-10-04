using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue Functions/BoxFree")]
public class DialogueActionBoxFree : DialogueAction
{
    public override void OnButtonPressed()
    {
        base.OnButtonPressed();

        GameManager.Instance.FreeSoul();
    }
}

