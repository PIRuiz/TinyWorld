using UnityEngine;

public class MeteorShower : MonoBehaviour
{
    [Tooltip("Jugador")] [SerializeField] private PlayerController player;
    [Tooltip("Contador")] [SerializeField] private float timer;
    [Tooltip("Frecuencia mínima")] [SerializeField] private float minSpawnRate = 1f;
    [Tooltip("Frecuencia máxima")] [SerializeField] private float maxSpawnRate = 10f;
    [Tooltip("Multiplicador")] [SerializeField] private float multiplier = 5f;
    [Tooltip("Siguiente meteorito")] [SerializeField] private float nextMeteor;
    [Tooltip("Pool de meteoritos")] [SerializeField] private ObjPool pool;
    [Tooltip("Pool de meteoritos pequeños")] [SerializeField] private ObjPool smallPool;

    private float GetRandom()
    {
        return Random.Range(minSpawnRate, maxSpawnRate);
    }

    private void Start()
    {
        nextMeteor = timer + GetRandom() * multiplier;
    }

    private void Update()
    {
        if (timer >= nextMeteor)
        {
            var meteor = pool.Pool.Get();
            var rnd = GetRandom() * multiplier;
            meteor.transform.position = player.transform.position + player.transform.up * rnd;
            if (meteor.TryGetComponent<Meteor>(out var newMeteor))
            {
                newMeteor.gravityController.gravityOrigin = player.gravityController.gravityOrigin;
                newMeteor.gravityController.rb.linearVelocity = Vector3.zero;
                newMeteor.gravityController.rb.angularVelocity = Vector3.zero;
                newMeteor.pool = pool;
                newMeteor.smallPool = smallPool;
            }
            nextMeteor = timer + rnd;
        }
        timer += Time.deltaTime;
    }
}
