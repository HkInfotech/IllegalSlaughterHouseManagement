using SQLite;
using SSCMobileDataBus.Offline_Store.Services.Interfaces;
using SSCMobileDataBus.OfflineStore.Models;
using SSCMobileServiceBus.Platform_Specific_Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSCMobileDataBus.OfflineStore.Services.Implementations
{
    public class Repository<T> : IRepository<T> where T : ModelBase, new()
    {
        IBaseUrl _baseUrl;
        private readonly SQLiteConnection _dbManager;
        public Repository(IBaseUrl baseUrl)
        {
            _baseUrl = baseUrl;
            //if (_dbManager == null)
            //{
            _dbManager = new SQLiteConnection(_baseUrl.GetDatabasePath());
            _dbManager.CreateTable<T>();
            //}
        }
        public List<T> GetItems() => _dbManager.Table<T>().ToList();
        public T GetItemsById(long Id) => _dbManager.Find<T>(Id);

        public int Insert(T Entity) => _dbManager.Insert(Entity);
        public int Update(T Entity) => Entity.Id == 0 ? _dbManager.Insert(Entity) : _dbManager.Update(Entity);
        public int Delete(T Entity) => _dbManager.Delete(Entity);

        public int InsertAll(System.Collections.IEnumerable collections) => _dbManager.InsertAll(collections);
        public int UpdateAll(System.Collections.IEnumerable collections) => _dbManager.UpdateAll(collections);

    }
}
