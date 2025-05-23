using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out var player) && GameManager.Instance.remainingTargets == 0)
            SceneManager.LoadScene(0);
    }
}
