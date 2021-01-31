using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSCMobileDataBus.OfflineStore.Models
{
    public class ModelBase
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? CreateDateTime { get; set; }
        public DateTime? UpdateDateTime { get; set; }
        public bool IsActive { get; set; }
        public bool IsEmptyModel { get; set; }
    }
}
