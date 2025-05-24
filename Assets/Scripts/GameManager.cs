using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Manager general del juego
/// </summary>
public class GameManager : MonoBehaviour
{
    [Tooltip("Instancia")] public static GameManager Instance;
    [Tooltip("Lista de Objetivos")] public List<GameObject> targets;
    [Tooltip("Objetivos restantes")] public int remainingTargets;
    [Tooltip("Objetivo final")] public GameObject finalTarget;

    [Tooltip("Texto coleccionables")] [SerializeField]
    private TextMeshProUGUI collectiblesText;
    [Tooltip("Panel Opciones")] [SerializeField]
    private GameObject optionsPanel;
    
    private void Awake()
    {
        if (!Instance) Instance = this;
        targets = new List<GameObject>();
        foreach (var collectable in FindObjectsByType<Collectable>(FindObjectsSortMode.None))
        {
            targets.Add(collectable.gameObject);
        }
    }

    private void Start()
    {
        finalTarget.SetActive(false);
        UpdateCollectibles();
    }

    private void OnDisable()
    {
        if (Instance == this) Instance = null;
    }

    /// <summary>
    /// Contar objetos coleccionables
    /// </summary>
    private void CountCollectibles()
    {
        remainingTargets = targets.Count;
    }

    /// <summary>
    /// Cuenta cuantos artículos coleccionables quedan, actualiza el texto en canvas y devuelve el valor
    /// </summary>
    /// <returns>Cantidad de coleccionables restantes</returns>
    public int UpdateCollectibles()
    {
        CountCollectibles();
        collectiblesText.text = $"x {remainingTargets}";
        return remainingTargets;
    }

    /// <summary>
    /// Pausar el juego
    /// </summary>
    public void PauseGame()
    {
        Time.timeScale = 0;
        optionsPanel.SetActive(true);
    }
}
