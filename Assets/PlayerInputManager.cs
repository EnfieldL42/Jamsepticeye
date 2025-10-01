using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager playerInputManager;

    private PlayerManager player;
    private PlayerControls playerControls;

    [Header("Movement Inputs")]
    [SerializeField] Vector2 movementInput;
    public float verticalInput;
    public float horizontalInput;
    public float moveAmount;

    private void Awake()
    {
        if (playerInputManager == null)
        {
            playerInputManager = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        player = GetComponent<PlayerManager>();
        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {

        if (playerControls == null)
        {
            playerControls = new PlayerControls();

            //movement
            playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>(); //stores vector 2 of input in i then reads it and adds it to the vector 2 movement
            playerControls.PlayerMovement.Movement.canceled += i => movementInput = Vector2.zero;

            playerControls.Enable();
        }
    }
    private void OnDestroy()
    {
        
    }

    private void Update()
    {
        HandleAllInputs();
    }

    private void HandleAllInputs()
    {
        HandleMovementInput();
    }
    private void HandleMovementInput()
    {
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;


        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput));

        if (moveAmount <= 0.5 && moveAmount > 0)
        {
            moveAmount = 0.5f;
        }
        else if (moveAmount > 0.5 && moveAmount <= 1)
        {
            moveAmount = 1;
        }

        if (player == null)
        {
            return;
        }

        if (moveAmount != 0)
        {
            player.isMoving = true;
        }
        else
        {
            player.isMoving = false;

        }

        if (!player.canRun)
        {
            if (moveAmount > 0.5)
            {
                moveAmount = 0.5f;
            }
            if (verticalInput > 0.5f)
            {
                verticalInput = 0.5f;
            }
            if (verticalInput < -0.5f)
            {
                verticalInput = -0.5f;
            }
            if (horizontalInput < -0.5f)
            {
                horizontalInput = -0.5f;
            }
        }
    }


}
