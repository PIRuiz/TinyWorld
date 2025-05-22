using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

/// <summary>
/// Controlador del protagonista
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour, IHealth
{
    #region Variables
    
    [Header("Movimiento Horizontal")]
    [Tooltip("Velocidad de movimiento. Andar")] [SerializeField] [Range(1, 20f)] private float walkSpeed = 10f;
    [Tooltip("Velocidad de movimiento. Correr")] [SerializeField] [Range(1, 30f)] private float runSpeed = 20f;
    [Tooltip("Aceleración. Tierra")] [SerializeField] [Range(1, 20f)] private float groundAcceleration = 5f;
    [Tooltip("Deceleración. Tierra")] [SerializeField] [Range(1, 20f)] private float groundDeceleration = 20f;
    [Tooltip("Aceleración. Aire")] [SerializeField] [Range(1, 20f)] private float airAcceleration = 2.5f;
    [Tooltip("Deceleración. Aire")] [SerializeField] [Range(1, 20f)] private float airDeceleration = 10f;
    [Tooltip("Máxima velocidad")] [SerializeField] [Range(1, 20f)] private float maxSpeed = 20f;
    [Tooltip("Vector de movimiento vertical")] [SerializeField] private Vector2 movementInput;
    [Tooltip("Suavizado de movimiento horizontal")] [SerializeField] [Range(0.1f, 1f)]
    private float horizontalSmooth = 0.5f;
    [Tooltip("Movimiento de tanque")] public bool tankControl;
    
    [Header("Colisiones")]
    [Tooltip("Capa de terreno 3D")] [SerializeField]
    private LayerMask groundLayer3D;
    [Tooltip("Rigidbody 3D")] [SerializeField]
    private Rigidbody rb3D;
    [Tooltip("Colisionador 3D")] public CapsuleCollider my3DCollider;
    [Tooltip("Rango de detección de colisión")] [SerializeField] [Range(0.01f, 1)]
    private float rayLenght = .2f;
    
    [Header("Movimiento Vertical")]
    [Tooltip("Duración de tiempo de coyote")] [SerializeField] [Range(0, 2f)] private float coyoteTime = .25f;
    [Tooltip("Altura de Salto en Unidades de Unity")] [SerializeField] [Range(0.01f, 100)] private float jumpForce = 3;
    [Tooltip("Controlador de gravedad")] public GravityController gravityController;
    
    [Header("Animación")]
    [Tooltip("Animator")] [SerializeField] private Animator animator;
    [Tooltip("Camara de persecución")] [SerializeField] private CinemachineThirdPersonFollow followCamera;
    [Tooltip("Distancia de la cámara normal")] [SerializeField] private float normalCameraDistance = 3f;
    [Tooltip("Distancia de la cámara en sprint")] [SerializeField] private float sprintCameraDistance = 2f;
    [Tooltip("Renderizador")] [SerializeField] private SkinnedMeshRenderer renderer3d;
    [Tooltip("Tiempo invulnerable")] [SerializeField] [Range(0.1f, 5f)] private float invulnerableDuration = 1f;
    
    [Header("Vida")]
    [Tooltip("Vida inicial del jugador")] [SerializeField] [Range(1, 100)] private int maxHealth = 10;

    [Header("Audio")]
    [Tooltip("SFX daño")] [SerializeField] private AudioClip sfxDamage;
    [Tooltip("SFX salto")] [SerializeField] private AudioClip sfxJump;
    [Tooltip("Audio Source")] [SerializeField] private AudioSource audioSource;
    
    [Header("Debug")]
    [Tooltip("Tiempo de ejecución")] [SerializeField] private float timer;
    [Tooltip("Cuando se desactiva el tiempo de Coyote")] [SerializeField] private float coyoteTimer;
    [Tooltip("Estamos corriendo")] [SerializeField] private bool isRunning;
    [Tooltip("Estamos tocando el suelo")] [SerializeField] private bool isGrounded;
    [Tooltip("Estamos en tiempo de coyote")] [SerializeField] private bool isCoyoteTime;
    [Tooltip("Velocidad 3D")] [SerializeField] private Vector3 velocity3D;
    [Tooltip("Saltando")] [SerializeField] private bool isJumping;
    [Tooltip("Dañado")] [SerializeField] private bool isDamaged;
    [Tooltip("Parpadeando")] [SerializeField] private bool isInvulnerable;
    [Tooltip("Invulnerable hasta")] [SerializeField] private float blinkingTimer;
    [Tooltip("Vector del golpe")] [SerializeField] private Vector3 hitVector;
    
    private static readonly int HSpeed = Animator.StringToHash("HSpeed");
    private static readonly int Grounded = Animator.StringToHash("Grounded");
    
    private Camera _mainCamera;
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
    /// Que ocurre al usar el botón saltar
    /// </summary>
    /// <param name="input">Valor de entrada</param>
    private void OnJump(InputValue input)
    {
        if (input.isPressed)
        {
            if (!isGrounded && !isCoyoteTime) return;
            isJumping = true;
            audioSource.PlayOneShot(sfxJump);
        }
    }
    
    /// <summary>
    /// Que ocurre al usar el botón de sprint
    /// </summary>
    /// <param name="input">Valor de entrada</param>
    private void OnSprint(InputValue input)
    {
        isRunning = input.isPressed;
    }
    
    #endregion

    #region Unity

    private void Awake()
    {
        OnHealthChange ??= new UnityEvent();
        MaxHealth = maxHealth;
        RestoreHealth();
        _mainCamera = Camera.main;
    }

    private void Start()
    {
        rb3D.maxLinearVelocity = maxSpeed;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        SprintAnim();
        BlinkingAnim();
    }
    
    private void FixedUpdate()
    {
        IsGrounded();
        MovePlayer();
    }
    
    #endregion
    
    #region Movement

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
    /// Movimiento del jugador
    /// </summary>
    private void MovePlayer()
    {
        float acceleration = isGrounded ? groundAcceleration : airAcceleration;
        float deceleration = isGrounded ? groundDeceleration : airDeceleration;
        Vector3 surfaceUp = -gravityController.gravityVector.normalized;
        Vector3 forward;
        Vector3 right;
        if (tankControl)
        {
            forward = Vector3.ProjectOnPlane(transform.forward, surfaceUp).normalized;
            right = Vector3.ProjectOnPlane(transform.right, surfaceUp).normalized;
        }
        else
        {
            forward = Vector3.ProjectOnPlane(_mainCamera.transform.forward, surfaceUp).normalized;
            right = Vector3.ProjectOnPlane(_mainCamera.transform.right, surfaceUp).normalized;
        }
        //Vector3 right = Vector3.Cross(surfaceUp, forward).normalized;
        
        Vector3 inputDirection = forward * movementInput.y + right * (movementInput.x * horizontalSmooth);
        
        Vector3 targetHorizontalVelocity = inputDirection * (isRunning ? runSpeed : walkSpeed);
        Vector3 currentHorizontalVelocity = Vector3.ProjectOnPlane(rb3D.linearVelocity, surfaceUp);

        Vector3 newHorizontalVelocity;
        
        if (movementInput == Vector2.zero)
        {
            newHorizontalVelocity = Vector3.Lerp(
                currentHorizontalVelocity,
                (inputDirection * 0),
                deceleration * Time.fixedDeltaTime);
        }
        else
        {
            newHorizontalVelocity = Vector3.Lerp(
                currentHorizontalVelocity,
                targetHorizontalVelocity,
                acceleration * Time.fixedDeltaTime);
        }
        
        if (isJumping)
        {
            rb3D.AddForce(surfaceUp * jumpForce, ForceMode.Impulse);
            isJumping = false;
            isCoyoteTime = false;
        }/*
        else
        {
            rb3D.AddForce(-surfaceUp * gravityController.gravity, ForceMode.Force);
        }*/

        if (isDamaged)
        {
            rb3D.AddForce(hitVector * (jumpForce), ForceMode.Impulse);
            isDamaged = false;
        }
        
        Vector3 verticalVelocity = Vector3.Project(rb3D.linearVelocity, surfaceUp);

        if (!isJumping && !isDamaged)
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
    #endregion Movement

    #region Animation

    /// <summary>
    /// Acerca la cámara durante un sprint y la aleja el resto del tiempo
    /// </summary>
    private void SprintAnim()
    {
        if (isRunning)
        {
            if (followCamera.CameraDistance > sprintCameraDistance)
                followCamera.CameraDistance -= Time.deltaTime;
        }
        else
        {
            if (followCamera.CameraDistance < normalCameraDistance)
                followCamera.CameraDistance += Time.deltaTime;
        }
    }
    
    /// <summary>
    /// Hace parpadear intermitentemente el personaje tras recibir daño
    /// </summary>
    private void BlinkingAnim()
    {
        if (isInvulnerable)
        {
            renderer3d.enabled = !renderer3d.enabled;
            if (timer >= blinkingTimer)
            {
                isInvulnerable = false;
                renderer3d.enabled = true;
            }
        }
    }

    #endregion
    
    #region Vida

    /// <summary>
    /// Cantidad de vida actual
    /// </summary>
    public int Health { get; private set; }

    /// <summary>
    /// Salud Máxima
    /// </summary>
    public int MaxHealth { get; private set; }

    /// <summary>
    /// Evento cambiar la cantidad de vida
    /// </summary>
    public UnityEvent OnHealthChange { get; private set; }

    /// <summary>
    /// Configurar la salud máxima
    /// </summary>
    /// <param name="amount">Nueva salud máxima</param>
    public void SetMaxHealth(int amount)
    {
        MaxHealth = amount;
        RestoreHealth();
        OnHealthChange.Invoke();
    }

    /// <summary>
    /// Recibir daño
    /// </summary>
    /// <param name="amount">Daño recibido</param>
    public void TakeDamage(int amount)
    {
        Health -= amount;
        if (Health <= 0) HandleDeath();
        OnHealthChange.Invoke();
        audioSource.PlayOneShot(sfxDamage);
        isInvulnerable = true;
        blinkingTimer = timer + invulnerableDuration;
    }

    /// <summary>
    /// Recibir daño con posición
    /// </summary>
    /// <param name="amount">Daño recibido</param>
    /// <param name="damageOrigin">Origen del daño</param>
    public void TakeDamage(int amount, Vector3 damageOrigin)
    {
        if (isInvulnerable) return;
        TakeDamage(amount);
        isDamaged = true;
        hitVector = (transform.position + transform.up) - damageOrigin;
    }

    /// <summary>
    /// Reiniciar salud al máximo
    /// </summary>
    public void RestoreHealth()
    {
        Health = MaxHealth;
        OnHealthChange.Invoke();
    }

    /// <summary>
    /// Controlar muerte
    /// </summary>
    public void HandleDeath()
    {
        RestoreHealth();
        var respawn = GameObject.FindGameObjectWithTag("Respawn");
        transform.position = respawn.transform.position;
    }

    #endregion Vida
}
