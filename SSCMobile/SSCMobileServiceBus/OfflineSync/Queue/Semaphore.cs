using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace SSCMobileServiceBus.OfflineSync.Queue
{
    public class Semaphore
    {
        private readonly static Task _completed = Task.FromResult(true);
        private readonly Queue<TaskCompletionSource<bool>> _waiters = new Queue<TaskCompletionSource<bool>>();
        private int _currentCount;

        public Semaphore(int initialCount)
        {
            if (initialCount < 0) throw new ArgumentOutOfRangeException("initialCount");
            _currentCount = initialCount;
        }

        public Task WaitAsync()
        {
            lock (_waiters)
            {
                //Debug.WriteLine($"WaitAsync CurrentCount {_currentCount}");
                if (_currentCount > 0)
                {
                    --_currentCount;
                    return _completed;
                }
                else
                {
                    var waiter = new TaskCompletionSource<bool>();
                    _waiters.Enqueue(waiter);
                    return waiter.Task;
                }
            }
        }

        public void Release()
        {
            TaskCompletionSource<bool> toRelease = null;
            lock (_waiters)
            {
                Debug.WriteLine($"Release Queue count {_waiters.Count}");
                if (_waiters.Count > 0)
                    toRelease = _waiters.Dequeue();
                else
                    ++_currentCount;
            }

            toRelease?.SetResult(true);
        }
    }
}
