using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMove : MonoBehaviour
{
    public static CameraMove Instance { get; private set; }

    [Header("References")]
    [SerializeField] private Rigidbody PlayerRigidbody;
    public CinemachineCamera CinemachineCamera;

    [SerializeField] private Transform CameraHolder;
    [SerializeField] private Rigidbody CameraRigidbody;
    [SerializeField] private Transform Orientation;

    [Header("Settings")]
    public bool PlayerControlsCamera = true;

    private bool PreviousCameraState = true;
    [SerializeField] private bool CameraInverted = false;

    [SerializeField] private float MouseSensitivity = 2f;
    [SerializeField] private float MaxLookAngle = 90f;

    private float CameraYaw = 0f;
    public float CameraPitch = 0f;

    public PlayerControls Controls;
    private Vector2 CameraInputs;
    private bool CameraInitialized = false;

    private IEnumerator WaitForPlayerInputManager()
    {
        while (PlayerInputManager.playerInputManager == null || !PlayerInputManager.playerInputManager.ControlsEnabled)
        {
            yield return null;
        }

        Controls = PlayerInputManager.playerInputManager.playerControls;
        Controls.Enable();

        Controls.PlayerMovement.Look.started += OnCameraMoved;

        Controls.PlayerMovement.Look.canceled += OnCameraStopped;
        CameraInitialized = true;

        GameManager.Instance.SetCurstorState(CursorLockMode.Locked, false);
    }

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
    }

    private void Start()
    {
        StartCoroutine(WaitForPlayerInputManager());
    }

    private void OnDisable()
    {
        Controls.PlayerMovement.Look.started -= OnCameraMoved;
        Controls.PlayerMovement.Look.canceled -= OnCameraStopped;
    }

    private void OnCameraMoved(InputAction.CallbackContext ctx)
    {
        CameraInputs = ctx.ReadValue<Vector2>();
    }

    private void OnCameraStopped(InputAction.CallbackContext ctx)
    {
        CameraInputs = Vector2.zero;
    }

    private void ToggleCamera() {
        CinemachineCamera.enabled = PlayerControlsCamera;
    }

    private void Update()
    {
        if (!CameraInitialized) return;
        CameraRigidbody.MovePosition(Orientation.position);
    }

    private void LateUpdate()
    {
        if (!CameraInitialized) return;

        if (PreviousCameraState != PlayerControlsCamera) {
            PreviousCameraState = PlayerControlsCamera;
            ToggleCamera();
        }

        if (!PlayerControlsCamera) return;

        float MouseX = CameraInputs.x * MouseSensitivity;
        float MouseY = CameraInputs.y * MouseSensitivity;

        CameraYaw += CameraInverted ? -MouseX : MouseX;
        CameraPitch += CameraInverted ? MouseY : -MouseY;
        CameraPitch = Mathf.Clamp(CameraPitch, -MaxLookAngle, MaxLookAngle);

        PlayerRigidbody.MoveRotation(Quaternion.Euler(0f, CameraYaw, 0f));
        CameraRigidbody.MoveRotation(Quaternion.Euler(CameraPitch, CameraYaw, 0f));
    }
}
