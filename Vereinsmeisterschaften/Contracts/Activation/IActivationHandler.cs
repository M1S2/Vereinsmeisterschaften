namespace Vereinsmeisterschaften.Contracts.Activation;

/// <summary>
/// Interface for activation handlers
/// </summary>
public interface IActivationHandler
{
    /// <summary>
    /// Can this handler handle the activation?
    /// </summary>
    /// <returns><see cref="bool"/></returns>
    bool CanHandle();

    /// <summary>
    /// Handle the activation
    /// </summary>
    /// <returns><see cref="Task"/></returns>
    Task HandleAsync();
}
