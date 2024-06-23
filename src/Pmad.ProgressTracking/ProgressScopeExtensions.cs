namespace Pmad.ProgressTracking
{
    public static class ProgressScopeExtensions
    {
        public static async Task<T> Track<T>(this IProgressScope scope, string name, Func<Task<T>> taskFactory)
        {
            using var report = scope.CreateSingle(name);
            var result = await taskFactory().ConfigureAwait(false);
            return result;
        }

        public static async Task<T> Track<T>(this IProgressScope scope, string name, Task<T> task)
        {
            using var report = scope.CreateSingle(name);
            var result = await task.ConfigureAwait(false);
            return result;
        }

        public static T TrackPercent<T>(this IProgressScope scope, string name, Func<IProgressPercent,T> action)
        {
            using var report = scope.CreatePercent(name);
            var result = action(report);
            return result;
        }

        public static async Task<T> TrackPercent<T>(this IProgressScope scope, string name, Func<IProgressPercent, Task<T>> action)
        {
            using var report = scope.CreatePercent(name);
            var result = await action(report).ConfigureAwait(false);
            return result;
        }
    }
}
