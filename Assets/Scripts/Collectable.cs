using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Collectable : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.remainingTargets--;
            gameObject.SetActive(false);
            GameManager.Instance.targets.Remove(gameObject);
        }
    }
}
