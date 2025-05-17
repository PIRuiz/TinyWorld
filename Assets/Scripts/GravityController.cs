using System;
using UnityEngine;

/// <summary>
/// Clase para controlar la gravedad de planetas pequeños
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class GravityController : MonoBehaviour
{
    [Tooltip("Origen de la gravedad")] public Transform gravityOrigin;
    [Tooltip("Vector al origen")] public Vector3 gravityVector;
    [Tooltip("Fuerza de la gravedad")] [SerializeField] private float gravity = 9.81f;
    [Tooltip("Fuerza de la auto orientación")] [SerializeField] private float autoOrientForce = 1f;
    
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        ProcessGravity();
    }

    /// <summary>
    /// Aplicar la fuerza de la gravedad hacia el objetivo gravityOrigin y reorientar
    /// </summary>
    private void ProcessGravity()
    {
        gravityVector = gravityOrigin.position - transform.position;
        rb.AddForce(gravityVector.normalized * (gravity * rb.mass));
        Quaternion orientation = Quaternion.FromToRotation(-transform.up, gravityVector) * transform.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, orientation, Time.deltaTime * autoOrientForce);
    }
}
