using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    [Tooltip("Origen de audio")] [SerializeField]
    private AudioSource audioSource;

    [Tooltip("Instancia pública para facilitar acceso")]public static AudioPlayer Instance;

    private void Awake()
    {
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        if (Instance == null) Instance = this;
    }

    private void OnDisable()
    {
        Instance = null;
    }

    /// <summary>
    /// Reproducir sonido
    /// </summary>
    /// <param name="clip"><see cref="AudioClip">Sonido</see> a reproducir</param>
    public void PlaySfx(AudioClip clip)
    {
        audioSource.pitch = 1 + Random.Range(-0.1f, .1f);
        audioSource.PlayOneShot(clip);
    }
}