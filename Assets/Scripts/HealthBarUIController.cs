using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Clase para mostrar la vida en una Imagen de UI de tipo Filled
/// </summary>
public class HealthBarUIController : MonoBehaviour
{
    [Header("Componentes")] [Tooltip("Elemento imagen de UI que representa la vida")] [SerializeField]
    Image hBar;

    /// <summary>
    /// Componente de <see cref="IHealth">vida</see> a monitorear
    /// </summary>
    IHealth health;

    [Header("Colores")] [Tooltip("Color salud de llena")] [SerializeField]
    private Color fullColor = Color.green;

    [Tooltip("Color salud de media")] [SerializeField]
    private Color halfColor = Color.yellow;

    [Tooltip("Color salud de baja")] [SerializeField]
    private Color emptyColor = Color.red;

    private void Start()
    {
        health = GetComponent<IHealth>();
        health.OnHealthChange.AddListener(UpdateHealthBar);
    }

    private void OnEnable()
    {
        InitBar();
    }

    /// <summary>
    /// Inicializar la barra de vida
    /// </summary>
    void InitBar()
    {
        hBar.fillAmount = 1;
        hBar.color = fullColor;
    }

    /// <summary>
    /// Actualizar el valor de la barra de vida
    /// </summary>
    void UpdateHealthBar()
    {
        float healthValue = (float)health.Health / health.MaxHealth;
        hBar.fillAmount = healthValue;
        switch (healthValue)
        {
            case > 0.5f:
                hBar.color = fullColor;
                break;
            case > 0.25f:
                hBar.color = halfColor;
                break;
            default:
                hBar.color = emptyColor;
                break;
        }
    }
}