using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class FishingMechanics : MonoBehaviour
{
    [Header("UI References")]
    public RectTransform catchBar;
    public RectTransform fishMarker;
    public Slider progressBar;

    [Header("Settings")]
    public float driftSpeed = 200f;
    public float pushSpeed = 300f;
    public float fishSpeed = 200f;
    public float fishChangeInterval = 1.5f;
    public float successRate = 25f;
    public float failRate = 15f;
    [SerializeField] float initialTimer = 3f;

    private RectTransform panelBounds;
    private float barVelocity;
    private float fishDirection;
    private float fishTimer;
    

    private PlayerControls playerControls;
    [SerializeField] RodManager rodManager;
    [SerializeField] BaitManager baitManager;

    // Store initial positions
    private Vector2 catchBarInitialPos;
    private Vector2 fishMarkerInitialPos;
    private float initialTimerStart;

    private void Awake()
    {
        playerControls = new PlayerControls();
        catchBarInitialPos = catchBar.anchoredPosition;
        fishMarkerInitialPos = fishMarker.anchoredPosition;
        initialTimerStart = initialTimer;
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    private void Start()
    {
        panelBounds = GetComponent<RectTransform>();
        fishTimer = fishChangeInterval;
        fishDirection = Random.value > 0.5f ? 1f : -1f;
    }

    private void Update()
    {
        HandlePlayerBar();
        HandleFishMovement();
        HandleProgress();
        InitialTimer();

        if(gameObject.activeSelf == true)
        {
            rodManager.canCast = false;
        }
    }

    void InitialTimer()
    {
        if (initialTimerStart > 0)
            initialTimerStart -= Time.deltaTime;
    }

    void HandlePlayerBar()
    {
        float input = playerControls.PlayerActions.Reel.ReadValue<float>();

        if (input > 0.5f)
            barVelocity = pushSpeed * Time.deltaTime;
        else
            barVelocity = -driftSpeed * Time.deltaTime;

        Vector2 pos = catchBar.anchoredPosition;
        pos.x += barVelocity;

        float halfWidth = panelBounds.rect.width / 2;
        pos.x = Mathf.Clamp(pos.x, -halfWidth, halfWidth);

        catchBar.anchoredPosition = pos;
    }

    void HandleFishMovement()
    {
        fishTimer -= Time.deltaTime;
        if (fishTimer <= 0)
        {
            fishTimer = fishChangeInterval;
            fishDirection = Random.value > 0.5f ? 1f : -1f;
        }

        Vector2 pos = fishMarker.anchoredPosition;
        pos.x += fishDirection * fishSpeed * Time.deltaTime;

        float halfWidth = panelBounds.rect.width / 2;
        pos.x = Mathf.Clamp(pos.x, -halfWidth, halfWidth);

        fishMarker.anchoredPosition = pos;
    }

    void HandleProgress()
    {
        bool isOverlapping = Mathf.Abs(catchBar.anchoredPosition.x - fishMarker.anchoredPosition.x) < 50f;

        if (isOverlapping)
            progressBar.value += successRate * Time.deltaTime / 100f;
        else
            progressBar.value -= failRate * Time.deltaTime / 100f;

        progressBar.value = Mathf.Clamp01(progressBar.value);

        if (progressBar.value >= 1f)
        {
            Debug.Log("Fish Caught");
            baitManager.InstantiateSoulAtBait();
            GameManager.Instance.playerHasSoul = true;
            rodManager.ReturnBait();
            ResetFishing();

            gameObject.SetActive(false);
        }
        else if (progressBar.value <= 0f && initialTimerStart <= 0f)
        {
            Debug.Log("Fish Escaped");
            rodManager.ReturnBait();
            ResetFishing();

            gameObject.SetActive(false);
        }
    }

    private void ResetFishing()
    {
        // Reset bar and fish positions
        baitManager.ResetBite();
        catchBar.anchoredPosition = catchBarInitialPos;
        fishMarker.anchoredPosition = fishMarkerInitialPos;

        // Reset progress and timer
        progressBar.value = 0f;
        initialTimerStart = initialTimer;
        fishTimer = fishChangeInterval;

        // Reset fish direction
        fishDirection = Random.value > 0.5f ? 1f : -1f;
    }
}
