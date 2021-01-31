using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SSCWebApi.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace SSCWebApi.Helper
{
    public static class Functions
    {
        public static Image byteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }

        public static byte[] imageToByteArray(System.Drawing.Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
            return ms.ToArray();
        }
        public static DateTime GetCurrentDateTime()
        {
            var DateDiffrentMinute = Convert.ToDouble(ConfigurationManager.AppSettings["DateDiffrentMinute"]);

            DateTime ReturnDate = DateTime.Now.AddMinutes(DateDiffrentMinute);

            return ReturnDate;
        }

        private static readonly IDictionary<string, string> ImageMimeDictionary
    = new Dictionary<string, string>{
                    { ".bmp", "image/bmp" },
                    { ".dib", "image/bmp" },
                    { ".gif", "image/gif" },
                    { ".svg", "image/svg+xml" },
                    { ".jpe", "image/jpeg" },
                    { ".jpeg", "image/jpeg" },
                    { ".jpg", "image/jpeg" },
                    { ".png", "image/png" },
                    { ".pnz", "image/png" }
        };

        public static bool IsImage(string file)
        {
            if (string.IsNullOrEmpty(file))
            {
                throw new ArgumentNullException(nameof(file));
            }

            var extension = Path.GetExtension(file);
            return ImageMimeDictionary.ContainsKey(extension);
        }

        public static System.Drawing.Image RotateImage(System.Drawing.Image imgSource)
        {
            if (Array.IndexOf(imgSource.PropertyIdList, 274) > -1)
            {
                var orientation = (int)imgSource.GetPropertyItem(274).Value[0];
                switch (orientation)
                {
                    case 1:
                        // No rotation required.
                        break;
                    case 2:
                        imgSource.RotateFlip(RotateFlipType.RotateNoneFlipX);
                        break;
                    case 3:
                        imgSource.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        break;
                    case 4:
                        imgSource.RotateFlip(RotateFlipType.Rotate180FlipX);
                        break;
                    case 5:
                        imgSource.RotateFlip(RotateFlipType.Rotate90FlipX);
                        break;
                    case 6:
                        imgSource.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        break;
                    case 7:
                        imgSource.RotateFlip(RotateFlipType.Rotate270FlipX);
                        break;
                    case 8:
                        imgSource.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        break;
                }
                // This EXIF data is now invalid and should be removed.
                imgSource.RemovePropertyItem(274);
            }
            return imgSource;
        }

        public static Bitmap ZoomImage(Image image, int width, int height)
        {

            Rectangle destRect;
            int x = 0;
            int y = 0;

            decimal r1 = (decimal)image.Width / image.Height;
            decimal r2 = (decimal)width / height;
            int w = width;
            int h = height;
            if (r1 > r2)
            {
                w = width;
                h = (int)(w / r1);
            }
            else if (r1 < r2)
            {
                h = height;
                w = (int)(r1 * h);
            }
            x = (width - w) / 2;
            y = (height - h) / 2;

            Rectangle source_rect = new Rectangle(x, y, width, height);
            destRect = new Rectangle(0, 0, width, height);

            // Copy that part of the image to a new bitmap.
            Bitmap new_image = new Bitmap(width, height);
            using (Graphics gr = Graphics.FromImage(new_image))
            {
                gr.DrawImage(image, destRect, source_rect, GraphicsUnit.Pixel);
            }

            return new_image;

        }

        public static Image ResizeImage(Image image, int width, int height, int offset = 0)
        {
            Rectangle destRect;

            destRect = new Rectangle(0, 0, width - offset, height - offset);
            Bitmap destImage = new Bitmap(width - offset, height - offset);

            destImage.SetResolution(96, 96);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0 + offset, 0 + offset, image.Width - offset, image.Height - offset, GraphicsUnit.Pixel, wrapMode);
                }
            }


            return Compress(destImage);
        }

        private static Image Compress(Bitmap imgBitmap)
        {
            MemoryStream ms = new MemoryStream();
            ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);

            // Create an Encoder object based on the GUID
            // for the Quality parameter category.
            System.Drawing.Imaging.Encoder myEncoder =
                System.Drawing.Imaging.Encoder.Quality;

            // Create an EncoderParameters object.
            // An EncoderParameters object has an array of EncoderParameter
            // objects. In this case, there is only one
            // EncoderParameter object in the array.
            EncoderParameters myEncoderParameters = new EncoderParameters(1);

            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 50L);
            myEncoderParameters.Param[0] = myEncoderParameter;
            imgBitmap.Save(ms, jpgEncoder, myEncoderParameters);
            ms.Position = 0; // this is important
            return Image.FromStream(ms, true);
        }

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        public static byte[] ResizeImage(byte[] imageBytes, int width, int height, bool zoom = false)
        {
            MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
            ms.Position = 0; // this is important
            Image image = Image.FromStream(ms, true);

            Image destImage = ResizeImage(image, width, height);

            if (zoom)
            {
                destImage = ResizeImage(destImage, width, height, 50);
                destImage = ResizeImage(destImage, width, height);
            }

            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(destImage, typeof(byte[]));

        }



        public static void SendMailAPI(string Subject, string Body, string ToAddress = "", string FromAddress = "", string CCAddress = "", string BCCAddress = "")
        {

            if (ToAddress == "") ToAddress = ConfigurationManager.AppSettings["supportemail"].ToString();
            if (FromAddress == "") FromAddress = ConfigurationManager.AppSettings["emailfrom"].ToString();
            if (CCAddress == "") CCAddress = ConfigurationManager.AppSettings["emailcc"].ToString();
            if (BCCAddress == "") BCCAddress = ConfigurationManager.AppSettings["emailbcc"].ToString();

            string SendInBlueEndpoint = ConfigurationManager.AppSettings["SendInBlueEndpoint"];
            string SendInBlueKey = ConfigurationManager.AppSettings["SendInBlueKey"];

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(SendInBlueEndpoint + "smtp/email");
            request.ContentType = "application/json";
            request.Headers.Add("api-key", SendInBlueKey);
            request.Method = "POST";

            JObject reqBody = new JObject();
            JObject User = new JObject();
            JArray Recepients = new JArray();

            if (FromAddress.Contains('|'))
            {
                User.Add(new JProperty("name", FromAddress.Split('|')[0].ToString()));
                User.Add(new JProperty("email", FromAddress.Split('|')[1].ToString()));
            }
            else
            {
                User.Add(new JProperty("email", FromAddress));
            }

            reqBody.Add("sender", User);

            foreach (string address in ToAddress.Split(';'))
                if (!address.Contains("dummy.com"))
                {
                    User = new JObject();
                    User.Add(new JProperty("email", address));
                    Recepients.Add(User);
                }

            reqBody.Add("to", Recepients);


            if (BCCAddress != "")
            {
                Recepients = new JArray();
                foreach (string address in BCCAddress.Split(';'))
                {
                    User = new JObject();
                    User.Add(new JProperty("email", address));
                    Recepients.Add(User);
                }
                reqBody.Add("bcc", Recepients);
            }



            if (CCAddress != "")
            {
                Recepients = new JArray();
                foreach (string address in CCAddress.Split(';'))
                {
                    User = new JObject();
                    User.Add(new JProperty("email", address));
                    Recepients.Add(User);
                }
                reqBody.Add("cc", Recepients);
            }

            reqBody.Add("htmlContent", Body);
            reqBody.Add("subject", Subject);

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(reqBody.ToString());
                streamWriter.Flush();
                streamWriter.Close();
            }
            var httpResponse = (HttpWebResponse)request.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

            }

        }

        public static void SendMail(string Subject, string Body, string ToAddress = "", string FromAddress = "", string CCAddress = "", string BCCAddress = "")
        {
            if (ToAddress == "") ToAddress = ConfigurationManager.AppSettings["supportemail"].ToString();
            if (FromAddress == "") FromAddress = ConfigurationManager.AppSettings["emailfrom"].ToString();
            if (CCAddress == "") CCAddress = ConfigurationManager.AppSettings["emailcc"].ToString();
            if (BCCAddress == "") BCCAddress = ConfigurationManager.AppSettings["emailbcc"].ToString();

            string smtp = ConfigurationManager.AppSettings["smtp"];
            string smtplogin = ConfigurationManager.AppSettings["smtplogin"];
            string smtppwd = ConfigurationManager.AppSettings["smtppwd"];
            bool isSSL = Convert.ToBoolean(ConfigurationManager.AppSettings["isssl"]);
            int port = Convert.ToInt16(ConfigurationManager.AppSettings["smtpport"].ToString());

            MailMessage mail = new MailMessage();
            mail.IsBodyHtml = true;
            if (FromAddress.Contains('|'))
            {
                mail.From = new MailAddress(FromAddress.Split('|')[1].ToString(), FromAddress.Split('|')[0].ToString());
            }
            else
            {
                mail.From = new MailAddress(FromAddress);
            }
            foreach (string address in ToAddress.Split(';'))
                if (!address.Contains("dummy.com"))
                    mail.To.Add(address);

            if (mail.To.Count > 0)
            {
                if (BCCAddress != "") foreach (string address in BCCAddress.Split(';')) mail.Bcc.Add(address);
                if (CCAddress != "") foreach (string address in CCAddress.Split(';')) mail.CC.Add(address);
                mail.Subject = Subject;
                mail.Body = Body;
                mail.Priority = MailPriority.High;

                SmtpClient client = new SmtpClient(smtp, port);
                client.EnableSsl = isSSL;
                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential(smtplogin, smtppwd);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Send(mail);
            }
        }

        public static void SendSMS(string mobileno, string text, string smssenderid)
        {

            string smsuserid = ConfigurationManager.AppSettings["smsuserid"];
            string smspassword = ConfigurationManager.AppSettings["smspassword"];


            string GetSMSURI = String.Format("http://login.arihantsms.com/vendorsms/pushsms.aspx?user={0}&password={1}&msisdn={2}&sid={3}&msg={4}&fl=0&gwid=2", smsuserid, smspassword, mobileno, smssenderid, text);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(GetSMSURI);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            var rawJson = new StreamReader(response.GetResponseStream()).ReadToEnd();

            dynamic json = JsonConvert.DeserializeObject(rawJson);

            string result = Convert.ToString(json.ErrorCode);

            if (result != "000")
            {
                SendMail("ResQuark WebApp - Error sending sms", "URL of SMS API <br/><br/>" + GetSMSURI + "<br/><br/>Got result as below <br/><br/>" + rawJson);
            }

        }



        public static string Generatehash512(string text)
        {

            byte[] message = Encoding.UTF8.GetBytes(text);

            UnicodeEncoding UE = new UnicodeEncoding();
            byte[] hashValue;
            SHA512Managed hashString = new SHA512Managed();
            string hex = "";
            hashValue = hashString.ComputeHash(message);
            foreach (byte x in hashValue)
            {
                hex += String.Format("{0:x2}", x);
            }
            return hex;

        }

        public static string AmountInWords(decimal Amount)
        {
            string strAmount = Amount.ToString("0.00");
            string Rupees = strAmount.Split('.')[0];
            string Paises = strAmount.Split('.')[1];

            return NumbersToWords(Convert.ToInt32(Rupees)) + " Rupees " + NumbersToWords(Convert.ToInt32(Paises)) + " Paise";

        }

        private static string NumbersToWords(int inputNumber)
        {
            int inputNo = inputNumber;

            if (inputNo == 0)
                return "Zero";

            int[] numbers = new int[4];
            int first = 0;
            int u, h, t;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            if (inputNo < 0)
            {
                sb.Append("Minus ");
                inputNo = -inputNo;
            }

            string[] words0 = {"" ,"One ", "Two ", "Three ", "Four ",
            "Five " ,"Six ", "Seven ", "Eight ", "Nine "};
            string[] words1 = {"Ten ", "Eleven ", "Twelve ", "Thirteen ", "Fourteen ",
            "Fifteen ","Sixteen ","Seventeen ","Eighteen ", "Nineteen "};
            string[] words2 = {"Twenty ", "Thirty ", "Forty ", "Fifty ", "Sixty ",
            "Seventy ","Eighty ", "Ninety "};
            string[] words3 = { "Thousand ", "Lakh ", "Crore " };

            numbers[0] = inputNo % 1000; // units
            numbers[1] = inputNo / 1000;
            numbers[2] = inputNo / 100000;
            numbers[1] = numbers[1] - 100 * numbers[2]; // thousands
            numbers[3] = inputNo / 10000000; // crores
            numbers[2] = numbers[2] - 100 * numbers[3]; // lakhs

            for (int i = 3; i > 0; i--)
            {
                if (numbers[i] != 0)
                {
                    first = i;
                    break;
                }
            }
            for (int i = first; i >= 0; i--)
            {
                if (numbers[i] == 0) continue;
                u = numbers[i] % 10; // ones
                t = numbers[i] / 10;
                h = numbers[i] / 100; // hundreds
                t = t - 10 * h; // tens
                if (h > 0) sb.Append(words0[h] + "Hundred ");
                if (u > 0 || t > 0)
                {
                    if (h > 0 || i == 0) sb.Append("");
                    if (t == 0)
                        sb.Append(words0[u]);
                    else if (t == 1)
                        sb.Append(words1[u]);
                    else
                        sb.Append(words2[t - 2] + words0[u]);
                }
                if (i != 0) sb.Append(words3[i - 1]);
            }
            return sb.ToString().TrimEnd();
        }
        public static string GetComplaintStatus(int value) 
        {
            switch (value) 
            {
                case 2: 
                    {
                        return "Verified";
#pragma warning disable CS0162 // Unreachable code detected
                        break;
#pragma warning restore CS0162 // Unreachable code detected
                    }
                case 3:
                    {
                        return "Rejected";
#pragma warning disable CS0162 // Unreachable code detected
                        break;
#pragma warning restore CS0162 // Unreachable code detected
                    }
                case 4:
                    {
                        return "Registered";
#pragma warning disable CS0162 // Unreachable code detected
                        break;
#pragma warning restore CS0162 // Unreachable code detected
                    }
                default:
                case 1: 
                    {
                        return "Pending";
#pragma warning disable CS0162 // Unreachable code detected
                        break;
#pragma warning restore CS0162 // Unreachable code detected
                    }
            }
        }

        public static AspNetUsers GetUserName(string UserId) 
        {
            using (SSCEntities db=new SSCEntities()) 
            {
                var user = db.AspNetUsers.Find(UserId);
                return user;
            }
           
        }
        public static string GetDatestring(DateTime dateTime)
        {
            string date = "";
            var Day = Convert.ToString(dateTime.Day);
            var Month = Convert.ToString(dateTime.Month);
            var year = Convert.ToString(dateTime.Year);

            if (Convert.ToInt32(Month) <= 9)
            {
                Month = Convert.ToString("0" + Month);
            }
            if (Convert.ToInt32(Day) < 10 && Convert.ToInt32(Day) >= 1)
            {
                Day = Convert.ToString("0" + Day);
            }
            date = Convert.ToString(year + "-" + Month + "-" + Day);
            return date;
        }

        //public static string GetPayUTxnID()
        //{
        //    SurtiEntities db = new SurtiEntities();

        //    Random rnd = new Random();
        //    string strHash = common.Generatehash512(rnd.ToString() + DateTime.Now);
        //    string transactionid = strHash.ToString().Substring(0, 20);
        //    transactionid = new string(transactionid.ToCharArray().OrderBy(s => (rnd.Next(2) % 2) == 0).ToArray());
        //    if (db.PaymentTransactions.Where(a => a.txnid == transactionid).ToList<PaymentTransactions>().Count > 0)
        //        return GetPayUTxnID();
        //    else
        //        return transactionid;
        //}
    }
}