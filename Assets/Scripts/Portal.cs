using System;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [Tooltip("Destino")] [SerializeField] private Transform destination;
    [Tooltip("Gravedad en destino")] [SerializeField] private Transform destinationGravity;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out var player))
        {
            player.transform.position = destination.position;
            player.gravityController.gravityOrigin = destinationGravity;
        }
    }
}
