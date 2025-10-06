using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHandler : MonoBehaviour
{
    public static PlayerHandler Instance { get; private set; }

    [Header("Player Data")]
    [Tooltip("Where the raycast will start for interacting"), SerializeField] private Vector3 InteractionOffset;
    [SerializeField] private float InteractionDistance = 5f;

    [Tooltip("Shows Raycasts and other stuff"), SerializeField] private bool DebugMode = false;
    private bool PlayerInitialized = false;

    private GameObject CurrentDetectedObject;
    private IInteractable CurrentDetectedInteractable;

    [SerializeField] private LayerMask InteractionLayerMask;
    [SerializeField] private LayerMask PlayerMask;
    private int InteractionLayerID;

    public string InteractionKeybind = "";
    private PlayerControls Controls;

    private bool InteractionListening = true;

    [Space, Header("Core Components")]
    public Camera BasePlayerCamera;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDisable()
    {
        Controls.PlayerActions.Interact.started -= Interact;
    }

    private void Start()
    {
        Controls = PlayerInputManager.playerInputManager.playerControls;
        InteractionKeybind = Controls.PlayerActions.Interact.bindings[0].ToDisplayString();
        Controls.PlayerActions.Interact.started += Interact;

        InteractionLayerID = (int)Mathf.Log(InteractionLayerMask.value, 2);
        RodManager.Instance.transform.gameObject.SetActive(false);
        PlayerInitialized = true;
    }

    private void HideInteraction()
    {
        CurrentDetectedObject = null;
        CurrentDetectedInteractable = null;
        UIManager.Instance.HideInteractionPrompt();
        return;
    }

    private void Interact(InputAction.CallbackContext ctx)
    {
        if (CurrentDetectedInteractable == null) return;

        CurrentDetectedInteractable.OnInteracted(this);
    }

    public void SetInteractionListening(bool IsListening)
    {
        InteractionListening = IsListening;

        if (!InteractionListening)
        {
            if (CurrentDetectedObject != null)
            {
                HideInteraction();
            }
        }
    }

    private void Update()
    {

        if (!PlayerInitialized || !InteractionListening) return;

        RaycastHit DetectedObject;
        Vector3 RaycastOrigin = BasePlayerCamera.transform.position + InteractionOffset;
        Vector3 RaycastDirection = BasePlayerCamera.transform.forward;

        if (Physics.Raycast(RaycastOrigin, RaycastDirection, out DetectedObject, InteractionDistance, ~PlayerMask))
        {
            GameObject DetectedGameObject = DetectedObject.transform.gameObject;

            print(DetectedGameObject.name);

            if (DetectedGameObject.layer != InteractionLayerID && CurrentDetectedObject != null)
            {
                HideInteraction();
                RodManager.Instance.CurrentSoulInteract?.EnableInteractionUI();
            }

            if (DetectedGameObject != CurrentDetectedObject && DetectedGameObject.layer == InteractionLayerID && !UIManager.Instance.DialogueIsOpen)
            {
                if (DetectedGameObject.TryGetComponent<IInteractable>(out IInteractable IInteractableScript))
                {
                    CurrentDetectedInteractable = IInteractableScript;
                    CurrentDetectedObject = DetectedGameObject;

                    RodManager.Instance.CurrentSoulInteract?.DisableInteractionUI();
                    UIManager.Instance.ShowInteractionPrompt(CurrentDetectedInteractable.GetInteractionPrompt(this));
                }
            }
        }
        else
        {
            if (CurrentDetectedObject != null)
            {
                RodManager.Instance.CurrentSoulInteract?.EnableInteractionUI();
                HideInteraction();
            }
        }

        if (DebugMode)
        {
            Debug.DrawRay(RaycastOrigin, RaycastDirection * InteractionDistance, Color.red);
        }
    }
}
