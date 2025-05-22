using System;
using TMPro;
using UnityEngine;

public class Point : MonoBehaviour
{
    [Tooltip("Objetivo")] public GameObject target;

    [Tooltip("Texto distancia")] [SerializeField] private TextMeshProUGUI distanceText;
    
    [Tooltip("Tiempo de ejecución")] [SerializeField] private float timer;

    private void Start()
    {
        GetNewTarget();
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= 3)
        {
            timer = 0;
            GetNewTarget();
        }
        transform.LookAt(target.transform.position);
        distanceText.text = $"Distance: {Vector3.Distance(target.transform.position, transform.position):F}";
    }
    
    private void GetNewTarget()
    {
        if (GameManager.Instance.targets.Count == 0)
        {
            gameObject.SetActive(false);
            return;
        }
        GameObject temp = GameManager.Instance.targets[0];
        foreach (var collectable in GameManager.Instance.targets)
        {
            if (collectable.activeSelf &&
                Vector3.Distance(collectable.transform.position, transform.position) < 
                Vector3.Distance(temp.transform.position, transform.position))
                temp = collectable;
        }
        target = temp;
    }
}
