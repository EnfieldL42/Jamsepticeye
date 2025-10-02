using UnityEngine;

public class InteractTest : MonoBehaviour, IInteractable
{
    public InteractionData Data;
    public DialogueNode StartingDialogue;

    public void OnInteracted(PlayerHandler Player)
    {
        UIManager.Instance.StartDialogue(StartingDialogue);
        PlayerHandler.Instance.SetInteractionListening(false);

        UIManager.Instance.HideInteractionPrompt();
        PlayerMovement.Instance.MovementDisabled = true;
    }
    
    public string GetInteractionPrompt(PlayerHandler Player)
    {
        bool RequireHold = Data.HoldTime > 0;
        return RequireHold ? $"- {Data.Name} -\n\nHold [{Player.InteractionKeybind}] to {Data.Description}" : $"- {Data.Name} -\n\nPress [{Player.InteractionKeybind}] to {Data.Description}";
    }

    public float GetHoldTime(PlayerHandler Player)
    {
        return Data.HoldTime;
    }
}
