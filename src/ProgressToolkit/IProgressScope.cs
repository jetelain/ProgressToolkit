namespace Pmad.ProgressToolkit
{
    public interface IProgressScope : IProgressBase
    {
        /// <summary>
        /// Cancellation token associated to root scope
        /// </summary>
        CancellationToken CancellationToken { get; }

        /// <summary>
        /// Create an <see langword="int" /> based progress. Reported value have to be between 0 and <paramref name="total"/>.
        /// </summary>
        /// <param name="name">Name of the progress item</param>
        /// <param name="total">Maximum value (to reach 100%)</param>
        /// <returns></returns>
        IProgressInteger CreateInteger(string name, int total);

        /// <summary>
        /// Create an <see langword="long" /> based progress. Reported value have to be between 0 and <paramref name="total"/>.
        /// </summary>
        /// <param name="name">Name of the progress item</param>
        /// <param name="total">Maximum value (to reach 100%)</param>
        /// <returns></returns>
        IProgressLong CreateLong(string name, long total);

        /// <summary>
        /// Create a progress that no detailed progress report. It will reach 100% when returned progress is disposed.
        /// </summary>
        /// <param name="name">Name of the progress item</param>
        /// <returns></returns>
        IProgressBase CreateSingle(string name);

        /// <summary>
        /// Create an <see langword="double" /> based progress. Reported value is progress in percent, and have to be between 0.0 and 100.0.
        /// </summary>
        /// <param name="name">Name of the progress item</param>
        /// <returns></returns>
        IProgressPercent CreatePercent(string name);

        IProgressScope CreateScope(string name, int estimatedChildrenCount = 0);
    }
}
