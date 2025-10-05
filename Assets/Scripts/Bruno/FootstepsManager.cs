using UnityEngine;
using UnityEngine.InputSystem;

public class FootstepsManager : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] footstepClips;
    [Range(0.1f, 1f)] public float footstepVolume = 0.5f;
    [Range(0f, 0.5f)] public float pitchVariation = 0.2f;

    [Header("Step Timing")]
    [SerializeField] private float stepInterval = 0.4f;            // time between steps
    [SerializeField] private float movementGraceTime = 0.12f;      // tolerate short input drops
    [SerializeField] private float inputDeadzone = 0.1f;           // how strong input must be to count as "moving"

    private PlayerControls controls;
    private Vector2 moveInput;
    private float lastNonZeroInputTime = -Mathf.Infinity;
    private float lastStepTime = -Mathf.Infinity;

    private void Awake()
    {
        controls = new PlayerControls();
    }

    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();

    private void Update()
    {
        // Read the value action directly each frame (stable for Value-type Vector2 actions)
        moveInput = controls.PlayerMovement.Movement.ReadValue<Vector2>();

        // Record the last time we had a non-zero input
        if (moveInput.magnitude > inputDeadzone)
            lastNonZeroInputTime = Time.time;

        // Consider the player "moving" if we had movement recently (grace time smooths tiny micro-stops)
        bool isMoving = (Time.time - lastNonZeroInputTime) < movementGraceTime;

        // Play a step only if moving and enough time has passed since the last step
        if (isMoving && (Time.time - lastStepTime >= stepInterval))
        {
            PlayFootstep();
            lastStepTime = Time.time;
        }
    }

    private void PlayFootstep()
    {
        if (audioSource == null || footstepClips == null || footstepClips.Length == 0)
            return;

        // Optional: only play if the AudioSource isn't already playing (ensures single-step at a time).
        // Remove this check if you want footsteps to overlap or if you're using different mixer routing.
        if (audioSource.isPlaying)
            return;

        AudioClip clip = footstepClips[Random.Range(0, footstepClips.Length)];
        audioSource.pitch = 1f + Random.Range(-pitchVariation, pitchVariation);
        audioSource.PlayOneShot(clip, footstepVolume);
    }
}
