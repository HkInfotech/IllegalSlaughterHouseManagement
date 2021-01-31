using SQLite;
using SQLiteNetExtensions.Attributes;
using SQLiteNetExtensions.Extensions;
using SSCMobileServiceBus.Platform_Specific_Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SSCMobileServiceBus.OfflineSync.Queue
{
    public class QueuedRepository<T> : IDisposable, IQueuedRepository<T> where T : ModelBase, new()
    {
        #region Properties
        public SQLiteAsyncConnection _dbManager;
        private readonly IBaseUrl _baseUrl;
        private static readonly AsyncLock AsyncLock = new AsyncLock();
        #endregion
        #region Constructor
        public QueuedRepository(IBaseUrl baseUrl)
        {
            _baseUrl = baseUrl;
            _dbManager = new SQLiteAsyncConnection(_baseUrl.GetDatabasePath(), storeDateTimeAsTicks: false);
            _dbManager?.CreateTableAsync<T>();
        }
        #endregion
        #region ReadOnly Methods
        public AsyncTableQuery<T> AsQueryable()
        {
            return _dbManager.Table<T>();
        }

        public async Task<T> GetItemByIdAsync(int id)
        {
            return await _dbManager.FindAsync<T>(id);
        }

        public async Task<T> GetItemByQueryAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbManager.FindAsync(predicate);
        }

        public async Task<List<T>> GetItemsAsync()
        {
            return await _dbManager.Table<T>().ToListAsync();
        }

        //public async Task<List<T>> GetItemsByIdAsync(int parentId)
        //{
        //    var list = await _dbManager.Table<T>().ToListAsync();
        //    return list.Where(x => x.Id == parentId).ToList();
        //}

        public async Task<List<T>> GetItemsByQueryAsync<TValue>(Expression<Func<T, bool>> predicate = null, Expression<Func<T, TValue>> orderBy = null)
        {
            var query = _dbManager.Table<T>();

            if (predicate != null)
                query = query.Where(predicate);

            if (orderBy != null)
                query = query.OrderBy(orderBy);

            return await query.ToListAsync();
        }

        public List<T> GetItemsWithChildren(Expression<Func<T, bool>> predicate = null)
        {
            using (SQLiteConnection _dbManager = new SQLiteConnection(_baseUrl.GetDatabasePath(), storeDateTimeAsTicks: false))
            {
                return _dbManager.GetAllWithChildren(predicate, true);
            }
        }

        public T GetItemWithChildrenByExternalId(int externalId)
        {
            using (SQLiteConnection _dbManager = new SQLiteConnection(_baseUrl.GetDatabasePath(), storeDateTimeAsTicks: false))
            {
                return _dbManager.GetAllWithChildren<T>(x => x.ExternalId == externalId).FirstOrDefault();
            }
        }

        public T GetItemWithChildrenById(int id)
        {
            using (SQLiteConnection _dbManager = new SQLiteConnection(_baseUrl.GetDatabasePath(), storeDateTimeAsTicks: false))
            {
                return _dbManager.GetWithChildren<T>(id, true);
            }
        }
        #endregion

        #region WriteOnly Methods
        public async Task<int> InsertAsync(T entity)
        {
            try
            {
                using (await AsyncLock.LockAsync())
                {
                    if (entity != null) //return await _dbManager.InsertAsync(entity);
                    {
                        await _dbManager.InsertAsync(entity);
                        var props = entity.GetType().GetProperties();

                        await UpsertChildrenRecursive(entity, props, true);

                        return 1;
                    }

                    return -1;
                }
            }
            catch (SQLiteException sqliteException)
            {
                Debug.WriteLine($"InsertAsync Exception, SQLite3 Status: {sqliteException.Result.ToString()}");
                if (sqliteException.Result == SQLite3.Result.Busy || sqliteException.Result == SQLite3.Result.Constraint)
                {
                    return await InsertAsync(entity);
                }
                throw;
            }
        }

        public async Task<int> InsertAllAsync(IEnumerable collection)
        {
            try
            {
                using (await AsyncLock.LockAsync())
                {
                    if (collection != null && ((IEnumerable<T>)collection).Any()) return await _dbManager.InsertAllAsync(collection);
                    return -1;
                }
            }
            catch (SQLiteException sqliteException)
            {
                Debug.WriteLine($"InsertAllAsync Exception, SQLite3 Status: {sqliteException.Result.ToString()}");
                if (sqliteException.Result == SQLite3.Result.Busy || sqliteException.Result == SQLite3.Result.Constraint)
                {
                    return await InsertAllAsync(collection);
                }
                throw;
            }
        }

        public async Task<int> UpdateAsync(T entity)
        {
            try
            {
                using (await AsyncLock.LockAsync())
                {
                    if (entity != null) //return await _dbManager.UpdateAsync(entity);
                    {
                        await _dbManager.UpdateAsync(entity);
                        var props = entity.GetType().GetProperties();

                        await UpsertChildrenRecursive(entity, props, false);

                        return 1;
                    }

                    return -1;
                }
            }
            catch (SQLiteException sqliteException)
            {
                Debug.WriteLine($"UpdateAsync Exception, SQLite3 Status: {sqliteException.Result.ToString()}");
                if (sqliteException.Result == SQLite3.Result.Busy || sqliteException.Result == SQLite3.Result.Constraint)
                {
                    return await UpdateAsync(entity);
                }
                throw;
            }
        }

        public async Task<int> UpdateAllAsync(IEnumerable collection)
        {
            try
            {
                using (await AsyncLock.LockAsync())
                {
                    if (collection != null && ((IEnumerable<T>)collection).Any()) return await _dbManager.UpdateAllAsync(collection);
                    return -1;
                }
            }
            catch (SQLiteException sqliteException)
            {
                Debug.WriteLine($"UpdateAllAsync Exception, SQLite3 Status: {sqliteException.Result.ToString()}");
                if (sqliteException.Result == SQLite3.Result.Busy || sqliteException.Result == SQLite3.Result.Constraint)
                {
                    return await UpdateAllAsync(collection);
                }
                throw;
            }
        }

        public async Task<int> UpsertAsync(T entity)
        {
            try
            {
                using (await AsyncLock.LockAsync())
                {
                    if (entity != null) return entity.Id == 0 ? await _dbManager.InsertAsync(entity) : await _dbManager.UpdateAsync(entity);
                    return -1;
                }
            }
            catch (SQLiteException sqliteException)
            {
                Debug.WriteLine($"UpsertAsync Exception, SQLite3 Status: {sqliteException.Result.ToString()}");
                if (sqliteException.Result == SQLite3.Result.Busy || sqliteException.Result == SQLite3.Result.Constraint)
                {
                    return await UpdateAsync(entity);
                }
                throw;
            }
        }

        public async Task<int> UpsertAllAsync(IEnumerable collection)
        {
            if (collection != null && ((IEnumerable<T>)collection).Any())
            {
                foreach (T item in collection)
                {
                    if (item.Id == 0)
                    {
                        await InsertAsync(item);
                    }
                    else
                    {
                        await UpdateAsync(item);
                    }
                }

                return ((IEnumerable<T>)collection).Count();
            }

            Debug.WriteLine($"UpsertAllAsync number of items: {((IEnumerable<T>)collection).Count()}");
            return -1;
        }

        public async Task<int> DeleteAsync(T entity)
        {
            try
            {
                using (await AsyncLock.LockAsync())
                {
                    if (entity != null) return await _dbManager.DeleteAsync(entity);
                    return -1;
                }
            }
            catch (SQLiteException sqliteException)
            {
                Debug.WriteLine($"DeleteAsync Exception, SQLite3 Status: {sqliteException.Result.ToString()}");
                if (sqliteException.Result == SQLite3.Result.Busy || sqliteException.Result == SQLite3.Result.Constraint)
                {
                    return await DeleteAsync(entity);
                }
                throw;
            }
        }

        public async Task<int> DeleteAllAsync(IEnumerable collection)
        {
            if (collection != null && ((IEnumerable<T>)collection).Any())
            {
                foreach (T item in collection)
                {
                    await DeleteAsync(item);
                }

                return ((IEnumerable<T>)collection).Count();
            }

            Debug.WriteLine($"DeleteAllAsync Exception");
            return -1;
        }

        public async Task InsertWithChildren(T entity)
        {
            using (SQLiteConnection _dbManager = new SQLiteConnection(_baseUrl.GetDatabasePath(), storeDateTimeAsTicks: false))
            {
                try
                {
                    using (await AsyncLock.LockAsync())
                    {
                        if (entity != null) _dbManager.InsertWithChildren(entity, true);
                    }
                }
                catch (SQLiteException sqliteException)
                {
                    if (sqliteException.Result == SQLite3.Result.Busy || sqliteException.Result == SQLite3.Result.Constraint)
                    {
                        await InsertWithChildren(entity);
                    }
                    throw;
                }
            }
        }

        public async Task InsertOrReplaceWithChildren(T entity)
        {
            using (SQLiteConnection _dbManager = new SQLiteConnection(_baseUrl.GetDatabasePath(), storeDateTimeAsTicks: false))
            {
                try
                {
                    using (await AsyncLock.LockAsync())
                    {
                        if (entity != null) _dbManager.InsertOrReplaceWithChildren(entity, true);
                    }
                }
                catch (SQLiteException sqliteException)
                {
                    Debug.WriteLine($"InsertOrReplaceWithChildren Exception, SQLite3 Status: {sqliteException.Result.ToString()}");
                    if (sqliteException.Result == SQLite3.Result.Busy || sqliteException.Result == SQLite3.Result.Constraint)
                    {
                        await InsertOrReplaceWithChildren(entity);
                    }
                    throw;
                }
            }
        }

        public async Task UpdateWithChildren(T entity)
        {
            using (SQLiteConnection _dbManager = new SQLiteConnection(_baseUrl.GetDatabasePath(), storeDateTimeAsTicks: false))
            {
                try
                {
                    using (await AsyncLock.LockAsync())
                    {
                        if (entity != null) _dbManager.UpdateWithChildren(entity);
                    }
                }
                catch (SQLiteException sqliteException)
                {
                    if (sqliteException.Result == SQLite3.Result.Busy || sqliteException.Result == SQLite3.Result.Constraint)
                    {
                        await UpdateWithChildren(entity);
                    }
                    throw;
                }
            }
        }

        public async Task InsertAllWithChildren(IEnumerable collection)
        {
            using (SQLiteConnection _dbManager = new SQLiteConnection(_baseUrl.GetDatabasePath(), storeDateTimeAsTicks: false))
            {
                try
                {
                    using (await AsyncLock.LockAsync())
                    {
                        if (collection != null && ((IEnumerable<T>)collection).Any()) _dbManager.InsertAllWithChildren(collection);
                    }
                }
                catch (SQLiteException sqliteException)
                {
                    if (sqliteException.Result == SQLite3.Result.Busy || sqliteException.Result == SQLite3.Result.Constraint)
                    {
                        await InsertAllWithChildren(collection);
                    }
                    throw;
                }
            }
        }

        public async Task InsertOrReplaceAllWithChildren(IEnumerable collection)
        {
            using (SQLiteConnection _dbManager = new SQLiteConnection(_baseUrl.GetDatabasePath(), storeDateTimeAsTicks: false))
            {
                try
                {
                    using (await AsyncLock.LockAsync())
                    {
                        if (collection != null && ((IEnumerable<T>)collection).Any()) _dbManager.InsertOrReplaceAllWithChildren(collection);
                    }
                }
                catch (SQLiteException sqliteException)
                {
                    Debug.WriteLine($"InsertOrReplaceAllWithChildren Exception, SQLite3 Status: {sqliteException.Result.ToString()}");
                    if (sqliteException.Result == SQLite3.Result.Busy || sqliteException.Result == SQLite3.Result.Constraint)
                    {
                        await InsertOrReplaceAllWithChildren(collection);
                    }
                    throw;
                }
            }
        }

        public async Task UpdateAllWithChildren(IEnumerable collection)
        {
            throw new NotImplementedException();
        }

        public async Task<int> DropTable()
        {
            return await _dbManager.DropTableAsync<T>();
        }

        #endregion
        public void Dispose()
        {
            _dbManager.CloseAsync();
        }
        #region Recursive methods
        private async Task UpsertChildrenRecursive(object entity, PropertyInfo[] properties, bool isInsert)
        {
            int id = (int)entity.GetType().GetProperty("Id").GetValue(entity);

            foreach (var prop in properties)
            {
                object value = prop.GetValue(entity);
                if (value != null && prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    var list = (IList)value;
                    foreach (var childItem in list)
                    {
                        var childProps = childItem.GetType().GetProperties();
                        foreach (var childProp in childProps)
                        {
                            var targetPropertyName = childProp.GetAttribute<ForeignKeyAttribute>();
                            if (targetPropertyName != null)
                            {
                                childItem.GetType().GetProperty(targetPropertyName.Name).SetValue(childItem, id);
                            }
                        }

                        if (isInsert)
                            await _dbManager.InsertAsync(childItem);
                        else
                            await _dbManager.UpdateAsync(childItem);

                        if (childProps.Any(x => x.PropertyType.IsGenericType && x.PropertyType.GetGenericTypeDefinition() == typeof(List<>)))
                        {
                            await UpsertChildrenRecursive(childItem, childProps, isInsert);
                        }
                    }
                }
            }
        }

        public async Task<List<T>> GetItemsAsync2()
        {
            var list = await _dbManager.Table<T>().ToListAsync();

            for (int i = 0; i < list.Count; i++)
            {
                var props = list[i].GetType().GetProperties();
                list[i] = (T)await GetChildrenRecursive(list[i], props);
            }

            return list;
        }

        private async Task<object> GetChildrenRecursive(object entity, PropertyInfo[] properties)
        {
            try
            {
                int id = (int)entity.GetType().GetProperty("Id").GetValue(entity);

                var hasLists = properties.Any(x => x.PropertyType.IsGenericType && x.PropertyType.GetGenericTypeDefinition() == typeof(List<>));
                if (hasLists)
                {
                    foreach (var prop in properties)
                    {
                        object value = prop.GetValue(entity);
                        if (value == null && prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                        {
                            Type objectType = prop.PropertyType.GenericTypeArguments.FirstOrDefault();
                            Type queuedRepository = typeof(QueuedRepository<>).MakeGenericType(new Type[] { objectType });
                            ConstructorInfo repoCtor = queuedRepository.GetConstructor(new Type[] { _baseUrl.GetType() });
                            object repoInstance = repoCtor.Invoke(new object[] { _baseUrl });
                            MethodInfo getItemsAsync = queuedRepository.GetMethod("GetItemsAsync");
                            Task task = (Task)getItemsAsync.Invoke(repoInstance, null);

                            await task.ConfigureAwait(false);

                            if (task != null)
                            {
                                var childProps = objectType.GetProperties();
                                var resultProperty = task.GetType().GetProperty("Result");
                                var unfilteredList = (IList)resultProperty.GetValue(task);

                                var fkName = (from property in childProps
                                              where property.GetCustomAttributes(typeof(ForeignKeyAttribute), false).Length > 0
                                              select property.Name).FirstOrDefault();

                                var filteredList = (from property in unfilteredList.Cast<object>()
                                                    where (int)property.GetType().GetProperty(fkName).GetValue(property) == id
                                                    select property);

                                Type listType = typeof(List<>).MakeGenericType(objectType);
                                var newList = (IList)Activator.CreateInstance(listType);

                                foreach (var item in filteredList)
                                {
                                    newList.Add(item);
                                }

                                entity.GetType().GetProperty(prop.Name).SetValue(entity, newList);

                                var hasChildItems = childProps.Any(x => x.PropertyType.IsGenericType && x.PropertyType.GetGenericTypeDefinition() == typeof(List<>));
                                if (hasChildItems)
                                {
                                    for (int i = 0; i < newList.Count; i++)
                                    {
                                        var props = newList[i].GetType().GetProperties();
                                        newList[i] = await GetChildrenRecursive(newList[i], props);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return entity;
        }
        #endregion
    }
}
