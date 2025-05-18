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
    
    /// <summary>
    /// Que ocurre al usar controles de movimiento
    /// </summary>
    /// <param name="input">Valor de entrada</param>
    private void OnMove(InputValue input)
    {
        movementInput = input.Get<Vector2>();
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
            var targetVelocity = Vector3.zero;
            if (isRunning)
            {
                targetVelocity += transform.forward * (movementInput.y * runSpeed);
                targetVelocity += transform.right * (movementInput.x * runSpeed);
            }
            else
            {
                targetVelocity += transform.forward * (movementInput.y * walkSpeed);
                targetVelocity += transform.right * (movementInput.x * walkSpeed);
            }
            targetVelocity += gravityController.gravityVector.normalized * (gravityController.gravity * 0.5f);
            velocity3D = Vector3.Lerp(velocity3D, targetVelocity, acceleration * Time.fixedDeltaTime);
            rb3D.linearVelocity = velocity3D;
        }

        if (movementInput != Vector2.zero)
        {
            transform.forward = transform.position + velocity3D;
            transform.up = -gravityController.gravityVector;
        }
    }
}
