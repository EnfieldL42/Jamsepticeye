using System.Collections;
using UnityEngine;
using UnityEngine.UI;

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
    private int InteractionLayerID;

    public string InteractionKeybind = "";
    private PlayerControls Controls;

    private bool InteractionListening = true;

    [Space, Header("Core Components")]
    [SerializeField] private Camera BasePlayerCamera;

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

    private IEnumerator WaitForInputManager()
    {
        while (PlayerInputManager.playerInputManager == null && !PlayerInputManager.playerInputManager.ControlsEnabled)
        {
            yield return null;
        }

        Controls = PlayerInputManager.playerInputManager.playerControls;
        Controls.Enable();
        InteractionKeybind = Controls.PlayerActions.Interact.bindings[0].ToDisplayString();

        InteractionLayerID = (int)Mathf.Log(InteractionLayerMask.value, 2);
        PlayerInitialized = true;
    }

    private void Start()
    {
        StartCoroutine(WaitForInputManager());
    }

    private void HideInteraction()
    {
        CurrentDetectedObject = null;
        CurrentDetectedInteractable = null;
        UIManager.Instance.HideInteractionPrompt();
        return;
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

        if (Physics.Raycast(RaycastOrigin, RaycastDirection, out DetectedObject, InteractionDistance))
        {
            GameObject DetectedGameObject = DetectedObject.transform.gameObject;

            if (DetectedGameObject.layer != InteractionLayerID && CurrentDetectedObject != null)
            {
                HideInteraction();
            }

            if (DetectedGameObject != CurrentDetectedObject)
            {
                if (DetectedGameObject.TryGetComponent<IInteractable>(out IInteractable IInteractableScript))
                {
                    CurrentDetectedInteractable = IInteractableScript;
                    CurrentDetectedObject = DetectedGameObject;

                    UIManager.Instance.ShowInteractionPrompt(CurrentDetectedInteractable.GetInteractionPrompt(this));
                }
            }
        }
        else
        {
            if (CurrentDetectedObject != null)
            {
                HideInteraction();
            }
        }

        if (DebugMode)
        {
            Debug.DrawRay(RaycastOrigin, RaycastDirection * InteractionDistance, Color.red);
        }
    }
}
