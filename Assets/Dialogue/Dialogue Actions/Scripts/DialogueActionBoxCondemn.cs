using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue Functions/BoxCondemn")]
public class DialogueActionBoxCondemn : DialogueAction
{
    public override void OnButtonPressed()
    {
        base.OnButtonPressed();

        GameManager.Instance.CondemnSoul();
    }
}

