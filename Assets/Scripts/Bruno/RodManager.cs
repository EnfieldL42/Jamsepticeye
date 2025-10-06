using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class RodManager : MonoBehaviour
{
    public static RodManager Instance { get; private set; }
    [Header("References")]
    public Transform rodTip;
    public Rigidbody BaitPosition;
    public LayerMask waterLayer;
    public ConfigurableJoint BaitJoint;

    public ConstantForce BaitGravity;
    public Rigidbody BaitVisuals;
    public Transform CoinVisuals;

    public Rigidbody RodTipRigidbody;
    public Vector3 DefaultGravity = new Vector3(0, -300, 0);
    public Vector3 FishingGravity = new Vector3(0, -20, 0);
    public Transform BaitReference;
    public Animator FishingRodAnimator;

    [Header("Throw Settings")]
    public float throwHeight = 2f;       // vertical boost
    public float throwSpeed = 5f;        // initial forward speed
    public float bobAmplitude = 0.2f;    // max up/down offset while bobbing
    public float bobFrequency = 1f;      // how fast it bobs

    public bool isThrowing = false;
    public bool onWater = false;
    public bool canCast = true;

    private Vector3 velocity;             // current velocity of the bait
    private Vector3 baitOffset;
    private float bobTimer = 0f;          // timer for bobbing
    private Vector3 PreviousBaitPosition;
    int WaterLayerID;
    int ItemLayerID;
    int DefaultLayerID;

    private PlayerControls playerControls;
    [SerializeField] BaitManager fishingTimer;
    private bool BringingBackBait;
    public SoulInteract CurrentSoulInteract;

    [SerializeField] AudioSource fishingRodAudio;
    [SerializeField] AudioSource baitAudio;

    [SerializeField] AudioClip castSound;
    [SerializeField] AudioClip reelSound;
    [SerializeField] AudioClip fishCaughtSound;
    [SerializeField] AudioClip fishNotCaughtSound;
    [SerializeField] AudioClip baitLandedSound;


    private void Awake()
    {
        playerControls = new PlayerControls();

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        WaterLayerID = LayerMask.NameToLayer("Water");
        ItemLayerID = LayerMask.NameToLayer("Item");
        DefaultLayerID = LayerMask.NameToLayer("Default");
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
        if (BaitPosition != null && rodTip != null)
            baitOffset = BaitPosition.position - rodTip.position;
    }

    private void LateUpdate()
    {
        if (GameManager.Instance.playerHasSoul)
        {
            //Debug.Log("Player has soul");
            canCast = false;
        }
        else if (onWater)
        {
            canCast = false;
        }
        else if (isThrowing)
        {
            canCast = false;
        }
        else if (UIManager.Instance.DialogueIsOpen)
        {
            Debug.Log("dialogue is open");
            canCast = false;
        }
        else if (GameManager.Instance.SoulsDamned + GameManager.Instance.SoulsFreed >= 20)
        {
            Debug.Log("Player Has caught 20 souls");
            canCast = false;
        }
        else
        {
            canCast = true;
        }


        //if (!GameManager.Instance.playerHasSoul && !UIManager.Instance.DialogueIsOpen)
        //{
        //    Debug.Log("Player can cast");
        //    canCast = true;
        //}


        BaitVisuals.MovePosition(BaitPosition.position);
        BaitVisuals.MoveRotation(BaitPosition.rotation);

        /*
        // Keep bait at rod tip if not throwing or on water
        if (!isThrowing && !onWater)
        {
            bait.MovePosition(rodTip.position + baitOffset);
        }*/

        // Bobbing effect while on water
        if (onWater)
        {
            bobTimer += Time.deltaTime * bobFrequency;
            Vector3 bobPos = BaitPosition.position;
            bobPos.y += Mathf.Sin(bobTimer * Mathf.PI * 2f) * bobAmplitude;
            BaitPosition.MovePosition(bobPos);
        }
    }

    private void OnCastPerformed(InputAction.CallbackContext context)
    {
        if (canCast)
        {
            RaycastHit FishingCheck;
            Vector3 RaycastOrigin = PlayerHandler.Instance.BasePlayerCamera.transform.position;
            Vector3 RaycastDirection = PlayerHandler.Instance.BasePlayerCamera.transform.forward;

            if (Physics.Raycast(RaycastOrigin, RaycastDirection, out FishingCheck, 500f))
            {

                if (FishingCheck.transform.gameObject.layer != WaterLayerID) return;
            }

            canCast = false;
            if (isThrowing)
            {
                Debug.Log("Cannot cast while bait is in the air.");
                return;
            }
            else
            {
                StartThrowBait();
            }
        }

        //if (onWater)
        //{
        //    ReturnBait();
        //}
    }

    public void StartThrowBait()
    {
        if (isThrowing) return;

        //PlayerMovement.Instance.MovementDisabled = true;
        //CameraMove.Instance.PlayerControlsCamera = false;
        FishingRodAnimator.Play("Throw");
    }

    public void ThrowBait()
    {
        RaycastHit FishingCheck;
        Vector3 RaycastOrigin = PlayerHandler.Instance.BasePlayerCamera.transform.position;
        Vector3 RaycastDirection = PlayerHandler.Instance.BasePlayerCamera.transform.forward;

        if (Physics.Raycast(RaycastOrigin, RaycastDirection, out FishingCheck, 500f))
        {

            if (FishingCheck.transform.gameObject.layer != WaterLayerID)
            {
                FishingRodAnimator.Play("Rod Return");
                return;
            }
        }

        BaitVisuals.gameObject.layer = DefaultLayerID;
        CoinVisuals.gameObject.layer = DefaultLayerID;
        BaitPosition.transform.SetParent(GameManager.Instance.World);

        BaitJoint.connectedBody = null;
        BaitJoint.xMotion = ConfigurableJointMotion.Free;
        BaitJoint.yMotion = ConfigurableJointMotion.Free;
        BaitJoint.zMotion = ConfigurableJointMotion.Free;
        BaitGravity.force = FishingGravity;

        Vector3 forward = RodTipRigidbody.transform.forward;
        Vector3 up = Vector3.up;
        Vector3 velocity = forward * throwSpeed + up * throwHeight;

        BaitPosition.isKinematic = false;
        BaitPosition.linearVelocity = Vector3.zero;
        BaitPosition.AddForce(velocity, ForceMode.VelocityChange);

        onWater = false;
        isThrowing = true;
        bobTimer = 0f;
    }

    public void ReturnBait()
    {
        BringingBackBait = false;
    }

    private Coroutine currentReturnCoroutine;

    public void StartReturningBait()
    {
        if (currentReturnCoroutine != null) StopCoroutine(currentReturnCoroutine);
        currentReturnCoroutine = StartCoroutine(SmoothReturnWhileAnimation());
    }

    private IEnumerator SmoothReturnWhileAnimation()
    {
        BaitPosition.isKinematic = true;
        while (BringingBackBait)
        {
            BaitPosition.position = Vector3.Lerp(BaitPosition.position, BaitReference.position, 2f * Time.deltaTime);
            BaitPosition.rotation = Quaternion.Slerp(BaitPosition.rotation, BaitReference.rotation, 2f * Time.deltaTime);
            yield return null;
        }

        float t = 0f;
        Vector3 startPos = BaitPosition.position;
        Quaternion startRot = BaitPosition.rotation;
        float duration = 0.3f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            BaitPosition.position = Vector3.Lerp(startPos, BaitReference.position, t);
            BaitPosition.rotation = Quaternion.Slerp(startRot, BaitReference.rotation, t);
            yield return null;
        }

        BaitPosition.position = BaitReference.position;
        BaitPosition.rotation = BaitReference.rotation;

        BaitJoint.connectedBody = RodTipRigidbody;
        BaitJoint.xMotion = ConfigurableJointMotion.Limited;
        BaitJoint.yMotion = ConfigurableJointMotion.Limited;
        BaitJoint.zMotion = ConfigurableJointMotion.Limited;

        BaitPosition.isKinematic = false;

        // Reset flags
        onWater = false;
        bobTimer = 0f;
        //PlayerMovement.Instance.MovementDisabled = false;
        //CameraMove.Instance.PlayerControlsCamera = true;

        currentReturnCoroutine = null;
        CurrentSoulInteract?.EnableInteractionUI();
        BaitPosition.transform.SetParent(transform);

        BaitVisuals.gameObject.layer = ItemLayerID;
        CoinVisuals.gameObject.layer = ItemLayerID;
    }


    /*
    private void Update()
    {

        if (!isThrowing) return;

        print("Hello");
        // Apply gravity
        //velocity += Physics.gravity * Time.deltaTime;

        // Move bait
        //bait.position += velocity * Time.deltaTime;

        // Check if hit water
        RaycastHit hit;
        if (Physics.Raycast(BaitPosition.position + Vector3.up * 0.1f, Vector3.down, out hit, 0.2f, waterLayer))
        {
            isThrowing = false;
            onWater = true;
            // Snap to water surface
            //bait.MovePosition(hit.point);
            bobTimer = 0f;
            Debug.Log("Bait hit water!");
        }
    }*/

    public void OnBaitTouchedWater()
    {
        isThrowing = false;
        onWater = true;
        // Snap to water surface
        BaitPosition.isKinematic = true;
        bobTimer = 0f;
        BaitLandedSound();
        Debug.Log("Bait hit water!");
    }

    public void PlayCastSound()
    {
        fishingRodAudio.volume = 1f;
        fishingRodAudio.PlayOneShot(castSound);
    }

    public void PlayReelingSound()
    {
        fishingRodAudio.volume = 0.5f;
        fishingRodAudio.clip = reelSound;
        fishingRodAudio.loop = true;
        fishingRodAudio.Play();
    }

    public void StopReelingSound()
    {
        fishingRodAudio.volume = 1f;
        fishingRodAudio.loop = false;
        fishingRodAudio.Stop();
        fishingRodAudio.clip = null;
    }

    public void FishCaughtSound()
    {
        fishingRodAudio.volume = 0.5f;
        fishingRodAudio.PlayOneShot(fishCaughtSound);
    }

    public void FishNotCaughtSound()
    {

    }

    public void BaitLandedSound()
    {
        baitAudio.PlayOneShot(baitLandedSound);
    }

}


