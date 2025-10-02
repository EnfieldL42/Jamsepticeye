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
    public float driftSpeed = 200f;      // How fast bar drifts left
    public float pushSpeed = 300f;       // How fast bar moves right when held
    public float fishSpeed = 200f;       // How fast fish moves
    public float fishChangeInterval = 1.5f; // How often fish changes direction
    public float successRate = 25f;      // % per second gained when overlapping
    public float failRate = 15f;         // % per second lost when not overlapping

    private RectTransform panelBounds;
    private float barVelocity;
    private float fishDirection;
    private float fishTimer;

    private PlayerControls playerControls;

    private void Awake()
    {
        playerControls = new PlayerControls();
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
        progressBar.value = 0f;
        fishTimer = fishChangeInterval;
        fishDirection = Random.value > 0.5f ? 1f : -1f;
    }

    private void Update()
    {
        HandlePlayerBar();
        HandleFishMovement();
        HandleProgress();
    }

    void HandlePlayerBar()
    {
        // Read input from the new input system (0 = not held, 1 = held)
        float input = playerControls.PlayerActions.Reel.ReadValue<float>();

        if (input > 0.5f)
        {
            barVelocity = pushSpeed * Time.deltaTime; // Move right
        }
        else
        {
            barVelocity = -driftSpeed * Time.deltaTime; // Drift left
        }

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
            Debug.Log("Fish Caught!");
        else if (progressBar.value <= 0f)
            Debug.Log("Fish Escaped!");
    }
}
