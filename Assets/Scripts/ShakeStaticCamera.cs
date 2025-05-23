using UnityEngine;

public class ShakeStaticCamera : MonoBehaviour
{
    private Camera cam;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        cam.transform.rotation = Quaternion.Euler(
            Random.Range(0, 1f),
            Random.Range(0, 1f),
            Random.Range(0, 1f));
            
    }
}
