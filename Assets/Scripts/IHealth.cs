using UnityEngine.Events;

/// <summary>
/// Interfaz para controlar la vida
/// </summary>
public interface IHealth
{
    /// <summary>
    /// Entero medidor de la cantidad de vida
    /// </summary>
    int Health
    {
        get;
    }

    /// <summary>
    /// Valor máximo de la vida
    /// </summary>
    int MaxHealth
    {
        get;
    }

    UnityEvent OnHealthChange { get; }

    /// <summary>
    /// Configurar la vida máxima
    /// </summary>
    /// <param name="amount">Valor de la vida máxima</param>
    void SetMaxHealth(int amount);

    /// <summary>
    /// Restar la cantidad de vida pasada
    /// </summary>
    /// <param name="amount">Cantidad de vida a restar</param>
    void TakeDamage(int amount);

    /// <summary>
    /// Reiniciar la vida al valor máximo
    /// </summary>
    void RestoreHealth();

    /// <summary>
    /// Controlar la muerte en vida 0
    /// </summary>
    void HandleDeath();
}
