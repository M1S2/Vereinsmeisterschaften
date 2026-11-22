using Vereinsmeisterschaften.Core.Contracts;

namespace Vereinsmeisterschaften.Core.Helpers
{
    /// <summary>
    /// Class that is extending the functionality of a default <see cref="CancellationTokenSource"/> by adding a dismiss flag
    /// </summary>
    public class CancellationTokenSourceDismissable : CancellationTokenSource, IDisposable, ICancellationInfo
    {
        /// <summary>
        /// Indicating if the results of an operation should be dismissed when canceled.
        /// </summary>
        public bool Dismiss { get; private set; }

        /// <summary>
        /// Cancel including the option to set the <see cref="Dismiss"/> flag.
        /// </summary>
        /// <param name="dismiss">Indicating if the results of an operation should be dismissed when canceled.</param>
        public new void Cancel(bool dismiss = false)
        {
            Dismiss = dismiss;
            base.Cancel();
        }
        /// <summary>
        /// Cancel without dismiss.
        /// </summary>
        public new void Cancel() => Cancel(false);

        /// <summary>
        /// Cancel after a delay including the option to set the <see cref="Dismiss"/> flag.
        /// </summary>
        /// <param name="millisecondsDelay">The time span to wait before canceling this <see cref="CancellationTokenSource"/>.</param>
        /// <param name="dismiss">Indicating if the results of an operation should be dismissed when canceled.</param>
        public void CancelAfter(int millisecondsDelay, bool dismiss = false)
        {
            Dismiss = dismiss;
            base.CancelAfter(millisecondsDelay);
        }
        /// <summary>
        /// Cancel after a delay without dismiss.
        /// </summary>
        /// <param name="millisecondsDelay">The time span to wait before canceling this <see cref="CancellationTokenSource"/>.</param>
        public new void CancelAfter(int millisecondsDelay) => CancelAfter(millisecondsDelay, false);
    }
}
