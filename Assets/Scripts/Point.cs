using System;
using TMPro;
using UnityEngine;

public class Point : MonoBehaviour
{
    [Tooltip("Objetivo")] public GameObject target;

    [Tooltip("Texto distancia")] [SerializeField] private TextMeshProUGUI distanceText;
    
    [Tooltip("Tiempo de ejecución")] [SerializeField] private float timer;

    [Tooltip("Tiempo de refresco")] [SerializeField] [Range(0.1f, 5f)] private float refresh = 1f;

    private void Start()
    {
        GetNewTarget();
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= refresh)
        {
            timer = 0;
            GetNewTarget();
        }
        transform.LookAt(target.transform.position);
        distanceText.text = $"{Vector3.Distance(target.transform.position, transform.position):F} M";
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
