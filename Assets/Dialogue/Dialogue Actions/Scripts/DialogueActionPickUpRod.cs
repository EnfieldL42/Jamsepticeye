using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue Functions/PickUpRod")]
public class DialogueActionPickUpRod : DialogueAction
{
    public override void OnButtonPressed()
    {
        base.OnButtonPressed();

        FishingRodWorldObjectManager.Instance.PickUpFishingRod();
    }
}
