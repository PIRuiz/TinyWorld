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
            if (GameManager.Instance.remainingTargets == 0)
                GameManager.Instance.finalTarget.SetActive(true);
            gameObject.SetActive(false);
            GameManager.Instance.targets.Remove(gameObject);
            GameManager.Instance.UpdateCollectibles();
        }
    }
}
