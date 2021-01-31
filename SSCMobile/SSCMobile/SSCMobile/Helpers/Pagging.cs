using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SSCMobile.Helpers
{
    public interface IInfiniteListLoader
    {
        void OnScroll(int lastVisibleIndex);
    }

    public struct PageResult<TItem>
    {
        public static readonly PageResult<TItem> Empty = new PageResult<TItem>(0, new List<TItem>());

        public PageResult(int totalCount, IReadOnlyList<TItem> items)
        {
            TotalCount = totalCount;
            Items = items ?? new List<TItem>();
        }

        public int TotalCount { get; }

        public IReadOnlyList<TItem> Items { get; }

        public static bool operator ==(PageResult<TItem> p1, PageResult<TItem> p2)
        {
            return ReferenceEquals(p1.Items, p2.Items) && p1.TotalCount == p2.TotalCount;
        }

        public static bool operator !=(PageResult<TItem> p1, PageResult<TItem> p2)
        {
            return !ReferenceEquals(p1.Items, p2.Items) || p1.TotalCount != p2.TotalCount;
        }

        public bool Equals(PageResult<TItem> other)
        {
            return TotalCount == other.TotalCount && Equals(Items, other.Items);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is PageResult<TItem> result && Equals(result);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (TotalCount * 397) ^ (Items != null ? Items.GetHashCode() : 0);
            }
        }
    }


    public class Paginator<TResult> : IInfiniteListLoader, IDisposable
    {
        private const float LoadingThresholdDefault = 1f / 4;
        private const int PageSizeDefault = 10;
        private const int MaxItemCountDefault = 20000;

        private readonly object _syncRoot = new object();
        private readonly int _maxItemCount;
        private readonly Func<int, int, Task<PageResult<TResult>>> _pageSourceLoader;
        private readonly Action<Task> _onTaskCompleted;
        private readonly float _loadingThreshold;

        private bool _isDisposed;
        private List<TResult> _items;
        private bool _refreshRequested;
        private CancellationTokenSource _loadingTaskTokenSource;

        public Paginator(Func<int, int, Task<PageResult<TResult>>> pageSourceLoader, Action<Task> onTaskCompleted = null, int pageSize = PageSizeDefault, int maxItemCount = MaxItemCountDefault, float loadingThreshold = LoadingThresholdDefault)
        {
            _maxItemCount = maxItemCount;
            _pageSourceLoader = pageSourceLoader;
            _onTaskCompleted = onTaskCompleted;
            _loadingThreshold = loadingThreshold;

            PageSize = pageSize;
            TotalCount = _maxItemCount;

            Reset();
        }

        public Task<PageResult<TResult>> LoadingTask { get; private set; }

        public int PageLoadedCount { get; private set; }

        public int LoadedCount => Items.Count;

        public bool IsFull => LoadedCount >= TotalCount;

        public int PageSize { get; }

        public int TotalCount { get; private set; }

        public int TotalRemoteCount { get; private set; }

        public bool HasStarted => LoadingTask != null;

        public bool IsLoadingSuccessfull => LoadingTask.Status == TaskStatus.RanToCompletion;

        public bool HasRefreshed
        {
            get
            {
                lock (_syncRoot)
                {
                    return _refreshRequested;
                }
            }
        }

        public IReadOnlyList<TResult> Items => _items;

        public PageResult<TResult> LastResult { get; private set; }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;
        }

        public async void OnScroll(int lastVisibleIndex)
        {
            try
            {
                if (await ShouldLoadNextPage(lastVisibleIndex))
                {
                    int pageToLoad = lastVisibleIndex / PageSize + 2;
                    await LoadPage(pageToLoad, calledFromScroll: true);
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine($"Error in OnScroll task: {exception}");
            }
        }

        public Task<PageResult<TResult>> LoadPage(int pageNumber, bool calledFromScroll = false)
        {
            try
            {
                lock (_syncRoot)
                {
                    if (calledFromScroll)
                    {
                        if (pageNumber <= PageLoadedCount)
                        {
                            return Task.FromResult(PageResult<TResult>.Empty);
                        }
                    }

                    if (pageNumber > PageLoadedCount && IsFull)
                    {
                        return Task.FromResult(PageResult<TResult>.Empty);
                    }

                    if (pageNumber == 1 && PageLoadedCount > 0)
                    {
                        _refreshRequested = true;
                    }
                    else
                    {
                        _refreshRequested = false;
                    }

                    if (LoadingTask != null && !LoadingTask.IsCompleted)
                    {
                        // Cancels callbacks of previous task if not completed
                        _loadingTaskTokenSource.Cancel();
                    }

                    _loadingTaskTokenSource = new CancellationTokenSource();
                    LoadingTask = _pageSourceLoader(pageNumber, PageSize);

                    MonitorLoadingTask(_loadingTaskTokenSource.Token);
                }
            }
            catch (Exception ex)
            {

            }
            return LoadingTask;
        }

        private void MonitorLoadingTask(CancellationToken cancellationToken)
        {
            try
            {

                Task.Run(
                        async () =>
                        {
                            try
                            {
                                await LoadingTask;
                                if (cancellationToken.IsCancellationRequested)
                                {
                                    return;
                                }

                                OnPageRetrieved(LoadingTask);
                            }
                            catch (TaskCanceledException canceledException)
                            {
                                Debug.WriteLine($"Task has been canceled {canceledException}");
                            }
                            catch (Exception exception)
                            {
                                Debug.WriteLine($"Error in wrapped task {exception}");
                            }
                        })
                    .ContinueWith(task => OnTaskCompleted(LoadingTask), TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch (Exception ex)
            { }
        }

        private void OnPageRetrieved(Task<PageResult<TResult>> task)
        {
            if (_isDisposed)
            {
                return;
            }

            var result = task.Result;
            lock (_syncRoot)
            {
                LastResult = result;

                if (_refreshRequested)
                {
                    Reset();
                }

                TotalRemoteCount = result.TotalCount;
                TotalCount = Math.Min(result.TotalCount, _maxItemCount);
                PageLoadedCount++;
            }

            _items.AddRange(result.Items);
        }

        private void OnTaskCompleted(Task task)
        {
            if (_isDisposed)
            {
                return;
            }

            _onTaskCompleted?.Invoke(task);
        }

        private void Reset()
        {
            PageLoadedCount = 0;
            _items = new List<TResult>();
        }

        private Task<bool> ShouldLoadNextPage(int lastVisibleIndex)
        {
            return Task.Run(() =>
            {
                if (lastVisibleIndex < 0)
                {
                    return false;
                }

                if (PageLoadedCount == 0)
                {
                    // If no pages are loaded, there is nothing to scroll
                    return false;
                }

                if (IsFull)
                {
                    // All messages are already loaded nothing to paginate
                    return false;
                }

                if (HasStarted && !LoadingTask.IsCompleted)
                {
                    // Currently loading page
                    return false;
                }

                int itemsCount = LoadedCount;
                return lastVisibleIndex >= (itemsCount - (PageSize * _loadingThreshold));
            });
        }
    }
}
