using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controlador del protagonista
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class BrokenController : MonoBehaviour
{
    #region Variables de ajuste

    [Header("Movimiento Horizontal")]
    [Tooltip("Velocidad de movimiento. Andar")] [SerializeField] [Range(1, 20f)]
    private float walkSpeed = 10f;
    [Tooltip("Velocidad de movimiento. Correr")] [SerializeField] [Range(1, 30f)]
    private float runSpeed = 20f;
    [Tooltip("Aceleración. Tierra")] [SerializeField] [Range(1, 20f)]
    private float groundAcceleration = 5f;
    [Tooltip("Deceleración. Tierra")] [SerializeField] [Range(1, 20f)]
    private float groundDeceleration = 20f;
    [Tooltip("Aceleración. Aire")] [SerializeField] [Range(1, 20f)]
    private float airAcceleration = 2.5f;
    [Tooltip("Deceleración. Aire")] [SerializeField] [Range(1, 20f)]
    private float airDeceleration = 10f;
    [Tooltip("Máxima velocidad")] [SerializeField] [Range(1, 20f)]
    private float maxSpeed = 20f;

    [Header("Movimiento Vertical")]
    [Tooltip("Duración de tiempo de coyote")] [SerializeField] [Range(0, 2f)]
    private float coyoteTime = .25f;
    [Tooltip("Altura de Salto en Unidades de Unity")] [SerializeField] [Range(0.01f, 100)]
    private float jumpForce = 3;
    [Tooltip("Controlador de gravedad")] [SerializeField] private GravityController gravityController;
    
    [Header("Colisiones")]
    [Tooltip("Capa de terreno 3D")] [SerializeField]
    private LayerMask groundLayer3D;
    [Tooltip("Rigidbody 3D")] [SerializeField]
    private Rigidbody rb3D;
    [Tooltip("Colisionador 3D")] public CapsuleCollider my3DCollider;
    [Tooltip("Rango de detección de colisión")] [SerializeField] [Range(0.01f, 1)]
    private float rayLenght = .2f;
    
    #endregion
    
    #region Variables privadas
    [Header("Debug")]
    [Tooltip("Tiempo de ejecución")] [SerializeField]
    private float timer;
    [Tooltip("Cuando se desactiva el tiempo de Coyote")] [SerializeField]
    private float coyoteTimer;
    [Tooltip("Estamos corriendo")] [SerializeField]
    private bool isRunning;
    [Tooltip("Estamos tocando el suelo")] [SerializeField]
    private bool isGrounded;
    [Tooltip("Estamos en tiempo de coyote")] [SerializeField]
    private bool isCoyoteTime;
    [Tooltip("Vector de movimiento vertical")] [SerializeField]
    private Vector2 movementInput;
    [Tooltip("Velocidad 3D")] [SerializeField]
    private Vector3 velocity3D;
    [Tooltip("Saltando")] [SerializeField]
    private bool isJumping;

    /// <summary>
    /// Se está preparando para saltar
    /// </summary>
    public bool IsJumping
    {
        get => isJumping;
        set => isJumping = value;
    }

    public float JumpForce => jumpForce;

    /// <summary>
    /// Tiempo de ejecución
    /// </summary>
    public float Timer => timer;
    
    #endregion
    
    #region Input

        /// <summary>
        /// Que ocurre al usar controles de movimiento
        /// </summary>
        /// <param name="input">Valor de entrada</param>
        private void OnMove(InputValue input)
        {
            movementInput = input.Get<Vector2>();
        }

        /// <summary>
        /// Que ocurre al usar el botón de sprint
        /// </summary>
        /// <param name="input">Valor de entrada</param>
        private void OnSprint(InputValue input)
        {
            isRunning = input.isPressed;
        }

        /// <summary>
        /// Que ocurre al usar el botón saltar
        /// </summary>
        /// <param name="input">Valor de entrada</param>
        private void OnJump(InputValue input)
        {
            if (input.isPressed)
            {
                if (!isGrounded && !isCoyoteTime) return;
                isJumping = true;
            }
        }

    #endregion
    
    #region Unity

    private void Start()
    {
        rb3D.maxLinearVelocity = maxSpeed;
    }

    private void Update()
    {
        timer += Time.deltaTime;
    }
    
    private void FixedUpdate()
    {
        IsGrounded();
        MovePlayer();
    }
    #endregion
    
    /// <summary>
    /// Movimiento específico 3D
    /// </summary>
    private void MovePlayer()
    {
        if (movementInput != Vector2.zero)
            rb3D.gameObject.transform.forward = new Vector3(movementInput.x, 0, movementInput.y);
        float acceleration;
        float deceleration;
        if (isGrounded)
        {
            acceleration = groundAcceleration;
            deceleration = groundDeceleration;
        }
        else
        {
            acceleration = airAcceleration;
            deceleration = airDeceleration;
        }

        if (movementInput == Vector2.zero)
        {
            velocity3D = Vector3.Lerp(velocity3D, Vector3.zero, deceleration * Time.fixedDeltaTime);
        }
        else
        {
            var targetVelocity = new Vector3();
            if (isRunning)
            {
                targetVelocity.x = movementInput.x * runSpeed;
                targetVelocity.z = movementInput.y * runSpeed;
            }
            else
            {
                targetVelocity.x = movementInput.x * walkSpeed;
                targetVelocity.z = movementInput.y * walkSpeed;
            }
            velocity3D = Vector3.Lerp(velocity3D, targetVelocity, acceleration * Time.fixedDeltaTime);
        }

        velocity3D.y = rb3D.linearVelocity.y;
        rb3D.linearVelocity = velocity3D;
        if (isJumping)
        {
            //rb3D.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            rb3D.AddForce(-gravityController.gravityVector * jumpForce, ForceMode.Impulse);
            isJumping = false;
            isCoyoteTime = false;
        }
    }
    
    /// <summary>
    /// Comprobar si tocamos el suelo y si estamos en tiempo coyote
    /// </summary>
    private void IsGrounded()
    {
        var point = my3DCollider.bounds.center;
        //point.y = my3DCollider.bounds.min.y + my3DCollider.radius;
        if (Physics.SphereCast(
                point,
                my3DCollider.radius * 0.5f,
                //Vector3.down,
                gravityController.gravityVector,
                out _,
                rayLenght + my3DCollider.height/2,
                groundLayer3D))
        {
            isGrounded = true;
            isCoyoteTime = true;
        }
        else
        {
            if (isGrounded) coyoteTimer = timer + coyoteTime;
            isGrounded = false;
        }
        if (!isGrounded && coyoteTimer < timer) isCoyoteTime = false;
        //if (is3D && !isGrounded && isCoyoteTime && rb3D.linearVelocity.y > 0) isCoyoteTime = false;
        if (!isGrounded && isCoyoteTime && rb3D.linearVelocity.y > 0) 
            isCoyoteTime = false;
    }
}
