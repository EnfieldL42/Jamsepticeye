public interface IInteractable
{
    void OnInteracted(PlayerHandler Player);
    string GetInteractionPrompt(PlayerHandler Player);
    float GetHoldTime(PlayerHandler Player);
}
