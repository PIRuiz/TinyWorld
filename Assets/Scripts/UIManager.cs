using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Manager de la UI del juego
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("Audio Mixers con los que controlar el volumen")]
    [SerializeField] [Tooltip("AudioMixer de Master")]
    private AudioMixerGroup audioMixerMaster;
    [SerializeField] [Tooltip("AudioMixer de Efectos")]
    private AudioMixerGroup audioMixerEffects;
    [SerializeField] [Tooltip("AudioMixer de Música")]
    private AudioMixerGroup audioMixerMusic;

    [Header("Sliders con los que modificar los valores")]
    [SerializeField] [Tooltip("Slider de Master")]
    private Slider sliderMaster;
    [SerializeField] [Tooltip("Slider de Efectos")]
    private Slider sliderEffects;
    [SerializeField] [Tooltip("Slider de Música")]
    private Slider sliderMusic;

    [SerializeField] private GameObject LoadLevelAnim;

    
    /// <summary>
    /// Salir del juego
    /// </summary>
    public void OnClickExitButton()
    {
        // Sal del juego
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
    }
    
    public void SetTimeScale(float value)
    {
        Time.timeScale = value;
    }

    public void OnClickPlayButton()
    {
        LoadLevelAnim.SetActive(true);
        Invoke(nameof(LoadLevel1), 3f);
    }

    private void LoadLevel1()
    {
        SceneManager.LoadScene(1);
    }
    
    private void OnEnable()
    {
        audioMixerMaster.audioMixer.GetFloat("VolumeMaster", out float VolumeMaster);
        sliderMaster.value = DecibelToLinear(VolumeMaster);
        audioMixerEffects.audioMixer.GetFloat("VolumeEffects", out float VolumeEffects);
        sliderEffects.value = DecibelToLinear(VolumeEffects);
        audioMixerMusic.audioMixer.GetFloat("VolumeMusic", out float VolumeMusic);
        sliderMusic.value = DecibelToLinear(VolumeMusic);
    }
    /// <summary>
    /// Configurar volumen global
    /// </summary>
    /// <param name="level">Nuevo volumen</param>
    public void SetMasterVolume(float level)
    {
        audioMixerMaster.audioMixer.SetFloat("VolumeMaster", LinearToDecibel(level));
    }
    /// <summary>
    /// Configurar volumen de música
    /// </summary>
    /// <param name="level">Nuevo volumen</param>
    public void SetMusicVolume(float level)
    {
        audioMixerMusic.audioMixer.SetFloat("VolumeMusic", LinearToDecibel(level));
        
    }
    /// <summary>
    /// Configurar volumen de los efectos de sonido
    /// </summary>
    /// <param name="level">Nuevo volumen</param>
    public void SetEffectsVolume(float level)
    {
        audioMixerEffects.audioMixer.SetFloat("VolumeEffects", LinearToDecibel(level));
    }
    /// <summary>
    /// Convertir valor lineal de 0 a 1 a valor en decibeles
    /// </summary>
    /// <param name="linear">Valor lineal a cambiar</param>
    /// <returns>Valor convertido en decibeles</returns>
    private float LinearToDecibel(float linear)
    {
        float dB;
        if (linear != 0)
            dB = 20.0f * Mathf.Log10(linear);
        else
            dB = -144.0f;
        return dB;
    }
    /// <summary>
    /// Convertir valor en decibeles a valor lineal de 0 a 1
    /// </summary>
    /// <param name="dB">Valor en decibeles a cambiar</param>
    /// <returns>Valor convertido en formato lineal</returns>
    private float DecibelToLinear(float dB)
    {
        float linear = Mathf.Pow(10.0f, dB / 20.0f);
        return linear;
    }

    public void ShowCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void HideCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    private void Start()
    {
        LoadLevelAnim.SetActive(false);
    }
}
