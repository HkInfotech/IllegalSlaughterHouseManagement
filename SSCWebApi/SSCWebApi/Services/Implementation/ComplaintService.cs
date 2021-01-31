using SSCWebApi.Helper;
using SSCWebApi.Models;
using SSCWebApi.Models.ModelDTO;
using SSCWebApi.Services.Interface;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SSCWebApi.Services.Implementation
{
    public class ComplaintService : IComplaintService
    {
        private SSCEntities SSCEntities;

        public ComplaintService()
        {
            SSCEntities = new SSCEntities();
        }

        public async Task<Responce<ComplaintsDTO>> SaveComplaint(HttpRequest complaintRequest, string UserName)
        {
            Responce<ComplaintsDTO> Responce = new Responce<ComplaintsDTO>();
            Responce.Success = true;
            try
            {
                using (SSCEntities = new SSCEntities())
                {
                    Complaints complaints = new Complaints();
                    int Id = Convert.ToInt32(complaintRequest.Form["Id"]);
                    var Cityid = Convert.ToInt32(complaintRequest.Form["Cityid"]);
                    var SpeciesId = Convert.ToInt32(complaintRequest.Form["SpeciesId"]);
                    complaints.City = Cityid;

                    complaints.Id = Id;
                    if (complaints.Id != 0)
                    {
                        complaints = SSCEntities.Complaints.Find(Id);
                        var ComplaintLows = complaints.ComplaintsLows;
                        var ComplaintImages = complaints.ComplaintImages;
                        SSCEntities.ComplaintsLows.RemoveRange(ComplaintLows);
                        SSCEntities.ComplaintImages.RemoveRange(ComplaintImages);
                        await SSCEntities.SaveChangesAsync();
                    }

                    complaints.SpeciesId = SpeciesId;
                    complaints.ShopName = Convert.ToString(complaintRequest.Form["ShopName"]);
                    complaints.ShopAddress = Convert.ToString(complaintRequest.Form["ShopAddress"]);
                    complaints.DateOfInspection = Convert.ToDateTime(complaintRequest.Form["DateOfInspection"]);
                    complaints.Comments = Convert.ToString(complaintRequest.Form["Comments"]);
                    complaints.Violations = Convert.ToString(complaintRequest.Form["Violations"]);
                    complaints.GpsLocations = Convert.ToString(complaintRequest.Form["GpsLocations"]);
                    complaints.UserId = Convert.ToString(complaintRequest.Form["UserId"]);
                    complaints.ComplainStatus = Convert.ToInt32(complaintRequest.Form["ComplainStatus"]);
                    complaints.GroupName = Convert.ToString(complaintRequest.Form["GroupName"]);
                    complaints.IsDelete = Convert.ToBoolean(complaintRequest.Form["IsDelete"]);
                    complaints.IsActive = Convert.ToBoolean(complaintRequest.Form["IsActive"]);

                    
                    complaints.ModifiedBy = UserName ?? Convert.ToString(complaintRequest.Form["UserId"]);
                  
                    complaints.ModifiedDate = DateTime.UtcNow;
                    complaints.IsRejecet = Convert.ToBoolean(complaintRequest.Form["IsRejecet"]);
                    complaints.CommentForRejection = Convert.ToString(complaintRequest.Form["CommentForRejection"]);
                    complaints.RegistrationDate = Convert.ToDateTime(complaintRequest.Form["RegistrationDate"]);
                    complaints.IsRegister = Convert.ToBoolean(complaintRequest.Form["IsRegister"]);
                    complaints.IsEmailSend = Convert.ToBoolean(complaintRequest.Form["IsEmailSend"]);
                    if (complaints.Id != 0)
                    {
                        SSCEntities.Entry(complaints).State = EntityState.Modified;
                    }
                    else
                    {
                        complaints.CreatedBy = UserName ?? Convert.ToString(complaintRequest.Form["UserId"]);
                        complaints.CreatedDate = DateTime.UtcNow;
                        SSCEntities.Complaints.Add(complaints);

                    }
                    await SSCEntities.SaveChangesAsync();
                    var LowsIds = Convert.ToString(complaintRequest.Form["LowIds"]).Split(',');

                    if (LowsIds.Count() > 0)
                    {
                        foreach (var item in LowsIds)
                        {
                            ComplaintsLows complaintsLows = new ComplaintsLows();
                            complaintsLows.LowId = Convert.ToInt32(item);
                            complaintsLows.ComplaintId = complaints.Id;
                            SSCEntities.ComplaintsLows.Add(complaintsLows);
                            await SSCEntities.SaveChangesAsync();
                        }
                    }

                    if (complaintRequest.Files != null)
                    {
                        if (complaintRequest.Files.Count > 0)
                        {
                            for (int i = 0; i < complaintRequest.Files.Count; i++)
                            {
                                if (complaintRequest.Files[i].ContentLength > 0)
                                {
                                    ComplaintImages complaintsImages = new ComplaintImages();
                                    string extension = Path.GetExtension(complaintRequest.Files[i].FileName);
                                    bool isImage = Functions.IsImage(complaintRequest.Files[i].FileName);
                                    string ContentType = complaintRequest.Files[i].ContentType;
                                    string[] bloburi = AzureStore.UploadFile(complaintRequest.Files[i].InputStream, complaintRequest.Files[i].FileName, ContentType, isImage, extension);
                                    complaintsImages.ComplaintId = complaints.Id;
                                    complaintsImages.Imagetype = extension;
                                    complaintsImages.ImageUrl = bloburi[0];
                                    complaintsImages.CreatedDate = DateTime.UtcNow;
                                    complaintsImages.ModifiedDate = DateTime.UtcNow;
                                    complaintsImages.CreatedBy = UserName;
                                    complaintsImages.ModifiedBy = UserName;
                                    SSCEntities.ComplaintImages.Add(complaintsImages);
                                    await SSCEntities.SaveChangesAsync();
                                }

                            }



                            //foreach (string item in complaintRequest.Files)
                            //{
                            //    var postedFile = complaintRequest.Files[item];
                            //    if (!string.IsNullOrEmpty(postedFile.FileName))
                            //    {
                            //        ComplaintImages complaintsImages = new ComplaintImages();

                            //        string extension = Path.GetExtension(postedFile.FileName);

                            //    }
                            //    i = i + 1;
                            //}
                        }
                    }
                    ComplaintsDTO complaintsdto = new ComplaintsDTO();
                    SSCEntities db = new SSCEntities();
                    var saveComplaintobj = db.Complaints.Find(complaints.Id);
                    complaintsdto = saveComplaintobj.MapComplaintsToDTO();
                    Responce.ResponeContent = complaintsdto;
                }
            }
            catch (Exception ex)
            {
                Responce.Success = false;
                Responce.Message = $"ERROR SaveComplaint :{ex.ToString()}";
            }
            return Responce;
        }

#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        public async Task<Responce<List<ComplaintsDTO>>> GetComplaint(MobileRequest mobileRequest)
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        {
            Responce<List<ComplaintsDTO>> Responce = new Responce<List<ComplaintsDTO>>();
            Responce.Success = true;
            try
            {
                List<ComplaintsDTO> Complaints = new List<ComplaintsDTO>();
                List<Complaints> ComplaintsList = new List<Complaints>();
               
                using (SSCEntities db = new SSCEntities())
                {
                   
                    if (mobileRequest.Id != 0)
                    {
                        var Getcomplaintbyid = db.Complaints.Find(mobileRequest.Id);
                        if (Getcomplaintbyid != null)
                        {
                            ComplaintsList.Add(Getcomplaintbyid);
                        }
                    }
                    else
                    {
                        var UserCity = db.AspNetUsers.Find(mobileRequest.Username).City;
                        var UserName = db.AspNetUsers.Find(mobileRequest.Username).UserName;
                        var CityId = db.Citys.Where(a => a.CityName.ToLower().Equals(UserCity.ToLower()))?.FirstOrDefault().Id;
                        var UserRoles = db.GetUserRole(mobileRequest.Username).FirstOrDefault().UserRole;

                        switch (UserRoles)
                        {
                            case "Volunteer":
                                ComplaintsList = db.Complaints.Where(a => a.UserId.Trim().ToLower() == mobileRequest.Username.Trim().ToLower()).ToList();
                                break;
                            case "Controller":
                                ComplaintsList = db.Complaints.Where(a => a.City == CityId).ToList();
                                break;
                            case "Admin":
                                foreach (Citys city in db.Citys.Where(a => a.AdminEmail == UserName || a.Id == CityId).ToList())
                                {
                                    ComplaintsList = ComplaintsList.Concat(db.Complaints.Where(a => a.City == city.Id).ToList()).ToList();
                                }
                                break;
                            case "Site Admin":
                                ComplaintsList = db.Complaints.ToList();
                                break;
                        }

                        //if (UserRoles.Equals("Volunteer")) 
                        //{
                        //ComplaintsList = db.Complaints.Where(a=>a.City==CityId && a.UserId.Trim().ToLower()== mobileRequest.Username.Trim().ToLower()).ToList();

                        //}
                        //else
                        //{
                        //        ComplaintsList = db.Complaints.Where(a => a.City == CityId).ToList();
                        //}

                    }
                    
                    Complaints = ComplaintsList?.Select(a => a.MapComplaintsToDTO()).OrderByDescending(a => a.ModifiedDate).OrderBy(a => a.ComplainStatus).ToList();
                    Responce.ResponeContent = Complaints;
                }
            }
            catch (Exception ex)
            {
                Responce.Success = false;
                Responce.Message = $"ERROR GetComplaint :{ex.ToString()}";
            }
            return Responce;
        }


        
    }
}