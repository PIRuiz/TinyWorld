using UnityEngine;

public class TriggerDamage : MonoBehaviour
{
    [Tooltip("Daño a realizar")] [SerializeField] [Range(0, 10)] private int damageDealt = 1;

    private float _timer;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out var player))
        {
            player.TakeDamage(damageDealt, other.ClosestPointOnBounds(player.transform.position));
        }
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out var player))
        {
            _timer += Time.deltaTime;
            if (_timer >= 1)
            {
                player.TakeDamage(damageDealt, other.ClosestPointOnBounds(player.transform.position));
                _timer = 0;
            }
            
        }
    }
}
