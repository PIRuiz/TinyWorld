using System;
using UnityEngine;

/// <summary>
/// Hace girar con el mismo sentido que la cámara
/// </summary>
public class RotateWithCamera : MonoBehaviour
{
    private Camera mCamera;

    private void Start()
    {
        mCamera = Camera.main;
    }

    private void Update()
    {
        transform.rotation = mCamera.transform.rotation;
    }
}
