using UnityEngine;
using UnityEngine.InputSystem;

public class FootstepsManager: MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] footstepClips;
    [Range(0.1f, 1f)] public float footstepVolume = 0.5f;
    [Range(0f, 0.5f)] public float pitchVariation = 0.2f;

    [Header("Step Timing")]
    [SerializeField] private float stepInterval = 0.4f;

    private PlayerControls controls;
    private Vector2 moveInput;
    private float stepTimer;
    private bool isMoving;
    private bool isPlayingStep;

    private void Awake()
    {
        controls = new PlayerControls();

        // Subscribe to movement input
        controls.PlayerMovement.Movement.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.PlayerMovement.Movement.canceled += ctx => moveInput = Vector2.zero;
    }

    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();

    private void Update()
    {
        isMoving = moveInput.magnitude > 0.1f;

        if (isMoving && !isPlayingStep)
        {
            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0f)
            {
                StartCoroutine(PlayFootstepOnce());
                stepTimer = stepInterval;
            }
        }
        else if (!isMoving)
        {
            stepTimer = 0f;
        }
    }

    private System.Collections.IEnumerator PlayFootstepOnce()
    {
        if (isPlayingStep || footstepClips == null || footstepClips.Length == 0 || audioSource == null)
            yield break;

        isPlayingStep = true;

        AudioClip clip = footstepClips[Random.Range(0, footstepClips.Length)];
        audioSource.pitch = 1f + Random.Range(-pitchVariation, pitchVariation);

        // Play using PlayOneShot so it doesn’t cut off previous steps (if desired)
        audioSource.PlayOneShot(clip, footstepVolume);

        // Wait until the clip is mostly finished before allowing another
        yield return new WaitForSeconds(stepInterval * 0.9f);

        isPlayingStep = false;
    }
}
