namespace Pmad.ProgressTracking
{
    /// <summary>
    /// Numeric based progress, that can be incremented
    /// </summary>
    public interface IProgressIncrement : IProgressBase
    {
        /// <summary>
        /// Report one increment (compared to last reported value)
        /// </summary>
        void ReportOneDone();
    }
}
