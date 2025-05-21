using System;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Controla el Géiser de Lava
/// </summary>
public class Geyser : MonoBehaviour
{
    [Tooltip("Partículas")] [SerializeField] private ParticleSystem particles;
    [Tooltip("Trigger")] [SerializeField] private MeshCollider trigger;
    [Tooltip("Tiempo activo")] [SerializeField] [Range(1f, 5f)] private float lifetime = 3.5f;
    [Tooltip("Temporizador")] [SerializeField] private float timer;
    [Tooltip("Siguiente desactivación")] [SerializeField] private float nextDisable;
    [Tooltip("Siguiente ignición")] [SerializeField] private float nextIgnition;
    [Tooltip("Géiser activo")] [SerializeField] private bool active;

    private void Start()
    {
        DisableGeyser();
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= nextIgnition && !active) EnableGeyser();
        if (timer >= nextDisable && active) DisableGeyser();
    }

    private void DisableGeyser()
    {
        particles.gameObject.SetActive(false);
        trigger.gameObject.SetActive(false);
        nextIgnition = timer + Random.Range(1, 10f);
        active = false;
    }

    private void EnableGeyser()
    {
        particles.gameObject.SetActive(true);
        trigger.gameObject.SetActive(true);
        nextDisable = timer + lifetime;
        active = true;
    }
}
