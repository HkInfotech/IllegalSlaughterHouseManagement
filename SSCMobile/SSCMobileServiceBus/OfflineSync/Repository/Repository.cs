using SQLite;
using SQLiteNetExtensions.Extensions;
using SSCMobileServiceBus.Platform_Specific_Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SSCMobileServiceBus.OfflineSync.Repository
{
    public class Repository<T> : IRepository<T> where T : ModelBase, new()
    {
        private IBaseUrl _baseUrl;


        private const SQLite.SQLiteOpenFlags ReadOnlyFlags = SQLiteOpenFlags.ReadOnly | SQLiteOpenFlags.SharedCache;
        private const SQLiteOpenFlags WriteOnlyFlags = SQLite.SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.SharedCache;
        #region Constructor
        public Repository(IBaseUrl baseUrl)
        {
            _baseUrl = baseUrl;


            using (SQLiteConnection _dbManager = new SQLiteConnection(_baseUrl.GetDatabasePath(), WriteOnlyFlags, storeDateTimeAsTicks: false))
            {


                _dbManager.CreateTable<T>();

            }
        }
        #endregion
        #region Read Only Connection
        public TableQuery<T> AsQueryable()
        {
            using (SQLiteConnection _dbManager = new SQLiteConnection(_baseUrl.GetDatabasePath(), ReadOnlyFlags, storeDateTimeAsTicks: false))
            {
                return _dbManager.Table<T>();
            }
        }

        public List<T> GetItems()
        {
            using (SQLiteConnection _dbManager = new SQLiteConnection(_baseUrl.GetDatabasePath(), ReadOnlyFlags, storeDateTimeAsTicks: false))
            {
                return _dbManager.Table<T>().ToList();
            }
        }

        public List<T> GetItemsByQuery<TValue>(Expression<Func<T, bool>> predicate = null, Expression<Func<T, TValue>> orderBy = null)
        {
            TableQuery<T> query = null;

            using (SQLiteConnection _dbManager = new SQLiteConnection(_baseUrl.GetDatabasePath(), ReadOnlyFlags, storeDateTimeAsTicks: false))
            {
                query = _dbManager.Table<T>();

                if (predicate != null)
                    query = query.Where(predicate);

                if (orderBy != null)
                    query = query.OrderBy(orderBy);

                return query.ToList();
            }


        }

        public T GetItemById(int id)
        {
            using (SQLiteConnection _dbManager = new SQLiteConnection(_baseUrl.GetDatabasePath(), ReadOnlyFlags, storeDateTimeAsTicks: false))
            {
                return _dbManager.Find<T>(id);
            }
        }

        public T GetItemByQuery(Expression<Func<T, bool>> predicate)
        {
            using (SQLiteConnection _dbManager = new SQLiteConnection(_baseUrl.GetDatabasePath(), ReadOnlyFlags, storeDateTimeAsTicks: false))
            {
                return _dbManager.Find(predicate);
            }
        }
        public List<T> GetItemsWithChildren(Expression<Func<T, bool>> predicate = null)
        {
            using (SQLiteConnection _dbManager = new SQLiteConnection(_baseUrl.GetDatabasePath(), ReadOnlyFlags, storeDateTimeAsTicks: false))
            {
                return _dbManager.GetAllWithChildren(predicate, true);
            }
        }

        public T GetItemWithChildrenById(int id)
        {
            using (SQLiteConnection _dbManager = new SQLiteConnection(_baseUrl.GetDatabasePath(), ReadOnlyFlags, storeDateTimeAsTicks: false))
            {
                return _dbManager.GetWithChildren<T>(id, true);
            }
        }

        public T GetItemWithChildrenByExternalId(int externalId)
        {
            using (SQLiteConnection _dbManager = new SQLiteConnection(_baseUrl.GetDatabasePath(), ReadOnlyFlags, storeDateTimeAsTicks: false))
            {
                return _dbManager.GetAllWithChildren<T>(x => x.ExternalId == externalId).FirstOrDefault();
            }
        }
        #endregion Read Only Connection

        #region Write Only Connection
        public int Insert(T entity)
        {
            //using (SQLiteConnection _dbManager = new SQLiteConnection(_baseUrl.GetDatabasePath(), WriteOnlyFlags))
            using (SQLiteConnectionWithLock _dbManager = new SQLiteConnectionWithLock(new SQLiteConnectionString(_baseUrl.GetDatabasePath(), false, null), SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.SharedCache))
            {
                using (_dbManager.Lock())
                {
                    try
                    {
                        return _dbManager.Insert(entity);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"SQLiteError: {ex.Message}");
                        return -1;
                    }
                }
            }
        }

        public int Update(T entity)
        {
            //using (SQLiteConnection _dbManager = new SQLiteConnection(_baseUrl.GetDatabasePath(), WriteOnlyFlags))
            using (SQLiteConnectionWithLock _dbManager = new SQLiteConnectionWithLock(new SQLiteConnectionString(_baseUrl.GetDatabasePath(), false, null), SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.SharedCache))
            {
                using (_dbManager.Lock())
                {
                    try
                    {
                        return _dbManager.Update(entity);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"SQLiteError: {ex.Message}");
                        return -1;
                    }
                }
            }
        }

        public int Upsert(T entity)
        {
            //using (SQLiteConnection _dbManager = new SQLiteConnection(_baseUrl.GetDatabasePath(), WriteOnlyFlags))
            using (SQLiteConnectionWithLock _dbManager = new SQLiteConnectionWithLock(new SQLiteConnectionString(_baseUrl.GetDatabasePath(), false, null), SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.SharedCache))
            {
                using (_dbManager.Lock())
                {
                    try
                    {
                        return entity.Id == 0 ? _dbManager.Insert(entity) : _dbManager.Update(entity);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"SQLiteError: {ex.Message}");
                        return -1;
                    }
                }
            }
        }

        public int Delete(T entity)
        {
            //using (SQLiteConnection _dbManager = new SQLiteConnection(_baseUrl.GetDatabasePath(), WriteOnlyFlags))
            using (SQLiteConnectionWithLock _dbManager = new SQLiteConnectionWithLock(new SQLiteConnectionString(_baseUrl.GetDatabasePath(), false, null), SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.SharedCache))
            {
                using (_dbManager.Lock())
                {
                    try
                    {
                        return _dbManager.Delete(entity);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"SQLiteError: {ex.Message}");
                        return -1;
                    }
                }
            }
        }

        public int InsertAll(System.Collections.IEnumerable collection)
        {
            //using (SQLiteConnection _dbManager = new SQLiteConnection(_baseUrl.GetDatabasePath(), WriteOnlyFlags))
            using (SQLiteConnectionWithLock _dbManager = new SQLiteConnectionWithLock(new SQLiteConnectionString(_baseUrl.GetDatabasePath(), false, null), SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.SharedCache))
            {
                using (_dbManager.Lock())
                {
                    try
                    {
                        return _dbManager.InsertAll(collection);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"SQLiteError: {ex.Message}");
                        return -1;
                    }
                }
            }
        }

        public int UpdateAll(System.Collections.IEnumerable collection)
        {
            //using (SQLiteConnection _dbManager = new SQLiteConnection(_baseUrl.GetDatabasePath(), WriteOnlyFlags))
            using (SQLiteConnectionWithLock _dbManager = new SQLiteConnectionWithLock(new SQLiteConnectionString(_baseUrl.GetDatabasePath(), false, null), SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.SharedCache))
            {
                using (_dbManager.Lock())
                {
                    try
                    {
                        return _dbManager.UpdateAll(collection);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"SQLiteError: {ex.Message}");
                        return -1;
                    }
                }
            }
        }
        public void UpsertAll(System.Collections.IEnumerable collection)
        {
            foreach (T item in collection)
            {
                if (item.Id == 0)
                {
                    //using (SQLiteConnection _dbManager = new SQLiteConnection(_baseUrl.GetDatabasePath(), WriteOnlyFlags))
                    using (SQLiteConnectionWithLock _dbManager = new SQLiteConnectionWithLock(new SQLiteConnectionString(_baseUrl.GetDatabasePath(), false, null), SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.SharedCache))
                    {
                        using (_dbManager.Lock())
                        {
                            try
                            {
                                _dbManager.Insert(item);
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine($"SQLiteError: {ex.Message}");
                            }
                        }
                    }
                }
                else
                {
                    //using (SQLiteConnection _dbManager = new SQLiteConnection(_baseUrl.GetDatabasePath(), WriteOnlyFlags))
                    using (SQLiteConnectionWithLock _dbManager = new SQLiteConnectionWithLock(new SQLiteConnectionString(_baseUrl.GetDatabasePath(), false, null), SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.SharedCache))
                    {
                        using (_dbManager.Lock())
                        {
                            try
                            {
                                _dbManager.Update(item);
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine($"SQLiteError: {ex.Message}");
                            }
                        }
                    }
                }
            }
        }

        public void DeleteAll(System.Collections.IEnumerable collection)
        {
            foreach (var item in collection)
            {
                //using (SQLiteConnection _dbManager = new SQLiteConnection(_baseUrl.GetDatabasePath(), WriteOnlyFlags))
                using (SQLiteConnectionWithLock _dbManager = new SQLiteConnectionWithLock(new SQLiteConnectionString(_baseUrl.GetDatabasePath(), false, null), SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.SharedCache))
                {
                    using (_dbManager.Lock())
                    {
                        try
                        {
                            _dbManager.Delete(item);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"SQLiteError: {ex.Message}");
                        }
                    }
                }
            }
        }

        public int DropTable()
        {
            //using (SQLiteConnection _dbManager = new SQLiteConnection(_baseUrl.GetDatabasePath(), WriteOnlyFlags))
            using (SQLiteConnectionWithLock _dbManager = new SQLiteConnectionWithLock(new SQLiteConnectionString(_baseUrl.GetDatabasePath(), false, null), SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.SharedCache))
            {
                using (_dbManager.Lock())
                {
                    try
                    {
                        return _dbManager.DropTable<T>();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"SQLiteError: {ex.Message}");
                        return -1;
                    }
                }
            }
        }

        // Extensions
        public void InsertWithChildren(T entity)
        {
            //using (SQLiteConnection _dbManager = new SQLiteConnection(_baseUrl.GetDatabasePath(), WriteOnlyFlags))
            using (SQLiteConnectionWithLock _dbManager = new SQLiteConnectionWithLock(new SQLiteConnectionString(_baseUrl.GetDatabasePath(), false, null), SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.SharedCache))
            {
                using (_dbManager.Lock())
                {
                    try
                    {
                        _dbManager.InsertWithChildren(entity, true);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"SQLiteError: {ex.Message}");
                    }
                }
            }
        }

        public void UpdateWithChildren(T entity)
        {
            //using (SQLiteConnection _dbManager = new SQLiteConnection(_baseUrl.GetDatabasePath(), WriteOnlyFlags))
            using (SQLiteConnectionWithLock _dbManager = new SQLiteConnectionWithLock(new SQLiteConnectionString(_baseUrl.GetDatabasePath(), false, null), SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.SharedCache))
            {
                using (_dbManager.Lock())
                {
                    try
                    {
                        _dbManager.UpdateWithChildren(entity);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"SQLiteError: {ex.Message}");
                    }
                }
            }
        }

        public void InsertOrReplaceWithChildren(T entity)
        {
            //using (SQLiteConnection _dbManager = new SQLiteConnection(_baseUrl.GetDatabasePath(), WriteOnlyFlags))
            using (SQLiteConnectionWithLock _dbManager = new SQLiteConnectionWithLock(new SQLiteConnectionString(_baseUrl.GetDatabasePath(), false, null), SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.SharedCache))
            using (_dbManager.Lock())
            {
                try
                {
                    _dbManager.InsertOrReplaceWithChildren(entity, true);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"SQLiteError: {ex.Message}");
                }
            }
        }
        public void InsertOrReplaceAllWithChildren(System.Collections.IEnumerable collection)
        {
            //using (SQLiteConnection _dbManager = new SQLiteConnection(_baseUrl.GetDatabasePath(), WriteOnlyFlags))
            using (SQLiteConnectionWithLock _dbManager = new SQLiteConnectionWithLock(new SQLiteConnectionString(_baseUrl.GetDatabasePath(), false, null), SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.SharedCache))
            {
                using (_dbManager.Lock())
                {
                    try
                    {
                        _dbManager.InsertOrReplaceAllWithChildren(collection, true);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"SQLiteError: {ex.Message}");
                    }
                }
            }
        }

        public void InsertAllWithChildren(System.Collections.IEnumerable collection)
        {
            //using (SQLiteConnection _dbManager = new SQLiteConnection(_baseUrl.GetDatabasePath(), WriteOnlyFlags))
            using (SQLiteConnectionWithLock _dbManager = new SQLiteConnectionWithLock(new SQLiteConnectionString(_baseUrl.GetDatabasePath(), false, null), SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.SharedCache))
            {
                using (_dbManager.Lock())
                {
                    try
                    {
                        _dbManager.InsertAllWithChildren(collection, true);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"SQLiteError: {ex.Message}");
                    }
                }
            }
        }

        public void UpdateAllWithChildren(System.Collections.IEnumerable collection)
        {
            foreach (T item in collection)
            {
                //using (SQLiteConnection _dbManager = new SQLiteConnection(_baseUrl.GetDatabasePath(), WriteOnlyFlags))
                using (SQLiteConnectionWithLock _dbManager = new SQLiteConnectionWithLock(new SQLiteConnectionString(_baseUrl.GetDatabasePath(), false, null), SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.SharedCache))
                {
                    using (_dbManager.Lock())
                    {
                        try
                        {
                            _dbManager.UpdateWithChildren(item);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"SQLiteError: {ex.Message}");
                        }
                    }
                }
            }
        }
        #endregion Write Only Connection
    }



}

