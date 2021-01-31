using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SSCMobileServiceBus.OfflineSync.Queue
{
    public class AsyncLock
    {
        private readonly Semaphore _semaphore;
        private readonly Task<Releaser> _releaser;

        public AsyncLock()
        {
            _semaphore = new Semaphore(1);
            _releaser = Task.FromResult(new Releaser(this));
        }

        public Task<Releaser> LockAsync()
        {
            //Debug.WriteLine($"LockAsync started");
            var wait = _semaphore.WaitAsync();
            return wait.IsCompleted
                ? _releaser
                : wait.ContinueWith((_, state) => new Releaser((AsyncLock)state), this, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
        }

        public struct Releaser : IDisposable
        {
            private readonly AsyncLock _toRelease;

            internal Releaser(AsyncLock toRelease) { _toRelease = toRelease; }

            public void Dispose()
            {
                _toRelease?._semaphore.Release();
            }
        }
    }
}
