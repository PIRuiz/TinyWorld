using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controlador del protagonista
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
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
    [Tooltip("Vector de movimiento vertical")] [SerializeField]
    private Vector2 movementInput;
    
    [Header("Colisiones")]
    [Tooltip("Capa de terreno 3D")] [SerializeField]
    private LayerMask groundLayer3D;
    [Tooltip("Rigidbody 3D")] [SerializeField]
    private Rigidbody rb3D;
    [Tooltip("Colisionador 3D")] public CapsuleCollider my3DCollider;
    [Tooltip("Rango de detección de colisión")] [SerializeField] [Range(0.01f, 1)]
    private float rayLenght = .2f;
    
    [Header("Movimiento Vertical")]
    [Tooltip("Duración de tiempo de coyote")] [SerializeField] [Range(0, 2f)]
    private float coyoteTime = .25f;
    [Tooltip("Altura de Salto en Unidades de Unity")] [SerializeField] [Range(0.01f, 100)]
    private float jumpForce = 3;
    [Tooltip("Controlador de gravedad")] [SerializeField] private GravityController gravityController;
    
    [Header("Animación")]
    [Tooltip("Animator")] [SerializeField] Animator animator;
    
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
    [Tooltip("Velocidad 3D")] [SerializeField]
    private Vector3 velocity3D;
    [Tooltip("Saltando")] [SerializeField]
    private bool isJumping;
    
    private static readonly int HSpeed = Animator.StringToHash("HSpeed");
    private static readonly int Grounded = Animator.StringToHash("Grounded");

    /// <summary>
    /// Que ocurre al usar controles de movimiento
    /// </summary>
    /// <param name="input">Valor de entrada</param>
    private void OnMove(InputValue input)
    {
        movementInput = input.Get<Vector2>();
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
    
    private void Start()
    {
        rb3D.maxLinearVelocity = maxSpeed;
    }

    private void FixedUpdate()
    {
        IsGrounded();
        MovePlayer();
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

    /// <summary>
    /// Movimiento
    /// </summary>
    private void MovePlayer()
    {
        float acceleration = isGrounded ? groundAcceleration : airAcceleration;
        float deceleration = isGrounded ? groundDeceleration : airDeceleration;
        Vector3 surfaceUp = -gravityController.gravityVector.normalized;
        Vector3 forward = Vector3.ProjectOnPlane(transform.forward, surfaceUp).normalized;
        Vector3 right = Vector3.Cross(surfaceUp, forward).normalized;
        
        Vector3 inputDirection = (forward * movementInput.y + right * movementInput.x).normalized;
        
        Vector3 targetHorizontalVelocity = inputDirection * (isRunning ? runSpeed : walkSpeed);
        Vector3 currentHorizontalVelocity = Vector3.ProjectOnPlane(rb3D.linearVelocity, surfaceUp);
        
        Vector3 newHorizontalVelocity = Vector3.Lerp(
            currentHorizontalVelocity,
            targetHorizontalVelocity,
            acceleration * Time.fixedDeltaTime);
        
        if (isJumping)
        {
            rb3D.AddForce(surfaceUp * jumpForce, ForceMode.Impulse);
            isJumping = false;
            isCoyoteTime = false;
        }
        else
        {
            rb3D.AddForce(-surfaceUp * gravityController.gravity, ForceMode.Force);
        }
        
        Vector3 verticalVelocity = Vector3.Project(rb3D.linearVelocity, surfaceUp);

        if (!isJumping)
        {
            rb3D.linearVelocity = newHorizontalVelocity + verticalVelocity;
        }
        
        if (movementInput != Vector2.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(inputDirection, surfaceUp);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 1f * Time.fixedDeltaTime);
        }
        
        animator.SetFloat(HSpeed, currentHorizontalVelocity.magnitude);
        animator.SetBool(Grounded, isGrounded);
    }
}
