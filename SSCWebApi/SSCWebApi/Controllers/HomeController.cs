using SSCWebApi.Helper;
using SSCWebApi.Models;
using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;

namespace SSCWebApi.Controllers
{
    public class HomeController : Controller
    {
        private SSCEntities SSCEntities;
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult ComplaintEmail(int Id)
        {
            SSCEntities = new SSCEntities();
            var complaints = SSCEntities.Complaints.Find(Id);
            if (complaints!=null) 
            {
                return View(complaints);
            }
            return RedirectToAction("Error", "Home");
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Termsandcondition()
        {
            return View();
        }
        public ActionResult Error() 
        {
            return View();
        }


        [AllowAnonymous]
        [HttpGet]
        public ActionResult SendEmail(int Id)
        {
            SSCEntities = new SSCEntities();
            var complaints = SSCEntities.Complaints.Find(Id);
            if (complaints != null)
            {
                WebClient client = new WebClient();
                string EmailBodyURL = Url.Action("ComplaintEmail", "Home", new { Id = Id });
                EmailBodyURL = new Uri(Request.Url, Url.Content("~/" + EmailBodyURL)).ToString();
                string EmailBody = client.DownloadString(EmailBodyURL);
                string EmailSubject = $"Immediate action required against illegal meat shops in {complaints.ShopAddress}";
                //string ToAddress = "Vishalmakwana@live.in";
                string ToAddress = "makavana.vishal@gmail.com";
                string FromAddress = "support@resquark.com";
                string CCAddress = "";
                string smtp = ConfigurationManager.AppSettings["smtp"].ToString();
                string smtplogin = ConfigurationManager.AppSettings["smtplogin"].ToString();
                string smtppwd = ConfigurationManager.AppSettings["smtppwd"].ToString();
                int port = Convert.ToInt16(ConfigurationManager.AppSettings["smtpport"].ToString());
                MailMessage mail = new MailMessage();
                mail.IsBodyHtml = true;
                mail.From = new MailAddress(FromAddress);
                foreach (string address in ToAddress.Split(';')) mail.To.Add(address);
                if (CCAddress != "") foreach (string address in CCAddress.Split(';')) mail.CC.Add(address);
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
                return RedirectToAction("ComplaintEmail", new { Id = complaints.Id });
              
            }
            return RedirectToAction("Error", "Home");
        }
    }
}