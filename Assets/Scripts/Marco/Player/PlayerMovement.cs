using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance;
    [SerializeField] private float MoveSpeed;

    [SerializeField] private float GroundDrag;
    [SerializeField] private bool Grounded;

    [SerializeField] private Transform Floor;
    [SerializeField] private LayerMask PlayerMask;

    [SerializeField] private Transform PlayerOrientation;
    private Vector3 MoveDirection;
    [SerializeField] private Rigidbody Rigidbody;

    [SerializeField] private Vector3 GroundCheckOffset;
    [SerializeField] private float GroundCheckDistance;

    private PlayerControls Controls;
    [SerializeField] private ConstantForce PlayerGravity;
    private RaycastHit[] GroundHits = new RaycastHit[1];

    public bool MovementDisabled = false;
    [SerializeField] private Vector3 GroundedForce = new Vector3(0, 0, 0);

    [SerializeField] private Vector3 NonGroundedForce = new Vector3(0, -100, 0);
    private bool MovementInitialized = false;
    private int PlayerMaskID;

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
        Rigidbody.freezeRotation = true;
        PlayerMaskID = PlayerMask.value;
        MovementInitialized = true;
    }

    private void FixedUpdate()
    {
        if (MovementDisabled || !MovementInitialized) return;

        GroundCheck();
        MovePlayer();
        PlayerGravity.force = !Grounded ? NonGroundedForce : GroundedForce;
    }

    private void GroundCheck()
    {
        Vector3 downDirection = -Floor.up;
        Vector3 origin = Floor.position + GroundCheckOffset;

        Grounded = Physics.CheckSphere(
            Floor.position + GroundCheckOffset,
            GroundCheckDistance,
            PlayerMaskID,
            QueryTriggerInteraction.Ignore
        );

        Debug.DrawRay(origin, downDirection * GroundCheckDistance,Grounded ? Color.green : Color.red);
    }

    private void MovePlayer()
    {
        Vector3 InputDirection = new Vector3(PlayerInputManager.playerInputManager.movementInput.x, 0, PlayerInputManager.playerInputManager.movementInput.y);
        Vector3 FlatForward = new Vector3(PlayerOrientation.forward.x, 0, PlayerOrientation.forward.z).normalized;
        Vector3 FlatRight = new Vector3(PlayerOrientation.right.x, 0, PlayerOrientation.right.z).normalized;

        MoveDirection = FlatForward * InputDirection.z + FlatRight * InputDirection.x;
        Vector3 targetVelocity = MoveDirection.normalized * MoveSpeed;

        targetVelocity.y = Rigidbody.linearVelocity.y;
        Rigidbody.linearVelocity = targetVelocity;
    }
}