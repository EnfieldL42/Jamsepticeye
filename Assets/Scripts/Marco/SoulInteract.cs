using UnityEngine;
using UnityEngine.InputSystem;

public class SoulInteract : MonoBehaviour
{
    public InteractionData Data;
    public DialogueNode StartingDialogue;

    private PlayerControls Controls;
    public bool UIEnabled = false;

    private void OnDisable()
    {
        Controls.PlayerActions.Interact.started -= Interact;
    }

    private void Interact(InputAction.CallbackContext ctx)
    {
        print("Interacted");
        UIManager.Instance.StartDialogue(StartingDialogue, Data.Name);
        DisableInteractionUI();
        //PlayerHandler.Instance.SetInteractionListening(false);

        //UIManager.Instance.HideInteractionPrompt();
        PlayerMovement.Instance.MovementDisabled = true;
        Data.Function?.OnDialogueStarted();
    }

    public void EnableInteractionUI()
    {
        UIManager.Instance.SoulInteractionText.SetText($" - {Data.Name} - \n [{PlayerHandler.Instance.InteractionKeybind}] Talk");
        UIManager.Instance.SoulInteractionUI.SetActive(true);

        Controls = PlayerInputManager.playerInputManager.playerControls;
        Controls.Enable();
        Controls.PlayerActions.Interact.started += Interact;

        UIEnabled = true;
    }

    public void DisableInteractionUI()
    {
        Controls.PlayerActions.Interact.started -= Interact;
        UIEnabled = false;
        UIManager.Instance.SoulInteractionUI.SetActive(false);
    }

    private void Update()
    {
        Vector3 ScreenPosition = PlayerHandler.Instance.BasePlayerCamera.WorldToScreenPoint(transform.position - transform.up * 0.3f);

        var UI = UIManager.Instance.SoulInteractionUI.transform;

        UI.position = Vector3.Lerp(UI.position, ScreenPosition, 20f * Time.deltaTime);
    }

}
