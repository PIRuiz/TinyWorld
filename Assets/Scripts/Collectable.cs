using System;
using UnityEngine;

/// <summary>
/// Lógica de los coleccionables
/// </summary>
[RequireComponent(typeof(Collider))]
public class Collectable : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CollectObject();
        }
    }

    /// <summary>
    /// Desactiva el coleccionable en el mundo y modifica el contador. Si es el último activa la salida
    /// </summary>
    private void CollectObject()
    {
        gameObject.SetActive(false);
        GameManager.Instance.targets.Remove(gameObject);
        var remaining = GameManager.Instance.UpdateCollectibles();
        if (remaining <= 0)
            GameManager.Instance.finalTarget.SetActive(true);
    }
}
