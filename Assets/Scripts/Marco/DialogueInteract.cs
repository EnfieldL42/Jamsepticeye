using UnityEngine;

public class DialogueInteract : MonoBehaviour, IInteractable
{
    public InteractionData Data;
    public DialogueNode StartingDialogue;

    public void OnInteracted(PlayerHandler Player)
    {
        UIManager.Instance.StartDialogue(StartingDialogue, Data.Name);
        PlayerHandler.Instance.SetInteractionListening(false);

        UIManager.Instance.HideInteractionPrompt();
        PlayerMovement.Instance.MovementDisabled = true;
        Data.Function?.OnDialogueStarted();
    }
    
    public string GetInteractionPrompt(PlayerHandler Player)
    {
        bool RequireHold = Data.HoldTime > 0;
        string DialogueName = string.IsNullOrEmpty(Data.Name) ? "" : $"- {Data.Name} - ";
        return RequireHold ? $"{DialogueName}\n\nHold [{Player.InteractionKeybind}] to {Data.Description}" : $"{DialogueName}\n\nPress [{Player.InteractionKeybind}] to {Data.Description}";
    }

    public float GetHoldTime(PlayerHandler Player)
    {
        return Data.HoldTime;
    }
}
