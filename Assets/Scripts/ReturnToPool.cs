using System;
using UnityEngine;

public class ReturnToPool : MonoBehaviour
{
    [Tooltip("Pool a la que volver")] public ObjPool pool;
    [Tooltip("Contador")] [SerializeField] private float timer;
    [Tooltip("Duración")] [SerializeField] private float lifetime = 10f;

    private void OnEnable()
    {
        timer = 0;
    }

    private void Update()
    {
        if (timer >= lifetime)
            gameObject.SetActive(false);
        timer += Time.deltaTime;
    }

    private void OnDisable()
    {
        pool.Pool.Release(gameObject);
    }
}
