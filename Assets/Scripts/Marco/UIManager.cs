using Febucci.UI;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    private Animator CanvasAnimator;

    private PlayerControls Controls;
    private bool InTypewriter = false;
    private DialogueNode CurrentDialogueNode;

    [Header("Stats UI")]
    public TextMeshProUGUI FPSText;

    [Space, Header("Interact UI")]
    [SerializeField] private GameObject InteractionHolder;
    [SerializeField] private TextMeshProUGUI InteractionText;

    [Space, Header("Dialogue UI")]
    [SerializeField] private TypewriterByCharacter DialogueText;
    [SerializeField] private TextMeshProUGUI DialogueName;

    [SerializeField] private Transform DialogueOptionsHolder;
    [SerializeField] private DialogueButton DialogueOptionPrefab;

    public DialogueObject test;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        CanvasAnimator = GetComponent<Animator>();
        StartCoroutine(WaitForControls());
    }

    private IEnumerator WaitForControls()
    {
        while (PlayerInputManager.playerInputManager == null || !PlayerInputManager.playerInputManager.ControlsEnabled)
        {
            yield return null;
        }

        Controls = PlayerInputManager.playerInputManager.playerControls;
        Controls.Enable();

        Controls.PlayerActions.DialogueSkip.performed += ProceedDialogue;
        StartDialogue(test);
    }

    private void OnDisable()
    {
        Controls.PlayerActions.DialogueSkip.performed -= ProceedDialogue;
    }

    public void ShowInteractionPrompt(string Text = default)
    {
        InteractionText.SetText(Text);
        InteractionHolder.SetActive(true);
    }

    public void HideInteractionPrompt()
    {
        InteractionHolder.SetActive(false);
    }

    private void ClearDialogue()
    {
        DialogueText.ShowText("");
        DialogueName.SetText("");

        for (int i = DialogueOptionsHolder.childCount - 1; i >= 0; i--)
        {
            Destroy(DialogueOptionsHolder.GetChild(i).gameObject);
        }
    }

    private void CloseDialogue()
    {
        CanvasAnimator.SetBool("ShowDialogue", false);
    }

    private void ProceedDialogue(InputAction.CallbackContext ctx)
    {
        if (InTypewriter)
        {
            DialogueText.SkipTypewriter();
        }
        else
        {
            if (CurrentDialogueNode.ClosesDialogue)
            {
                CloseDialogue();
                return;
            }
            else
            {
                if (CurrentDialogueNode is DialogueObject RegularDialogue)
                {
                    if (RegularDialogue.ProceedingDialogue != null)
                    {
                        //ShowRegularDialogue(RegularDialogue.ProceedingDialogue);
                    }
                    return;
                }
                else if (CurrentDialogueNode is DialogueOptionsObject OptionsDialogue)
                {
                    ShowOptionsDialogue(OptionsDialogue);
                    return;
                }
            }
        }
    }

    public void DialogueTextFinished()
    {
        InTypewriter = false;
        print("Dun");
    }

    private void ShowRegularDialogue(DialogueObject Data = default)
    {
        InTypewriter = true;
        DialogueName.SetText(Data.SpeakerName);

        DialogueText.ShowText(Data.DialogueText);
        DialogueText.StartShowingText();
    }

    private void ShowOptionsDialogue(DialogueOptionsObject Data = default)
    {
        InTypewriter = true;
        DialogueName.SetText(Data.SpeakerName);

        DialogueText.ShowText(Data.DialogueText);
        DialogueText.StartShowingText();
    }

    public void StartDialogue(DialogueObject DialogueData = default)
    {
        if (DialogueData == null) return;

        ClearDialogue();
        CanvasAnimator.SetBool("ShowDialogue", true);

        CurrentDialogueNode = DialogueData;

        if (CurrentDialogueNode is DialogueObject RegularDialogue)
        {
            ShowRegularDialogue(RegularDialogue);
            return;
        }
        else if (CurrentDialogueNode is DialogueOptionsObject OptionsDialogue)
        {
            ShowOptionsDialogue(OptionsDialogue);
            return;
        }
    }
}
