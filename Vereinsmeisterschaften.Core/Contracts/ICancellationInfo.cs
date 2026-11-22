using Vereinsmeisterschaften.Core.Helpers;

namespace Vereinsmeisterschaften.Core.Contracts
{
    /// <summary>
    /// Interface used by the <see cref="CancellationTokenSourceDismissable"/> to only make the dismiss info accessible
    /// </summary>
    public interface ICancellationInfo
    {
        /// <summary>
        /// Indicating if the results of an operation should be dismissed when canceled.
        /// </summary>
        bool Dismiss { get; }
    }
}
