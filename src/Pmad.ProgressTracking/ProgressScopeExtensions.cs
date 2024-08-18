namespace Pmad.ProgressTracking
{
    public static class ProgressScopeExtensions
    {
        public static void Track(this IProgressScope scope, string name, Action action)
        {
            using var report = scope.CreateSingle(name);
            try
            {
                action();
            }
            catch (Exception ex)
            {
                report.Failed(ex);
                throw;
            }
        }

        public static async Task Track(this IProgressScope scope, string name, Func<Task> taskFactory)
        {
            using var report = scope.CreateSingle(name);
            try
            {
                await taskFactory().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                report.Failed(ex);
                throw;
            }
        }

        public static T Track<T>(this IProgressScope scope, string name, Func<T> action)
        {
            using var report = scope.CreateSingle(name);
            try
            {
                return action();
            }
            catch (Exception ex)
            {
                report.Failed(ex);
                throw;
            }
        }

        public static async Task<T> Track<T>(this IProgressScope scope, string name, Func<Task<T>> taskFactory)
        {
            using var report = scope.CreateSingle(name);
            try
            {
                return await taskFactory().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                report.Failed(ex);
                throw;
            }
        }

        public static async Task<T> Track<T>(this IProgressScope scope, string name, Task<T> task)
        {
            using var report = scope.CreateSingle(name);
            try
            {
                return await task.ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                report.Failed(ex);
                throw;
            }
        }

        public static T TrackPercent<T>(this IProgressScope scope, string name, Func<IProgressPercent, T> action)
        {
            using var report = scope.CreatePercent(name);
            try
            {
                return action(report);
            }
            catch (Exception ex)
            {
                report.Failed(ex);
                throw;
            }
        }

        public static async Task<T> TrackPercent<T>(this IProgressScope scope, string name, Func<IProgressPercent, Task<T>> action)
        {
            using var report = scope.CreatePercent(name);
            try
            {
                return await action(report).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                report.Failed(ex);
                throw;
            }
        }
    }
}
