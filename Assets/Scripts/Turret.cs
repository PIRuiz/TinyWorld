using System;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Controla la lógica de la torreta
/// </summary>
public class Turret : MonoBehaviour
{
    [Tooltip("Cabeza de torreta")] [SerializeField]
    private Transform turretHead;

    [Tooltip("Objetivo")] [SerializeField] private PlayerController player;

    [Tooltip("Objetivo avistado")] [SerializeField] private bool targetLocked;

    [Tooltip("Contador")] [SerializeField] private float timer;
    
    [Tooltip("Vector Objetivo")] [SerializeField] private Vector3 targetVector;

    [Tooltip("Cadencia de fuego")] [SerializeField] private float rateOfFire = 3f;

    [Tooltip("Siguiente disparo")] [SerializeField] private float nextShoot;
    
    [Tooltip("Pool de balas")] [SerializeField] private ObjPool bulletPool;
    
    [Tooltip("Posición del cañon")] [SerializeField] private Transform firePosition;

    [Tooltip("Ajuste vertical")] [SerializeField] [Range(0, 5f)] private float verticalAdjustment = 0.5f;

    [Tooltip("SFX enemigo avistado")] [SerializeField] private AudioClip targetSighted;
    
    [Tooltip("Audio Source")] [SerializeField] private AudioSource audioSource;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out var target))
        {
            player = target;
            targetLocked = true;
            nextShoot = Time.time + rateOfFire;
            audioSource.PlayOneShot(targetSighted);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out var target))
        {
            player = null;
            targetLocked = false;
        }
    }

    private void Start()
    {
        targetVector = transform.position + Random.onUnitSphere;
    }

    private void Update()
    {
        UpdateTimer();
        RotateTurret();
        FireBullet();
    }
    
    /// <summary>
    /// Actualizar temporizador
    /// </summary>
    private void UpdateTimer()
    {
        timer += Time.deltaTime;
    }

    /// <summary>
    /// Si hay objetivo rota la "Cabeza" de la torreta la objetivo, si no hay objetivo da vueltas
    /// </summary>
    private void RotateTurret()
    {
        if (targetLocked)
        {
            targetVector = Vector3.Slerp(
                targetVector,
                player.transform.position + player.transform.up.normalized,
                Time.deltaTime);
        }
        else
        {
            Vector3 circularOffset =
                transform.right * Mathf.Cos(timer) +
                transform.forward * Mathf.Sin(timer);

            Vector3 newVector = transform.position + circularOffset + transform.up * verticalAdjustment;
            targetVector = (Vector3.Slerp(
                targetVector,
                newVector,
                Time.deltaTime));
        }
        turretHead.transform.LookAt(targetVector);
    }

    /// <summary>
    /// Lógica de disparo
    /// </summary>
    private void FireBullet()
    {
        if (targetLocked && nextShoot <= Time.time)
        {
            var bullet = bulletPool.Pool.Get();
            var lookVector = player.transform.position + player.transform.up.normalized;
            bullet.transform.position = firePosition.position;
            bullet.transform.LookAt(lookVector);
            bullet.transform.parent = null;
            if (bullet.TryGetComponent<Bullet>(out var bulletScript))
            {
                bulletScript.pool = bulletPool;
                bulletScript.moveVector = lookVector - firePosition.position;
            }
            nextShoot = Time.time + rateOfFire;
        }
    }
}
