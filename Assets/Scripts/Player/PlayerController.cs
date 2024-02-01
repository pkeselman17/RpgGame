using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public CharacterController controller;
    public LayerMask whatIsGround;


    private const float Gravity = -9.81f;

    [SerializeField]
    private float speed = 1.0f; 

    [SerializeField]
    private float runModifier = 6.0f; 
    [SerializeField]
    private float jumpHeight = 1f;
    [SerializeField]
    private float rotationSpeed = 4f;
    [SerializeField]
    private float jumpCooldown;

    [SerializeField]
    private Transform groundCheck;
    private bool readyToJump;
    private Vector3 playerVelocity;

    private bool grounded;

    private Vector3 moveDirection;
    private Animator animator;
    private Transform cameraMainTransform;
    private PlayerControls playerControls;
    private GameObject freeLook;
    private GameObject vCam;


    private void Awake() {
        playerControls = new PlayerControls();
    }

    private void OnEnable() {
        playerControls.Enable();
    }

    private void OnDisable() {
        playerControls.Disable();
    }
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        cameraMainTransform = Camera.main.transform;
        vCam = GameObject.FindWithTag("vCam");
        freeLook = GameObject.FindWithTag("freeLook");
        readyToJump = true;
    }

    // Update is called once per frame
    private void Update()
    {
        // grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight + 0.1f, whatIsGround);
        grounded = Physics.CheckSphere(groundCheck.position, 0.2f, whatIsGround);

        MovePlayer();
        Jump();
        Attack();
    }

    private void MovePlayer() 
    {
        Vector2 movement = playerControls.Keyboard.Movement.ReadValue<Vector2>();

        moveDirection = new Vector3(movement.x, 0f, movement.y);
        moveDirection = cameraMainTransform.forward * moveDirection.z + cameraMainTransform.right * moveDirection.x;
        moveDirection.y = 0f;

        if (movement != Vector2.zero) {

            RotatePlayer(movement);
            ControlMovement();
            
        } else {
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsRunning", false);
        }
    }

    private void ControlMovement()
    {
        if(playerControls.Keyboard.Run.IsPressed()) {
            controller.Move(runModifier * Time.deltaTime * moveDirection);
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsRunning", true);

        } else {
            controller.Move(speed * Time.deltaTime * moveDirection);
            animator.SetBool("IsWalking", true);
            animator.SetBool("IsRunning", false);
        }    
    }

    private void RotatePlayer(Vector2 movement) {
        float targetAngle = Mathf.Atan2(movement.x, movement.y) * Mathf.Rad2Deg + cameraMainTransform.eulerAngles.y;
        Quaternion rotation = Quaternion.Euler(0f, targetAngle, 0f);

        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);
    }
    private void Jump() {
        if (playerControls.Keyboard.Jump.triggered && readyToJump && grounded) {
            readyToJump = false;

            playerVelocity.y = Mathf.Sqrt(jumpHeight * 2 * -Gravity);
        } else {
            playerVelocity.y += Gravity * Time.deltaTime;
        }

        controller.Move(playerVelocity * Time.deltaTime);
        Invoke(nameof(ResetJump), jumpCooldown);
    }

    private void Attack()
    {
        if (playerControls.Mouse.Aim.IsPressed()) {
            freeLook.SetActive(false);
            vCam.SetActive(true);
        } else {
            freeLook.SetActive(true);
            vCam.SetActive(false);        }
    }

    private void ResetJump(){
        readyToJump = true;
    }
}
