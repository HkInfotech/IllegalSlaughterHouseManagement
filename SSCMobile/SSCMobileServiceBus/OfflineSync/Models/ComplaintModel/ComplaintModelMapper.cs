using SSCMobileServiceBus.OnlineSync.Models.ComplaintModelDTO;
using System;

namespace SSCMobileServiceBus.OfflineSync.Models.ComplaintModel
{
    public static class ComplaintModelMapper
    {
        public static CitysModel MapCityModel(this CityDTO cityDTO)
        {
            return new CitysModel()
            {
                Id = cityDTO.Id,
                ExternalId = cityDTO.Id,
                CityName = cityDTO.CityName,
                CountryId = cityDTO.CountryId,
                IsActive= cityDTO.IsActive,
                AdminEmail=cityDTO.AdminEmail,
                CreatedBy = cityDTO.CreatedBy,
                ModifiedBy = cityDTO.ModifiedBy,
                ModifiedDate = cityDTO.ModifiedDate ?? DateTime.UtcNow,
                CreatedDate = cityDTO.CreatedDate ?? DateTime.UtcNow,
                MCEmail = cityDTO.MCEmail,
                FcciEmail = cityDTO.FcciEmail
            };
        }

        public static SpeciesModel MapSpeciesModel(this SpeciesDTO speciesDTO)
        {
            return new SpeciesModel()
            {
                Id = speciesDTO.Id,
                ExternalId = speciesDTO.Id,
                Icon = speciesDTO.Icon,
                SpeciesName = speciesDTO.SpeciesName,
                CreatedBy = speciesDTO.CreatedBy,
                ModifiedBy = speciesDTO.ModifiedBy,
                ModifiedDate = speciesDTO.ModifiedDate ?? DateTime.UtcNow,
                CreatedDate = speciesDTO.CreatedDate ?? DateTime.UtcNow
            };
        }

        public static CityLowsModel MapCityLowsModel(this CityLowsDTO cityLowsDTO)
        {
            return new CityLowsModel()
            {
                Id = Convert.ToInt32(cityLowsDTO.Id),
                ExternalId = Convert.ToInt32(cityLowsDTO.Id),
                CreatedBy = cityLowsDTO.CreatedBy,
                CityId = cityLowsDTO.CityId,
                LowId = cityLowsDTO.LowId,
                ModifiedBy = cityLowsDTO.ModifiedBy,
                ModifiedDate = cityLowsDTO.ModifiedDate ?? DateTime.UtcNow,
                CreatedDate = cityLowsDTO.CreatedDate ?? DateTime.UtcNow
            };
        }

        public static LowsModel MapLowsModel(this LowDTO LowDTO)
        {
            return new LowsModel()
            {
                Id = LowDTO.Id,
                ExternalId = LowDTO.Id,
                LowsTitile = LowDTO.LowsTitile,
                LowsDescriptions = LowDTO.LowsDescriptions,
                IsActive = LowDTO.IsActive,
                IsDelete = LowDTO.IsDelete,
                CreatedBy = LowDTO.CreatedBy,
                ModifiedBy = LowDTO.ModifiedBy,
                ModifiedDate = LowDTO.ModifiedDate ?? DateTime.UtcNow,
                CreatedDate = LowDTO.CreatedDate ?? DateTime.UtcNow
            };
        }

        public static ComplaintModel MapComplaintModel(this ComplaintsDTO ComplaintDTO)
        {
            return new ComplaintModel()
            {
                ExternalId = ComplaintDTO.Id,
                ShopName = ComplaintDTO.ShopName,
                ShopAddress = ComplaintDTO.ShopAddress,
                DateOfInspection = ComplaintDTO.DateOfInspection,
                Violations = ComplaintDTO.Violations,
                GpsLocations = ComplaintDTO.GpsLocations,
                Cityid = ComplaintDTO.Cityid,
                SpeciesId = ComplaintDTO.SpeciesId,
                CreatedBy = ComplaintDTO.CreatedBy,
                CreatedDate = ComplaintDTO.CreatedDate ?? DateTime.Now,
                ModifiedDate = ComplaintDTO.ModifiedDate ?? DateTime.Now,
                ModifiedBy = ComplaintDTO.CreatedBy,
                IsEmptyModel = false,
                UserId = ComplaintDTO.UserId,
                ComplainStatus = ComplaintDTO.ComplainStatus,
                GroupName = ComplaintDTO.GroupName,
                IsActive = ComplaintDTO.IsActive,
                IsDelete = ComplaintDTO.IsDelete,
                ComplaintLowsId = ComplaintDTO.ComplaintLowsId,
                ComplaintFiles = ComplaintDTO.ComplaintFiles,
                Operation = (int)Operations.Sync,
                IsRejecet = ComplaintDTO.IsRejecet,
                CommentForRejection = ComplaintDTO.CommentForRejection,
                RegistrationDate = ComplaintDTO.RegistrationDate,
                IsRegister = ComplaintDTO.IsRegister,
                IsEmailSend = ComplaintDTO.IsEmailSend,


            };
        }
        public static ComplaintsDTO MapComplaintDTO(this ComplaintModel ComplaintModel)
        {
            return new ComplaintsDTO()
            {
                Id = ComplaintModel.Id,
                ShopName = ComplaintModel.ShopName,
                ShopAddress = ComplaintModel.ShopAddress,
                DateOfInspection = ComplaintModel.DateOfInspection,
                Violations = ComplaintModel.Violations,
                GpsLocations = ComplaintModel.GpsLocations,
                Cityid = ComplaintModel.Cityid,
                SpeciesId = ComplaintModel.SpeciesId,
                CreatedBy = ComplaintModel.CreatedBy,
                CreatedDate = ComplaintModel.CreatedDate,
                ModifiedDate = ComplaintModel.ModifiedDate,
                ModifiedBy = ComplaintModel.CreatedBy,
                IsEmptyModel = false,
                UserId = ComplaintModel.UserId,
                ComplainStatus = ComplaintModel.ComplainStatus,
                GroupName = ComplaintModel.GroupName,
                IsActive = ComplaintModel.IsActive,
                IsDelete = ComplaintModel.IsDelete,
                ComplaintLowsId = ComplaintModel.ComplaintLowsId,
                ComplaintFiles = ComplaintModel.ComplaintFiles,
                IsRejecet = ComplaintModel.IsRejecet,
                CommentForRejection = ComplaintModel.CommentForRejection,
                RegistrationDate = ComplaintModel.RegistrationDate,
                IsRegister = ComplaintModel.IsRegister,
                IsEmailSend = ComplaintModel.IsEmailSend,


            };
        }
    }
}