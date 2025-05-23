using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Tooltip("Instancia")] public static GameManager Instance;
    [Tooltip("Lista de Objetivos")] public List<GameObject> targets;
    [Tooltip("Objetivos restantes")] public int remainingTargets;
    [Tooltip("Objetivo final")] public GameObject finalTarget;
    
    private void Awake()
    {
        if (!Instance) Instance = this;
    }

    private void Start()
    {
        foreach (var obj in targets) remainingTargets++;
        finalTarget.SetActive(false);
    }

    private void OnDisable()
    {
        if (Instance == this) Instance = null;
    }
}
