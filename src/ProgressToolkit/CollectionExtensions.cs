using System.Collections.Concurrent;

namespace ProgressToolkit
{
    public static class CollectionExtensions
    {
        public static IEnumerable<T> WithProgress<T>(this IEnumerable<T> input, IProgressScope scope, string name)
        {
            return WithProgress(input.ToList(), scope, name);
        }

        public static IEnumerable<T> WithProgress<T>(this List<T> input, IProgressScope scope, string name)
        {
            return ProgressEnumerator(input, scope.CreateInteger(name, input.Count));
        }

        public static IEnumerable<T> WithProgress<T>(this IReadOnlyCollection<T> input, IProgressScope scope, string name)
        {
            return ProgressEnumerator(input, scope.CreateInteger(name, input.Count));
        }

        private static IEnumerable<T> ProgressEnumerator<T>(IEnumerable<T> input, IProgressInteger report)
        {
            using (report)
            {
                foreach (var item in input)
                {
                    yield return item;
                    report.ReportOneDone();
                }
            }
        }

        public static IEnumerable<T> WithProgressCancellable<T>(this IEnumerable<T> input, IProgressScope scope, string name)
        {
            return WithProgressCancellable(input.ToList(), scope, name);
        }

        public static IEnumerable<T> WithProgressCancellable<T>(this List<T> input, IProgressScope scope, string name)
        {
            return ProgressEnumeratorCancellable(input, scope.CreateInteger(name, input.Count), scope.CancellationToken);
        }

        public static IEnumerable<T> WithProgressCancellable<T>(this IReadOnlyCollection<T> input, IProgressScope scope, string name)
        {
            return ProgressEnumeratorCancellable(input, scope.CreateInteger(name, input.Count), scope.CancellationToken);
        }

        private static IEnumerable<T> ProgressEnumeratorCancellable<T>(this IEnumerable<T> input, IProgressInteger report, CancellationToken cancellationToken)
        {
            using (report)
            {
                foreach (var item in input)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        yield break;
                    }
                    yield return item;
                    report.ReportOneDone();
                }
            }
        }

        public static IReadOnlyCollection<TTarget> SelectParallelManyWithProgress<TSource,TTarget>(this IReadOnlyCollection<TSource> source, IProgressScope scope, string name, Func<TSource,IEnumerable<TTarget>> transform)
        {
            var result = new ConcurrentQueue<TTarget>();
            using (var report = scope.CreateInteger(name, source.Count))
            {
                Parallel.ForEach(source, sourceItem =>
                {
                    foreach (var resultItem in transform(sourceItem))
                    {
                        result.Enqueue(resultItem);
                    }
                    report.ReportOneDone();
                });
            }
            return result;
        }

        public static IReadOnlyCollection<TTarget> SelectParallelWithProgress<TSource, TTarget>(this IReadOnlyCollection<TSource> source, IProgressScope scope, string name, Func<TSource, TTarget> transform)
        {
            var result = new ConcurrentQueue<TTarget>();
            using (var report = scope.CreateInteger(name, source.Count))
            {
                Parallel.ForEach(source, sourceItem =>
                {
                    result.Enqueue(transform(sourceItem));
                    report.ReportOneDone();
                });
            }
            return result;
        }
    }
}
