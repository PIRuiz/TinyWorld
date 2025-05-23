using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Collectable : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameObject.SetActive(false);
            GameManager.Instance.targets.Remove(gameObject);
            var remaining = GameManager.Instance.UpdateCollectibles();
            if (remaining <= 0)
                GameManager.Instance.finalTarget.SetActive(true);
        }
    }
}
