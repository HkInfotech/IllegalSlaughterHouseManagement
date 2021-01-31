using Microsoft.AspNet.Identity;
using SSCWebApi.Helper;
using SSCWebApi.Models;
using SSCWebApi.Models.Common;
using SSCWebApi.Models.ModelDTO;
using SSCWebApi.Services.Implementation;
using SSCWebApi.Services.Interface;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SSCWebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/Complaint")]
    public class ComplaintController : BaseApiController
    {
        private readonly IComplaintService _complaintService;
        public ComplaintController()
        {
            _complaintService = new ComplaintService();
        }


        [Route("SaveComplaint")]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IHttpActionResult> SaveComplaint()
        {
            Responce<ComplaintsDTO> responce = new Responce<ComplaintsDTO>();

            var HttpRequest = HttpContext.Current.Request;

            try
            {
                string UserName = User.Identity.GetUserId();
                responce = await _complaintService.SaveComplaint(HttpRequest, UserName);
                return Ok(responce);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("GetComplaints")]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IHttpActionResult> GetComplaints(MobileRequest mobileRequest)
        {
            Responce<List<ComplaintsDTO>> responce = new Responce<List<ComplaintsDTO>>();

            var HttpRequest = HttpContext.Current.Request;

            try
            {

                string UserName = User.Identity.GetUserId();
                //mobileRequest.Username = UserName;


                if (!string.IsNullOrEmpty(UserName))
                {
                    mobileRequest.Username = UserName;
                }
                responce = await _complaintService.GetComplaint(mobileRequest);
                return Ok(responce);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("GetComplaintList")]
        public HttpResponseMessage GetComplaintlist()
        {

            var SSCEntities = new SSCEntities();
            DataTable dt = SSCEntities.ComplaintList.OrderByDescending(item => item.CreateDate).ToList().ToDataTable();

            //Convert the rendering of the gridview to a string representation 
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            GridView gv = new GridView();
            gv.DataSource = dt;
            gv.DataBind();

            PrepareGridViewForExport(gv);

            gv.RenderControl(htw);

            //Open a memory stream that you can use to write back to the response
            byte[] byteArray = Encoding.ASCII.GetBytes(sw.ToString());


            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new ByteArrayContent(byteArray);
            response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = "Complaints-" + DateTime.Now.ToString("yyyyMMddHHmm") + ".xls";
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.ms-excel");

            return response;


        }
        private void PrepareGridViewForExport(Control gv)
        {
            Literal l = new Literal();
            string name = String.Empty;
            for (int i = 0; i < gv.Controls.Count; i++)
            {
                if (gv.Controls[i].GetType() == typeof(CheckBox))
                {
                    l.Text = ((CheckBox)gv.Controls[i]).Checked.ToString();
                    gv.Controls.Remove(gv.Controls[i]);
                    gv.Controls.AddAt(i, l);
                }
                if (gv.Controls[i].HasControls())
                {
                    PrepareGridViewForExport(gv.Controls[i]);
                }
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("SendMail")]
        public async Task<IHttpActionResult> SendMail(MobileRequest mobileRequest)
        {
            Boolean EmailTest = false;
            string EmailTestTo = "pateljigu210@gmail.com";

            if (ConfigurationManager.AppSettings.AllKeys.Contains("EmailTest"))
                EmailTest = Convert.ToBoolean(ConfigurationManager.AppSettings["EmailTest"].ToString());

            if (ConfigurationManager.AppSettings.AllKeys.Contains("EmailTestTo"))
                EmailTestTo = ConfigurationManager.AppSettings["EmailTestTo"].ToString();

            Responce<List<bool>> responce = new Responce<List<bool>>();
            responce.Success = true;
            try
            {
                using (var SSCEntities = new SSCEntities())
                {
                    var complaints = SSCEntities.Complaints.Find(mobileRequest.Id);
                    if (complaints!=null && complaints.IsEmailSend==false) 
                    {
                        await SendMailMethod(complaints, EmailTest ? EmailTestTo : complaints.Citys.FcciEmail);
                        await SendMailMethod(complaints, EmailTest ? EmailTestTo : complaints.Citys.MCEmail);
                        complaints.IsEmailSend = true;
                        complaints.ComplainStatus = (int)ComplaintStatusEnum.Registered;
                        SSCEntities.Entry(complaints).State = System.Data.Entity.EntityState.Modified;
                        await SSCEntities.SaveChangesAsync();
                    }             

                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                responce.Success = false;
            }
            return Ok(responce);
        }


        public async Task<IHttpActionResult> SendMailMethod(Complaints complaints,string ToAddress) 

        {
            if (complaints != null)
            {
                ToAddress = ToAddress.Replace(",", ";").Replace(" ","");

                WebClient client = new WebClient();
                string EmailBodyURL = $"http://ssc.resquark.com/home/ComplaintEmail?Id={complaints.Id}";
                string EmailBody = client.DownloadString(EmailBodyURL);
                string EmailSubject = $"Immediate action required against illegal meat shops in {complaints.ShopAddress}";
                string FromAddress = ConfigurationManager.AppSettings["emailfrom"].ToString();
                string CCAddress = ConfigurationManager.AppSettings["emailcc"].ToString();
                string BCCAddress = ConfigurationManager.AppSettings["emailbcc"].ToString();
                string smtp = ConfigurationManager.AppSettings["smtp"].ToString();
                string smtplogin = ConfigurationManager.AppSettings["smtplogin"].ToString();
                string smtppwd = ConfigurationManager.AppSettings["smtppwd"].ToString();
                int port = Convert.ToInt16(ConfigurationManager.AppSettings["smtpport"].ToString());
                MailMessage mail = new MailMessage();
                mail.IsBodyHtml = true;
                mail.From = new MailAddress(FromAddress);
                foreach (string address in ToAddress.Split(';')) mail.To.Add(address);

                if (CCAddress != "")
                {
                    CCAddress = CCAddress.Replace(",", ";").Replace(" ", "");
                    foreach (string address in CCAddress.Split(';')) mail.CC.Add(address);
                }

                if (BCCAddress != "")
                {
                    BCCAddress = BCCAddress.Replace(",", ";").Replace(" ", "");
                    foreach (string address in BCCAddress.Split(';')) mail.Bcc.Add(address);
                }
                mail.Subject = EmailSubject;
                mail.Body = EmailBody;
                mail.Priority = MailPriority.High;
                try
                {
                    int cnt = 1;
                    foreach (var item in complaints.ComplaintImages)
                    {
                        WebRequest req = WebRequest.Create(item.ImageUrl);
                        string Contanttype = ".jpg";
                        string Extension = common.GetMimeType(Contanttype);
                        WebResponse response = req.GetResponse();
                        Stream stream = response.GetResponseStream();
                        mail.Attachments.Add(new Attachment(stream, "File" + cnt + Contanttype, Extension));
                        cnt++;
                    }
                }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
                catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
                {

                }
                SmtpClient Smtpclient = new SmtpClient(smtp, port);
                Smtpclient.UseDefaultCredentials = false;
                Smtpclient.Credentials = new System.Net.NetworkCredential(smtplogin, smtppwd);
                Smtpclient.DeliveryMethod = SmtpDeliveryMethod.Network;
                Smtpclient.Send(mail);

            }
            else
            {
                return BadRequest();
            }
            return BadRequest();
        }
        
    }
}
