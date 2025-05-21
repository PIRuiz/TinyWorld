using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Orbit : MonoBehaviour
{
    [Tooltip("Origen de la gravedad")] public Transform gravityOrigin;
    [Tooltip("Vector al origen")] public Vector3 gravityVector;
    [Tooltip("Fuerza de la gravedad")] [SerializeField] public float gravity = 9.81f;
    [Tooltip("Fuerza de la auto orientación")] [SerializeField] private float autoOrientForce = 1f;
    [Tooltip("Rigidbody")] [SerializeField] private Rigidbody rb;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearDamping = 0f;
        rb.angularDamping = 0f;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        ApplyInitialOrbitalVelocity();
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
        rb.AddForce((gravityVector.normalized) * (gravity * rb.mass), ForceMode.Force);
    }

    /// <summary>
    /// Reorientar hacia el objetivo gravityOrigin
    /// </summary>
    private void ProcessRotation()
    {
        var orientation = Quaternion.FromToRotation(-transform.up, gravityVector) * transform.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, orientation, Time.deltaTime * autoOrientForce);
    }
    
    /// <summary>
    /// Aplica velocidad inicial para orbitar
    /// </summary>
    private void ApplyInitialOrbitalVelocity()
    {
        gravityVector = (gravityOrigin.position - transform.position).normalized;

        Vector3 tangentialDirection = Vector3.Cross(gravityVector, transform.right).normalized;
        
        float distance = Vector3.Distance(transform.position, gravityOrigin.position);
        float orbitalVelocity = Mathf.Sqrt(gravity * distance); // v = √(g * r)

        rb.linearVelocity = tangentialDirection * orbitalVelocity;
    }
}
