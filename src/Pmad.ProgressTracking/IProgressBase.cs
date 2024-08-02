namespace Pmad.ProgressTracking
{
    /// <summary>
    /// Base of progress items
    /// 
    /// Method <see cref="IProgress{string}.Report(string)" /> is used to report a text status.
    /// </remarks>
    public interface IProgressBase : IProgress<string>, IDisposable
    {
        /// <summary>
        /// Write a log line associated with current progress item
        /// </summary>
        /// <param name="message"></param>
        void WriteLine(string message);

        /// <summary>
        /// Set the progress as failed with the specified exception.
        /// </summary>
        /// <param name="ex">Reason of the failure</param>
        void Failed(Exception ex);
    }
}