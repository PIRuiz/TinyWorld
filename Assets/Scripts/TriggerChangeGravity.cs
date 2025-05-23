using UnityEngine;

/// <summary>
/// Cambia el origen de la gravedad de los objetos que entran en el trigger
/// </summary>
public class TriggerChangeGravity : MonoBehaviour
{
    [Tooltip("Nuevo punto de gravedad")][SerializeField] private Transform origin;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<GravityController>(out var controller))
            controller.gravityOrigin = origin;
    }
}
