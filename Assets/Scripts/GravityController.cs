using UnityEngine;

/// <summary>
/// Clase para controlar la gravedad de planetas pequeños
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class GravityController : MonoBehaviour
{
    [Tooltip("Origen de la gravedad")] public Transform gravityOrigin;
    [Tooltip("Vector al origen")] public Vector3 gravityVector;
    [Tooltip("Fuerza de la gravedad")] [SerializeField] public float gravity = 9.81f;
    [Tooltip("Fuerza de la auto orientación")] [SerializeField] private float autoOrientForce = 1f;
    [Tooltip("Rigidbody")] [SerializeField] private Rigidbody rb;
    [Tooltip("Girar Automáticamente")] [SerializeField] private bool autoRotate = true;
    [Tooltip("Aplicar Gravedad")] [SerializeField] private bool applyGravity = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        ProcessGravity();
        ProcessRotation();
    }

    /// <summary>
    /// Aplicar la fuerza de la gravedad hacia el objetivo gravityOrigin
    /// </summary>
    private void ProcessGravity()
    {
        gravityVector = gravityOrigin.position - transform.position;
        if (applyGravity)
            rb.AddForce(gravityVector.normalized * (gravity * rb.mass));
    }

    /// <summary>
    /// Reorientar hacia el objetivo gravityOrigin
    /// </summary>
    private void ProcessRotation()
    {
        var orientation = Quaternion.FromToRotation(-transform.up, gravityVector) * transform.rotation;
        if (autoRotate)
            transform.rotation = Quaternion.Slerp(transform.rotation, orientation, Time.deltaTime * autoOrientForce);
    }
}
