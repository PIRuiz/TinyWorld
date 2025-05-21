using System;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Controla la lógica de las balas
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    [Tooltip("Rigidbody")] [SerializeField] private Rigidbody rb;

    [Tooltip("Temporizador")] [SerializeField] private float timer;

    [Tooltip("Velocidad")] [SerializeField] [Range(1f, 10f)] private float speed = 5f;
    
    [Tooltip("Tiempo de vida máximo")] [SerializeField] private float duration;
    
    [Tooltip("Pool")] public ObjPool pool;

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= duration) gameObject.SetActive(false);
        rb.MovePosition(transform.position + transform.forward * (speed * Time.deltaTime));
    }

    private void OnDisable()
    {
        if (SceneManager.GetActiveScene().isLoaded) pool.Pool.Release(gameObject);
    }

    private void OnEnable()
    {
        timer = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Enemy")) gameObject.SetActive(false);
    }
}
