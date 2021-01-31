using SQLite;
using System;

namespace SSCMobileServiceBus.OfflineSync
{
    public class ModelBase
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int ExternalId { get; set; }
        public DateTime ModifiedDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedByFullName { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedByFullName { get; set; }
        public int Operation { get; set; }
        public int ApplicationType { get; set; }
        public bool IsEmptyModel { get; set; }
        //public bool IsActive { get; set; }
        //public int ExternalId { get; set; }

        // Legacy data
        //public string TypeScreenInfo { get; set; }
        //public string SystemCode { get; set; }
        //public string TransactionType { get; set; } //M- when view master Data, T- When perform operation for Transaction table offline mode
        //public int? CategoryId { get; set; }
        //public string CategoryName { get; set; }
        //public DateTime? LastSyncDate { get; set; }
        //public int InstanceUserAssocId { get; set; }
        //public int? ExternalId { get; set; }
    }
}