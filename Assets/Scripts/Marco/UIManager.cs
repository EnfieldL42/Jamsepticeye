using Febucci.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    private Animator CanvasAnimator;

    private PlayerControls Controls;
    private bool InTypewriter = false;

    private DialogueNode CurrentDialogueNode;
    private List<DialogueButton> ActiveOptions = new List<DialogueButton>();

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
    private string CurrentNPCName = "";

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
    }

    private void Start()
    {
        Controls = PlayerInputManager.playerInputManager.playerControls;
        Controls.PlayerActions.DialogueSkip.started += ProceedDialogue;
    }

    private void OnDisable()
    {
        Controls.PlayerActions.DialogueSkip.started -= ProceedDialogue;
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
        CameraMove.Instance.PlayerControlsCamera = true;
        GameManager.Instance.SetCurstorState(CursorLockMode.Locked, false);

        PlayerHandler.Instance.SetInteractionListening(true);
        PlayerMovement.Instance.MovementDisabled = false;
        CurrentNPCName = "";
    }

    private void SetNextDialogue()
    {
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

    private void ProceedDialogue(InputAction.CallbackContext ctx)
    {
        if (InTypewriter)
        {
            DialogueText.SkipTypewriter();
        }
        else
        {
            if (CurrentDialogueNode is DialogueObject RegularDialogue)
            {
                if (CurrentDialogueNode.ClosesDialogue)
                {
                    CloseDialogue();
                    return;
                }
                else
                {
                    if (RegularDialogue.ProceedingDialogue != null)
                    {
                        CurrentDialogueNode = RegularDialogue.ProceedingDialogue;
                        SetNextDialogue();
                    }
                    return;
                }
            }
        }
    }

    public void DialogueTextFinished()
    {
        InTypewriter = false;

        if (CurrentDialogueNode is DialogueOptionsObject OptionsDialogue)
        {
            if (OptionsDialogue.DialogueOptions.Count <= 0)
            {
                CloseDialogue();
                return;
            }

            foreach (DialogueOption DialogueOption in OptionsDialogue.DialogueOptions)
            {
                DialogueButton NewButtton = Instantiate(DialogueOptionPrefab, DialogueOptionsHolder);
                NewButtton.ButtonText.SetText(DialogueOption.OptionText);

                NewButtton.ProceedingDialogue = DialogueOption.ProceedingDialogue;
                NewButtton.Button.onClick.AddListener(() => DialogueOptionPressed(NewButtton));

                NewButtton.Option = DialogueOption;
                ActiveOptions.Add(NewButtton);
            }
        }
    }

    private void DialogueOptionPressed(DialogueButton ButtonData)
    {
        if (ButtonData.ProceedingDialogue == null)
        {
            CloseDialogue();
            return;
        }
        else
        {
            ButtonData.Option.Function?.OnButtonPressed();

            CurrentDialogueNode = ButtonData.ProceedingDialogue;

            foreach (DialogueButton OptionButton in ActiveOptions)
            {
                OptionButton.Button.onClick.RemoveAllListeners();
            }

            for (int i = ActiveOptions.Count - 1; i >= 0; i--)
            {
                if (ActiveOptions[i] != null)
                {
                    Destroy(ActiveOptions[i].gameObject);
                }
            }

            ActiveOptions.Clear();
            print(ActiveOptions.Count);

            SetNextDialogue();
            return;
        }
    }

    private void ShowRegularDialogue(DialogueObject Data = default)
    {
        InTypewriter = true;
        DialogueName.gameObject.SetActive(!string.IsNullOrEmpty(Data.SpeakerName));

        if (Data.SpeakerName.ToLower() == "{npc_name}")
        {
            DialogueName.SetText(CurrentNPCName);
        }
        else
        {
            DialogueName.SetText(Data.SpeakerName);
        }
        DialogueText.ShowText(Data.DialogueText);
        DialogueText.StartShowingText();
    }

    private void ShowOptionsDialogue(DialogueOptionsObject Data = default)
    {
        InTypewriter = true;

        if (Data.SpeakerName.ToLower() == "{npc_name}")
        {
            DialogueName.SetText(CurrentNPCName);
        }
        else
        {
            DialogueName.SetText(Data.SpeakerName);
        }

        DialogueText.ShowText(Data.DialogueText);
        DialogueText.StartShowingText();
    }

    public void StartDialogue(DialogueNode DialogueData = default, string NPCName = default)
    {
        if (DialogueData == null) return;
        CurrentNPCName = NPCName;

        CameraMove.Instance.PlayerControlsCamera = false;
        GameManager.Instance.SetCurstorState(CursorLockMode.None, true);

        ClearDialogue();
        CanvasAnimator.SetBool("ShowDialogue", true);

        CurrentDialogueNode = DialogueData;
        SetNextDialogue();
    }

    #region Button Functions
    public void ButtonFunctionExample()
    {
        print("Example called");
    }

    public void Test()
    {
        print("Hello");
    }
    #endregion
}
