using UnityEngine;
using UnityEngine.SceneManagement;

public class Meteor : MonoBehaviour
{
    [Tooltip("Controlador de gravedad")] public GravityController gravityController;
    [Tooltip("Pool")] public ObjPool pool;
    [Tooltip("Pool de meteoritos pequeños")] public ObjPool smallPool;
    
    private void OnDisable()
    {
        var rnd = Random.Range(3, 7);
        for (var i = 0; i < rnd; i++)
        {
            var newMeteor = smallPool.Pool.Get();
            newMeteor.transform.position = transform.position + Vector3.one * Random.Range(0, .25f);
            newMeteor.transform.rotation = transform.rotation;
            if (newMeteor.TryGetComponent<ReturnToPool>(out var returnPool))
                returnPool.pool = smallPool;
            if (newMeteor.TryGetComponent<GravityController>(out var gravity))
                gravity.gravityOrigin = gravityController.gravityOrigin;
        }
        if (SceneManager.GetActiveScene().isLoaded) pool.Pool.Release(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        gameObject.SetActive(false);
    }
}
