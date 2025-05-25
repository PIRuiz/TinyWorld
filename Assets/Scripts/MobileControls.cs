using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class MobileControls : MonoBehaviour
{
    [Tooltip("Panel de Control en pantalla")]
    public GameObject panelCtl;
    [Tooltip("Toggle de Control en pantalla")]
    public Toggle toggleCtl;
    [Tooltip("CineMachine Input")]
    public CinemachineInputAxisController cineMachineInput;
    [Tooltip("CineMachine Orbital")]
    public CinemachineOrbitalFollow cineMachineOrbital;
    
    void Start()
    {
        SetOnScreenControls(Application.isMobilePlatform);
    }

    public void SetOnScreenControls(bool value)
    {
        panelCtl.SetActive(value);
        toggleCtl.isOn = value;
        cineMachineInput.enabled = !value;
        if (value)
        {
            cineMachineOrbital.HorizontalAxis.Value = 0;
            cineMachineOrbital.VerticalAxis.Value = 17.5f;
        }
    }
}
