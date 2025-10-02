using UnityEngine;
using UnityEngine.InputSystem;

public class BaitThrowInput : MonoBehaviour
{
    [Header("References")]
    public Transform rodTip;
    public Transform bait;
    public LayerMask waterLayer;

    [Header("Throw Settings")]
    public float throwHeight = 2f;       // vertical boost
    public float throwSpeed = 5f;        // initial forward speed
    public float bobAmplitude = 0.2f;    // max up/down offset while bobbing
    public float bobFrequency = 1f;      // how fast it bobs

    private bool isThrowing = false;
    public bool onWater = false;

    private Vector3 velocity;             // current velocity of the bait
    private Vector3 baitOffset;
    private float bobTimer = 0f;          // timer for bobbing

    private PlayerControls playerControls;
    [SerializeField] BaitFishingTimer fishingTimer;

    private void Awake()
    {
        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        playerControls.PlayerActions.Cast.performed += OnCastPerformed;
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.PlayerActions.Cast.performed -= OnCastPerformed;
        playerControls.Disable();
    }

    private void Start()
    {
        if (bait != null && rodTip != null)
            baitOffset = bait.position - rodTip.position;
    }

    private void LateUpdate()
    {
        // Keep bait at rod tip if not throwing or on water
        if (!isThrowing && !onWater)
        {
            bait.position = rodTip.position + baitOffset;
        }

        // Bobbing effect while on water
        if (onWater)
        {
            bobTimer += Time.deltaTime * bobFrequency;
            Vector3 bobPos = bait.position;
            bobPos.y += Mathf.Sin(bobTimer * Mathf.PI * 2f) * bobAmplitude;
            bait.position = bobPos;
        }
    }

    private void OnCastPerformed(InputAction.CallbackContext context)
    {
        if (isThrowing)
        {
            Debug.Log("Cannot cast while bait is in the air.");
            return;
        }

        if (onWater)
        {
            ReturnBait();
        }
        else
        {
            ThrowBait();
        }
    }

    private void ThrowBait()
    {
        // Detach from player so it moves freely
        bait.parent = null;

        // Compute initial velocity
        Vector3 forward = rodTip.forward;
        Vector3 up = Vector3.up;
        velocity = forward * throwSpeed + up * throwHeight;

        isThrowing = true;
        onWater = false;
        bobTimer = 0f;
    }

    public void ReturnBait()
    {
        // Return bait to rod tip
        fishingTimer.timerStarted = false; // reset bite timer
        bait.position = rodTip.position + baitOffset;
        bait.parent = rodTip; // optional
        onWater = false;
        bobTimer = 0f; // reset bob timer
        Debug.Log("Bait returned to rod.");
    }

    private void Update()
    {
        if (!isThrowing) return;

        // Apply gravity
        velocity += Physics.gravity * Time.deltaTime;

        // Move bait
        bait.position += velocity * Time.deltaTime;

        // Check if hit water
        RaycastHit hit;
        if (Physics.Raycast(bait.position + Vector3.up * 0.1f, Vector3.down, out hit, 0.2f, waterLayer))
        {
            isThrowing = false;
            onWater = true;
            // Snap to water surface
            bait.position = hit.point;
            bobTimer = 0f;
            Debug.Log("Bait hit water!");
        }
    }
}
