using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Tooltip("Instancia")] public static GameManager Instance;
    [Tooltip("Lista de Objetivos")] public List<GameObject> targets;
    [Tooltip("Objetivos restantes")] public int remainingTargets;
    [Tooltip("Objetivo final")] public GameObject finalTarget;

    [Tooltip("Texto coleccionables")] [SerializeField]
    private TextMeshProUGUI collectiblesText;
    
    private void Awake()
    {
        if (!Instance) Instance = this;
    }

    private void Start()
    {
        foreach (var obj in targets) remainingTargets++;
        finalTarget.SetActive(false);
        UpdateCollectibles();
    }

    private void OnDisable()
    {
        if (Instance == this) Instance = null;
    }

    public void UpdateCollectibles()
    {
        collectiblesText.text = $"x {remainingTargets}";
    }
}
