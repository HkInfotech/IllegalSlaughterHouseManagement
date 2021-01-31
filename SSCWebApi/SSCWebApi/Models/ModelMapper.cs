using SSCWebApi.Helper;
using SSCWebApi.Models.ModelDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SSCWebApi.Models
{
    public static class ModelMapper
    {
        public static ComplaintsDTO MapComplaintsToDTO(this Complaints complaints)
        {
            return new ComplaintsDTO()
            {
                Id = complaints.Id,
                ShopName = complaints.ShopName,
                ShopAddress = complaints.ShopAddress,
                DateOfInspection = complaints.DateOfInspection,
                Comments = complaints.Comments,
                Violations = complaints.Violations,
                GpsLocations = complaints.GpsLocations,
                Cityid = complaints.City ?? 0,
                SpeciesId = complaints.SpeciesId ?? 0,
                ComplainStatus = complaints.ComplainStatus,
                GroupName = complaints.GroupName,
                IsDelete = complaints.IsDelete,
                IsRejecet = complaints.IsRejecet,
                CommentForRejection = complaints.CommentForRejection,
                RegistrationDate = complaints.RegistrationDate,
                IsRegister = complaints.IsRegister,
                IsEmailSend = complaints.IsEmailSend,
                IsActive = complaints.IsActive,
                SpeciesName = complaints.Species.SpeciesName,
                CreatedBy = complaints.CreatedBy,
                ComplaintStatusValue = Functions.GetComplaintStatus(complaints.ComplainStatus ?? 1),
                CreatedDate = complaints.CreatedDate,
                ModifiedBy = complaints.ModifiedBy,
                ModifiedDate = complaints.ModifiedDate,
                CreatedUserName= Functions.GetUserName(complaints.CreatedBy).Name,
                UserMobileNumber= Functions.GetUserName(complaints.CreatedBy).PhoneNumber,
                CityName=complaints.Citys.CityName,
                ComplaintLows = complaints.ComplaintsLows?.Select(a => new LowDTO()
                {
                    Id = a.Lows.Id,
                    LowsTitile = a.Lows.LowsTitile,
                    LowsDescriptions = a.Lows.LowsDescriptions,
                    IsActive = a.Lows.IsActive,
                    IsDelete = a.Lows.IsDelete,
                    ModifiedBy = a.Lows.ModifiedBy,
                    ModifiedDate = a.Lows.ModifiedDate,
                    CreatedBy = a.Lows.CreatedBy,
                    CreatedDate = a.Lows.CreatedDate
                }).ToList(),
                ComplaintLowsId = string.Join(",", complaints.ComplaintsLows?.Select(a => Convert.ToString(a.LowId ?? 0)).ToList()),
                ComplaintFiles = string.Join(",", complaints.ComplaintImages?.Select(a => Convert.ToString(a.ImageUrl)).ToList()),
                ComplaintImages = complaints.ComplaintImages?.Select(a => new ComplaintImagesDTO()
                {
                    Id = a.Id,
                    ComplaintId = a.ComplaintId,
                    ImageUrl = a.ImageUrl,
                    Imagetype = a.Imagetype,
                    IsDelete = a.IsDelete
                }

                ).ToList()
            };
        }
    }
}