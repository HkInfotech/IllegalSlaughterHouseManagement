using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSCMobileServiceBus.OfflineSync.Models.ThinViewModels
{
    public class BaseThinViewModel : BaseViewModel, IViewModelExtensions
    {

        private int _id;
        [ModelProperty(nameof(ModelBase.Id))]
        public int Id
        {
            get => _id;
            set { SetProperty(ref _id, value); }
        }
        private int _externalId;
        [ModelProperty(nameof(ModelBase.ExternalId))]
        public int ExternalId
        {
            get => _externalId;
            set { SetProperty(ref _externalId, value); }
        }
        private string _modifiedBy;
        [ModelProperty(nameof(ModelBase.ModifiedBy))]
        public string ModifiedBy
        {
            get => _modifiedBy;
            set { SetProperty(ref _modifiedBy, value); }
        }
        private string _modifiedByFullname;
        [ModelProperty(nameof(ModelBase.ModifiedByFullName))]
        public string ModifiedByFullName
        {
            get => _modifiedByFullname;
            set { SetProperty(ref _modifiedByFullname, value); }
        }
        private DateTime _modifiedAt;
        [ModelProperty(nameof(ModelBase.ModifiedAt))]
        public DateTime ModifiedAt
        {
            get => _modifiedAt;
            set { SetProperty(ref _modifiedAt, value); }
        }
        private string _createdBy;
        [ModelProperty(nameof(ModelBase.CreatedBy))]
        public string CreatedBy
        {
            get => _createdBy;
            set { SetProperty(ref _createdBy, value); }
        }
        private DateTime _createdAt;
        [ModelProperty(nameof(ModelBase.CreatedAt))]
        public DateTime CreatedAt
        {
            get => _createdAt;
            set { SetProperty(ref _createdAt, value); }
        }
        private string _createdByFullname;
        [ModelProperty(nameof(ModelBase.CreatedByFullName))]
        public string CreatedByFullName
        {
            get => _createdByFullname;
            set { SetProperty(ref _createdByFullname, value); }
        }
        private Operation _operation;
        [ModelProperty(nameof(ModelBase.Operation))]
        public Operation Operation
        {
            get => _operation;
            set { SetProperty(ref _operation, value); }
        }
    }
}
