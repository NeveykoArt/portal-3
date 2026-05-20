using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Скорости передвижения")]
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float runSpeed = 5f;
    [SerializeField] private float acceleration = 5f;
    
    [Header("Настройки прыжка")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] [Range(0.90f, 1f)] private float airResistance = 0.995f; // Сопротивление воздуха
    [SerializeField] private LayerMask groundLayer = 1;
    
    [Header("Настройки бега")]
    [SerializeField] private bool toggleRun = false; // false = зажимать, true = переключатель
    
    private Rigidbody rb;
    private Vector2 moveInput;
    private bool isRunning = false;
    private bool isGrounded = false;

    // Input System
    private InputSystem_Actions controls;
    
    void Awake()
    {
        controls = new InputSystem_Actions();
    }
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.freezeRotation = true;
            rb.constraints = RigidbodyConstraints.FreezeRotationX | 
                            RigidbodyConstraints.FreezeRotationZ;
        }
    }
    
    void OnEnable()
    {
        controls.Enable();
        
        // Подписываемся на ввод движения
        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;
        
        // Подписываемся на бег
        if (toggleRun)
        {
            // Режим переключателя
            controls.Player.Run.performed += ctx => isRunning = !isRunning;
        }
        else
        {
            // Режим зажимания
            controls.Player.Run.performed += ctx => isRunning = true;
            controls.Player.Run.canceled += ctx => isRunning = false;
        }
        
        // Подписываемся на прыжок
        controls.Player.Jump.performed += ctx => Jump();
    }

    void OnDisable()
    {
        controls.Disable();
    }
    
    void FixedUpdate()
    {
        Move();
    }
    
    private void Move()
    {
        if(isGrounded)
        {
            GroundMovement();
        } else
        {
            AirMovement();
        }
    }
    
    private void GroundMovement()
    {
        float currentSpeed = isRunning ? runSpeed : walkSpeed;
        
        Vector3 moveDirection = transform.right * moveInput.x + transform.forward * moveInput.y;
        
        Vector3 moveVelocity = moveDirection.normalized * currentSpeed;
        moveVelocity.y = rb.linearVelocity.y;
        
        rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, moveVelocity, acceleration * Time.fixedDeltaTime);
    }

    private void AirMovement()
    {
        Vector3 velocity = rb.linearVelocity;
        
        velocity.x *= airResistance;
        velocity.z *= airResistance;
        
        rb.linearVelocity = velocity;
    }

    private void Jump()
    {
        if (isGrounded)
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
    
    void OnCollisionStay(Collision collision)
    {
        isGrounded = true;
    }

    void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }

    public bool IsRunning()
    {
        return isRunning;
    }
    
    public float GetCurrentSpeed()
    {
        return isRunning ? runSpeed : walkSpeed;
    }
    
    public bool IsGrounded()
    {
        return isGrounded;
    }
    
    public Vector2 GetMoveInput()
    {
        return moveInput;
    }
}
