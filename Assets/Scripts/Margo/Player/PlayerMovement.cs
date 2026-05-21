using UnityEngine;
using UnityEngine.InputSystem;


using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Скорости передвижения")]
    [SerializeField] private float walkSpeed = 4f;
    [SerializeField] private float runSpeed = 7f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float deceleration = 8f;

    [Header("Настройки прыжка")]
    [SerializeField] private float jumpForce = 20f;

    [Header("Настройки бега")]
    [SerializeField] private bool toggleRun = false;

    private Rigidbody rb;
    private Vector2 moveInput;
    private bool isRunning = false;
    private bool isGrounded = false;

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
        }

        // 🔹 ФИКС: Замораживаем ВСЕ вращения, чтобы игрок не крутился от толчков
        rb.freezeRotation = true;
        rb.constraints = RigidbodyConstraints.FreezeRotationX |
                        RigidbodyConstraints.FreezeRotationY |  // ← ДОБАВЛЕНО
                        RigidbodyConstraints.FreezeRotationZ;

        rb.useGravity = true;
        rb.linearDamping = 0f;
    }

    void OnEnable()
    {
        controls.Enable();

        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        if (toggleRun)
        {
            controls.Player.Run.performed += ctx => isRunning = !isRunning;
        }
        else
        {
            controls.Player.Run.performed += ctx => isRunning = true;
            controls.Player.Run.canceled += ctx => isRunning = false;
        }

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
        if (isGrounded)
        {
            GroundMovement();
        }
        else
        {
            AirMovement();
        }
    }

    private void GroundMovement()
    {
        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        Vector3 moveDirection = transform.right * moveInput.x + transform.forward * moveInput.y;

        if (moveInput != Vector2.zero)
        {
            Vector3 moveVelocity = moveDirection.normalized * currentSpeed;
            moveVelocity.y = rb.linearVelocity.y;
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, moveVelocity, acceleration * Time.fixedDeltaTime);
        }
        else
        {
            // Торможение при отсутствии ввода
            Vector3 horizontalVel = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            horizontalVel = Vector3.Lerp(horizontalVel, Vector3.zero, deceleration * Time.fixedDeltaTime);
            rb.linearVelocity = new Vector3(horizontalVel.x, rb.linearVelocity.y, horizontalVel.z);
        }
    }

    private void AirMovement()
    {
        // Минимальное сопротивление в воздухе — игрок сохраняет инерцию
        Vector3 velocity = rb.linearVelocity;
        velocity.x *= 0.99f;
        velocity.z *= 0.99f;
        rb.linearVelocity = velocity;
    }

    private void Jump()
    {
        if (isGrounded)
        {
            // Сброс вертикальной скорости для стабильной высоты прыжка
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    // 🔹 ТВОЯ ПРОВЕРКА ЗЕМЛИ — оставлена как была, работает надёжно
    void OnCollisionStay(Collision collision)
    {
        // Проверяем, что контакт снизу (нормаль направлена вверх)
        foreach (ContactPoint contact in collision.contacts)
        {
            if (Vector3.Dot(contact.normal, Vector3.up) > 0.7f)
            {
                isGrounded = true;
                return;
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }

    // ───────── Публичные геттеры ─────────
    public bool IsRunning() => isRunning;
    public float GetCurrentSpeed() => isRunning ? runSpeed : walkSpeed;
    public bool IsGrounded() => isGrounded;
    public Vector2 GetMoveInput() => moveInput;
}
/*
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
*/