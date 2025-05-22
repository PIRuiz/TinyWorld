using System;
using TMPro;
using UnityEngine;

public class Point : MonoBehaviour
{
    [Tooltip("Objetivo")] public GameObject target;

    [Tooltip("Texto distancia")] [SerializeField]
    private TextMeshProUGUI distanceText;

    private void Update()
    {
        transform.LookAt(target.transform.position);
        distanceText.text = $"Distance: {Vector3.Distance(target.transform.position, transform.position):F}";
    }
}
