namespace System.Threading.Tasks
{
    using Runtime.CompilerServices;

    public static class TaskEx
    {
        public static Task<TResult> FromResult<TResult>(TResult result)
        {
            var source = new TaskCompletionSource<TResult>(result);
            source.TrySetResult(result);
            return source.Task;
        }

        public static Task Run(Action action)
        {
            return Run(action, CancellationToken.None);
        }

        public static Task Run(Func<Task> function)
        {
            return Run(function, CancellationToken.None);
        }

        public static Task<TResult> Run<TResult>(Func<TResult> function)
        {
            return Run(function, CancellationToken.None);
        }

        public static Task<TResult> Run<TResult>(Func<Task<TResult>> function)
        {
            return Run(function, CancellationToken.None);
        }

        public static Task Run(Action action, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(action, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
        }

        public static Task Run(Func<Task> function, CancellationToken cancellationToken)
        {
            return Run<Task>(function, cancellationToken).Unwrap();
        }

        public static Task<TResult> Run<TResult>(Func<TResult> function, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(function, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
        }

        public static Task<TResult> Run<TResult>(Func<Task<TResult>> function, CancellationToken cancellationToken)
        {
            return Run<Task<TResult>>(function, cancellationToken).Unwrap();
        }

        
        public static YieldAwaitable Yield()
        {
            return new YieldAwaitable();
        }
    }
}