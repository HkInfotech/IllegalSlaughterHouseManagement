
using SSCMobileDataBus.OfflineStore.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSCMobileDataBus.Offline_Store.Services.Interfaces
{
    public interface IRepository<T> where T : ModelBase, new()
    {
        List<T> GetItems();
        T GetItemsById(long Id);
        int Insert(T Entity);
        int Update(T Entity);
        int Delete(T Entity);
        int InsertAll(System.Collections.IEnumerable collections);
        int UpdateAll(System.Collections.IEnumerable collections);
    }
}
