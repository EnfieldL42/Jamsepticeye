using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private PlayerControls Controls;
    [SerializeField] private float PauseSpeed = 0.15f;

    private float TimeElapsed = 0f;
    private float TargetSpeed = 0f;

    public bool UpdatingGameTime = false;
    public bool GamePaused = false;
    public bool CanPauseGame = true;

    private float StartSpeed = 0f;
    private bool Initialized = false;
    public Transform World;
    public Transform Water;
    private bool CursorLockedExternally = false;

    [Header("Souls Prefabs")]
    public List<GameObject> SoulPrefabs = new List<GameObject>();
    private GameObject lastSpawnedSoul;
    public int soulIndex = 0;
    public bool playerHasSoul = false;

    public int SoulsDamned = 0;
    public int SoulsFreed = 0;

    [Header("Countdown Settings")]
    public float countdownTime = 10f;

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

        AudioListener.volume = PlayerPrefs.GetFloat("MasterVolme", 1f);
    }

    public void SetCurstorState(CursorLockMode LockMode = default, bool Visible = default, bool ForceOverride = default)
    {
        if (CursorLockedExternally && !ForceOverride)
        {
            return;
        }

        Cursor.lockState = LockMode;
        Cursor.visible = Visible;

        print("Hid everything");
    }

    public void LockCursorExternal(bool locked)
    {
        CursorLockedExternally = locked;
    }

    private void Start()
    {
        Controls = PlayerInputManager.playerInputManager.playerControls;
        Controls.Enable();

        Controls.PlayerActions.GamePause.started += ToggleEscapeMenu;
        Initialized = true;

        if (LoadingScreenManager.Instance != null && LoadingScreenManager.Instance.InLoadingScreen)
        {
            LoadingScreenManager.Instance.StopLoading();
        }

        StartCoroutine(ReapersCallCountdown());
    }

    private void OnDisable()
    {
        Controls.PlayerActions.GamePause.started -= ToggleEscapeMenu;
        Controls.Disable();
    }

    public void ToggleEscapeMenu(InputAction.CallbackContext ctx = default)
    {
        if (!CanPauseGame) return;
        CanPauseGame = false;

        GamePaused = !GamePaused;
        CameraMove.Instance.PlayerControlsCamera = !GamePaused;

        UpdatingGameTime = true;
        StartCoroutine(TogglePauseMenu(GamePaused));

        UIManager.Instance.PauseMenuCanvas.interactable = false;
        UIManager.Instance.PauseMenuCanvas.blocksRaycasts = false;

        if (GamePaused)
        {
            SetCurstorState(CursorLockMode.None, true);
        }
        else
        {
            SetCurstorState(CursorLockMode.Locked, false);
        }

        TimeElapsed = 0f;
        StartSpeed = Time.timeScale;
        TargetSpeed = GamePaused ? 0f : 1f;
    }

    private IEnumerator TogglePauseMenu(bool showing)
    {
        float t = 0f;
        float duration = 0.25f;
        float start = showing ? 0f : 1f;
        float end = showing ? 1f : 0f;

        if (showing)
        {
            UIManager.Instance.PauseMenuCanvas.gameObject.SetActive(true);
        }

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / duration;
            UIManager.Instance.PauseMenuCanvas.alpha = Mathf.Lerp(start, end, t);
            yield return null;
        }

        UIManager.Instance.PauseMenuCanvas.interactable = showing;
        UIManager.Instance.PauseMenuCanvas.blocksRaycasts = showing;

        UIManager.Instance.PauseMenuCanvas.alpha = end;

        if (!showing)
        {
            UIManager.Instance.PauseMenuCanvas.gameObject.SetActive(false);
        }

        CanPauseGame = true;
    }


    public void StopUpdatingGameTime()
    {
        UpdatingGameTime = false;
    }

    private void Update()
    {
        if (!Initialized) return;
        //UIManager.Instance.FPSText.SetText("FPS {0:0}", 1 / Time.unscaledDeltaTime);

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


    //SOUL MECHANICS

    public GameObject SpawnNextSoulPrefab(Transform parent)
    {
        if (SoulPrefabs.Count == 0) return null;

        GameObject prefabToSpawn = SoulPrefabs[soulIndex];

        lastSpawnedSoul = Instantiate(prefabToSpawn, parent.position, parent.rotation, parent);

        soulIndex = (soulIndex + 1) % SoulPrefabs.Count;

        return lastSpawnedSoul;
    }

    public void DestroyLastSoul()
    {
        if (lastSpawnedSoul != null)
        {
            Destroy(lastSpawnedSoul);
            lastSpawnedSoul = null;
        }
    }

    public void CondemnSoul()
    {
        RodManager.Instance.CurrentSoulInteract = null;
        playerHasSoul = false;
        SoulsDamned += 1;
        DestroyLastSoul();
    }

    public void FreeSoul()
    {
        RodManager.Instance.CurrentSoulInteract = null;
        playerHasSoul = false;
        SoulsFreed += 1;
        DestroyLastSoul();
    }

    public void OpenFishindRodCollider()
    {
        
    }

    public void InitializeDeathDialogue()
    {

    }

    private IEnumerator ReapersCallCountdown()
    {
        float timer = countdownTime;

        while (timer > 0)
        {
            yield return new WaitForSeconds(1f);
            timer -= 1f;
        }

        ReaperManager.Instance.ReaperCallEvent();

        yield return null;
    }

    public void CheckSoulCount()
    {
        if(SoulsDamned + SoulsFreed >= 20)
        {
            if (SoulsFreed <= 1)
            {
                ReaperManager.Instance.ChooseEnding(1);
            }
            else
            {
                ReaperManager.Instance.ChooseEnding(2);
            }
        }
    }

    //ENDING SEQUENCE

    public void EndingSequence()
    {
        Debug.Log("game ended");
        UIManager.Instance.UIEndingSequence();
    }


}
