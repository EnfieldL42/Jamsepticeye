using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue Functions/End Game")]
public class DialogueActionEndGame : DialogueAction
{
    public override void OnButtonPressed()
    {
        base.OnButtonPressed();

        UIManager.Instance.CloseDialogue();
        GameManager.Instance.EndingSequence();
    }
}


