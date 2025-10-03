using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private PlayerControls Controls;
    [SerializeField] private float PauseSpeed = 0.15f;

    private float TimeElapsed = 0f;
    private float TargetSpeed = 0f;

    public bool UpdatingGameTime = false;
    private bool GamePaused = false;

    private float StartSpeed = 0f;
    private bool Initialized = false;

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

    public void SetCurstorState(CursorLockMode LockMode = default, bool Visible = default)
    {
        Cursor.lockState = LockMode;
        Cursor.visible = Visible;
    }

    /*
    private IEnumerator WaitForControls()
    {
        while (PlayerInputManager.playerInputManager == null || !PlayerInputManager.playerInputManager.ControlsEnabled)
        {
            yield return null;
        }

        Controls = PlayerInputManager.playerInputManager.playerControls;
        Controls.Enable();

        Controls.PlayerActions.GamePause.started += ToggleEscapeMenu;
        Initialized = true;
    }*/

    private void Start()
    {
        //StartCoroutine(WaitForControls());
        Controls = PlayerInputManager.playerInputManager.playerControls;
        Controls.Enable();

        Controls.PlayerActions.GamePause.started += ToggleEscapeMenu;
        Initialized = true;
    }

    private void OnDisable()
    {
        Controls.PlayerActions.GamePause.started -= ToggleEscapeMenu;
        Controls.Disable();
    }

    private void ToggleEscapeMenu(InputAction.CallbackContext ctx)
    {
        GamePaused = !GamePaused;
        CameraMove.Instance.PlayerControlsCamera = !GamePaused;
        UpdatingGameTime = true;

        TimeElapsed = 0f;
        StartSpeed = Time.timeScale;
        TargetSpeed = GamePaused ? 0f : 1f;
    }

    public void StopUpdatingGameTime()
    {
        UpdatingGameTime = false;
    }

    private void Update()
    {
        if (!Initialized) return;
        UIManager.Instance.FPSText.SetText("FPS {0:0}", 1 / Time.unscaledDeltaTime);

        if (!UpdatingGameTime) return;

        if (TimeElapsed < PauseSpeed)
        {
            TimeElapsed += Time.unscaledDeltaTime;
            float Progress = Mathf.Clamp01(TimeElapsed / PauseSpeed);

            float Eased = Mathf.SmoothStep(0f, 1f, Progress);
            Time.timeScale = Mathf.Lerp(StartSpeed, TargetSpeed, Eased);
        }
        else
        {
            Time.timeScale = TargetSpeed;
            UpdatingGameTime = false;
        }
    }
}
