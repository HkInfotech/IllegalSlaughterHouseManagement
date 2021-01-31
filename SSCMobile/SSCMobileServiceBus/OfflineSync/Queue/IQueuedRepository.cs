using SQLite;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SSCMobileServiceBus.OfflineSync.Queue
{
    public interface IQueuedRepository<T> where T : ModelBase, new()
    {
        Task<List<T>> GetItemsAsync();
        Task<List<T>> GetItemsAsync2();
        Task<T> GetItemByIdAsync(int id);
        Task<List<T>> GetItemsByQueryAsync<TValue>(Expression<Func<T, bool>> predicate = null, Expression<Func<T, TValue>> orderBy = null);
        Task<T> GetItemByQueryAsync(Expression<Func<T, bool>> predicate);
        AsyncTableQuery<T> AsQueryable();
        Task<int> InsertAsync(T entity);
        Task<int> UpdateAsync(T entity);
        Task<int> UpsertAsync(T entity);
        Task<int> DeleteAsync(T entity);
        Task<int> InsertAllAsync(System.Collections.IEnumerable collection);
        Task<int> UpdateAllAsync(System.Collections.IEnumerable collection);
        Task<int> UpsertAllAsync(System.Collections.IEnumerable collection);
        Task<int> DeleteAllAsync(System.Collections.IEnumerable collection);
        Task<int> DropTable();

        // Extensions
        //Task InsertWithChildren(T entity);
        //Task UpdateWithChildren(T entity);
        //Task InsertOrReplaceWithChildren(T entity);
        //Task InsertAllWithChildren(System.Collections.IEnumerable collection);
        //Task UpdateAllWithChildren(System.Collections.IEnumerable collection);
        Task InsertOrReplaceAllWithChildren(System.Collections.IEnumerable collection);
        List<T> GetItemsWithChildren(Expression<Func<T, bool>> predicate = null);
        T GetItemWithChildrenById(int id);
        T GetItemWithChildrenByExternalId(int externalId);
    }
}
