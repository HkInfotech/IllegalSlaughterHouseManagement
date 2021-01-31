using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace SSCWebApi.Helper
{
    public static class common
    {
        
        public static DateTime GetCurrentDateTime(int Companyid)
        {
            var DateDiffrentMinute = Convert.ToDouble(ConfigurationManager.AppSettings["DateDiffrentMinute"]);

            DateTime ReturnDate = DateTime.Now.AddMinutes(DateDiffrentMinute);

            return ReturnDate;
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

        public static string GenerateNewPassword(int PasswordLen = 6)
        {
            string allowedChars = "";
            allowedChars = "a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z,";
            allowedChars += "A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z,";
            allowedChars += "1,2,3,4,5,6,7,8,9,0,!,@,#,$,%,&,?";

            char[] sep = { ',' };

            string[] arr = allowedChars.Split(sep);

            string passwordString = "";

            string temp = "";

            Random rand = new Random();

            for (int i = 0; i < PasswordLen; i++)
            {
                temp = arr[rand.Next(0, arr.Length)];
                passwordString += temp;
            }
            return passwordString;
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
        public static string RandomNumber(int min=000000, int max=999999)
        {
            Random random = new Random();
            
            return Convert.ToString(random.Next(min, max));
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

        //public static IDictionary<string, string> ToDictionary(this NameValueCollection source)
        //{
        //    return source.AllKeys.ToDictionary(k => k, k => source[k]);
        //}

        public static void SendMail(string Subject, string Body, string ToAddress = "", string FromAddress = "", string CCAddress = "", string BCCAddress = "")
        {
            if (ToAddress == "") ToAddress = ConfigurationManager.AppSettings["supportemail"].ToString();
            if (FromAddress == "") FromAddress = ConfigurationManager.AppSettings["emailfrom"].ToString();
            if (CCAddress == "") CCAddress = ConfigurationManager.AppSettings["emailcc"].ToString();
            if (BCCAddress == "") BCCAddress = ConfigurationManager.AppSettings["emailbcc"].ToString();


            string smtp = ConfigurationManager.AppSettings["smtp"];
            string smtplogin = ConfigurationManager.AppSettings["smtplogin"];
            string smtppwd = ConfigurationManager.AppSettings["smtppwd"];
            int port = Convert.ToInt16(ConfigurationManager.AppSettings["smtpport"].ToString());

            MailMessage mail = new MailMessage();

            mail.IsBodyHtml = true;
            mail.From = new MailAddress(FromAddress);
            foreach (string address in ToAddress.Split(';')) mail.To.Add(address);
            if (BCCAddress != "") foreach (string address in BCCAddress.Split(';')) mail.Bcc.Add(address);
            if (CCAddress != "") foreach (string address in CCAddress.Split(';')) mail.CC.Add(address);
            mail.Subject = Subject;
            mail.Body = Body;
            mail.Priority = MailPriority.High;

            SmtpClient client = new SmtpClient(smtp, port);
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential(smtplogin, smtppwd);
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Send(mail);
        }

        public static void SendMailWithAttechMent(string Subject, string Body, string ToAddress = "", string FromAddress = "", string CCAddress = "", string BCCAddress = "")
        {
            if (ToAddress == "") ToAddress = ConfigurationManager.AppSettings["supportemail"].ToString();
            if (FromAddress == "") FromAddress = ConfigurationManager.AppSettings["emailfrom"].ToString();
            if (CCAddress == "") CCAddress = ConfigurationManager.AppSettings["emailcc"].ToString();
            if (BCCAddress == "") BCCAddress = ConfigurationManager.AppSettings["emailbcc"].ToString();


            string smtp = ConfigurationManager.AppSettings["smtp"];
            string smtplogin = ConfigurationManager.AppSettings["smtplogin"];
            string smtppwd = ConfigurationManager.AppSettings["smtppwd"];
            int port = Convert.ToInt16(ConfigurationManager.AppSettings["smtpport"].ToString());

            MailMessage mail = new MailMessage();

            mail.IsBodyHtml = true;
            mail.From = new MailAddress(FromAddress);
            foreach (string address in ToAddress.Split(';')) mail.To.Add(address);
            if (BCCAddress != "") foreach (string address in BCCAddress.Split(';')) mail.Bcc.Add(address);
            if (CCAddress != "") foreach (string address in CCAddress.Split(';')) mail.CC.Add(address);
            mail.Subject = Subject;
            mail.Body = Body;
            mail.Priority = MailPriority.High;

            SmtpClient client = new SmtpClient(smtp, port);
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential(smtplogin, smtppwd);
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Send(mail);
        }

        public static void SendSMS(string mobileno, string text)
        {

            string smsuserid = ConfigurationManager.AppSettings["smsuserid"];
            string smspassword = ConfigurationManager.AppSettings["smspassword"];
            string smssenderid = ConfigurationManager.AppSettings["smssenderid"];


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

        private static IDictionary<string, string> _mappings
            = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase) {
        #region Big freaking list of mime types
        {".323", "text/h323"},
        {".3g2", "video/3gpp2"},
        {".3gp", "video/3gpp"},
        {".3gp2", "video/3gpp2"},
        {".3gpp", "video/3gpp"},
        {".7z", "application/x-7z-compressed"},
        {".aa", "audio/audible"},
        {".AAC", "audio/aac"},
        {".aaf", "application/octet-stream"},
        {".aax", "audio/vnd.audible.aax"},
        {".ac3", "audio/ac3"},
        {".aca", "application/octet-stream"},
        {".accda", "application/msaccess.addin"},
        {".accdb", "application/msaccess"},
        {".accdc", "application/msaccess.cab"},
        {".accde", "application/msaccess"},
        {".accdr", "application/msaccess.runtime"},
        {".accdt", "application/msaccess"},
        {".accdw", "application/msaccess.webapplication"},
        {".accft", "application/msaccess.ftemplate"},
        {".acx", "application/internet-property-stream"},
        {".AddIn", "text/xml"},
        {".ade", "application/msaccess"},
        {".adobebridge", "application/x-bridge-url"},
        {".adp", "application/msaccess"},
        {".ADT", "audio/vnd.dlna.adts"},
        {".ADTS", "audio/aac"},
        {".afm", "application/octet-stream"},
        {".ai", "application/postscript"},
        {".aif", "audio/x-aiff"},
        {".aifc", "audio/aiff"},
        {".aiff", "audio/aiff"},
        {".air", "application/vnd.adobe.air-application-installer-package+zip"},
        {".amc", "application/x-mpeg"},
        {".application", "application/x-ms-application"},
        {".art", "image/x-jg"},
        {".asa", "application/xml"},
        {".asax", "application/xml"},
        {".ascx", "application/xml"},
        {".asd", "application/octet-stream"},
        {".asf", "video/x-ms-asf"},
        {".ashx", "application/xml"},
        {".asi", "application/octet-stream"},
        {".asm", "text/plain"},
        {".asmx", "application/xml"},
        {".aspx", "application/xml"},
        {".asr", "video/x-ms-asf"},
        {".asx", "video/x-ms-asf"},
        {".atom", "application/atom+xml"},
        {".au", "audio/basic"},
        {".avi", "video/x-msvideo"},
        {".axs", "application/olescript"},
        {".bas", "text/plain"},
        {".bcpio", "application/x-bcpio"},
        {".bin", "application/octet-stream"},
        {".bmp", "image/bmp"},
        {".c", "text/plain"},
        {".cab", "application/octet-stream"},
        {".caf", "audio/x-caf"},
        {".calx", "application/vnd.ms-office.calx"},
        {".cat", "application/vnd.ms-pki.seccat"},
        {".cc", "text/plain"},
        {".cd", "text/plain"},
        {".cdda", "audio/aiff"},
        {".cdf", "application/x-cdf"},
        {".cer", "application/x-x509-ca-cert"},
        {".chm", "application/octet-stream"},
        {".class", "application/x-java-applet"},
        {".clp", "application/x-msclip"},
        {".cmx", "image/x-cmx"},
        {".cnf", "text/plain"},
        {".cod", "image/cis-cod"},
        {".config", "application/xml"},
        {".contact", "text/x-ms-contact"},
        {".coverage", "application/xml"},
        {".cpio", "application/x-cpio"},
        {".cpp", "text/plain"},
        {".crd", "application/x-mscardfile"},
        {".crl", "application/pkix-crl"},
        {".crt", "application/x-x509-ca-cert"},
        {".cs", "text/plain"},
        {".csdproj", "text/plain"},
        {".csh", "application/x-csh"},
        {".csproj", "text/plain"},
        {".css", "text/css"},
        {".csv", "text/csv"},
        {".cur", "application/octet-stream"},
        {".cxx", "text/plain"},
        {".dat", "application/octet-stream"},
        {".datasource", "application/xml"},
        {".dbproj", "text/plain"},
        {".dcr", "application/x-director"},
        {".def", "text/plain"},
        {".deploy", "application/octet-stream"},
        {".der", "application/x-x509-ca-cert"},
        {".dgml", "application/xml"},
        {".dib", "image/bmp"},
        {".dif", "video/x-dv"},
        {".dir", "application/x-director"},
        {".disco", "text/xml"},
        {".dll", "application/x-msdownload"},
        {".dll.config", "text/xml"},
        {".dlm", "text/dlm"},
        {".doc", "application/msword"},
        {".docm", "application/vnd.ms-word.document.macroEnabled.12"},
        {".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
        {".dot", "application/msword"},
        {".dotm", "application/vnd.ms-word.template.macroEnabled.12"},
        {".dotx", "application/vnd.openxmlformats-officedocument.wordprocessingml.template"},
        {".dsp", "application/octet-stream"},
        {".dsw", "text/plain"},
        {".dtd", "text/xml"},
        {".dtsConfig", "text/xml"},
        {".dv", "video/x-dv"},
        {".dvi", "application/x-dvi"},
        {".dwf", "drawing/x-dwf"},
        {".dwp", "application/octet-stream"},
        {".dxr", "application/x-director"},
        {".eml", "message/rfc822"},
        {".emz", "application/octet-stream"},
        {".eot", "application/octet-stream"},
        {".eps", "application/postscript"},
        {".etl", "application/etl"},
        {".etx", "text/x-setext"},
        {".evy", "application/envoy"},
        {".exe", "application/octet-stream"},
        {".exe.config", "text/xml"},
        {".fdf", "application/vnd.fdf"},
        {".fif", "application/fractals"},
        {".filters", "Application/xml"},
        {".fla", "application/octet-stream"},
        {".flr", "x-world/x-vrml"},
        {".flv", "video/x-flv"},
        {".fsscript", "application/fsharp-script"},
        {".fsx", "application/fsharp-script"},
        {".generictest", "application/xml"},
        {".gif", "image/gif"},
        {".group", "text/x-ms-group"},
        {".gsm", "audio/x-gsm"},
        {".gtar", "application/x-gtar"},
        {".gz", "application/x-gzip"},
        {".h", "text/plain"},
        {".hdf", "application/x-hdf"},
        {".hdml", "text/x-hdml"},
        {".hhc", "application/x-oleobject"},
        {".hhk", "application/octet-stream"},
        {".hhp", "application/octet-stream"},
        {".hlp", "application/winhlp"},
        {".hpp", "text/plain"},
        {".hqx", "application/mac-binhex40"},
        {".hta", "application/hta"},
        {".htc", "text/x-component"},
        {".htm", "text/html"},
        {".html", "text/html"},
        {".htt", "text/webviewhtml"},
        {".hxa", "application/xml"},
        {".hxc", "application/xml"},
        {".hxd", "application/octet-stream"},
        {".hxe", "application/xml"},
        {".hxf", "application/xml"},
        {".hxh", "application/octet-stream"},
        {".hxi", "application/octet-stream"},
        {".hxk", "application/xml"},
        {".hxq", "application/octet-stream"},
        {".hxr", "application/octet-stream"},
        {".hxs", "application/octet-stream"},
        {".hxt", "text/html"},
        {".hxv", "application/xml"},
        {".hxw", "application/octet-stream"},
        {".hxx", "text/plain"},
        {".i", "text/plain"},
        {".ico", "image/x-icon"},
        {".ics", "application/octet-stream"},
        {".idl", "text/plain"},
        {".ief", "image/ief"},
        {".iii", "application/x-iphone"},
        {".inc", "text/plain"},
        {".inf", "application/octet-stream"},
        {".inl", "text/plain"},
        {".ins", "application/x-internet-signup"},
        {".ipa", "application/x-itunes-ipa"},
        {".ipg", "application/x-itunes-ipg"},
        {".ipproj", "text/plain"},
        {".ipsw", "application/x-itunes-ipsw"},
        {".iqy", "text/x-ms-iqy"},
        {".isp", "application/x-internet-signup"},
        {".ite", "application/x-itunes-ite"},
        {".itlp", "application/x-itunes-itlp"},
        {".itms", "application/x-itunes-itms"},
        {".itpc", "application/x-itunes-itpc"},
        {".IVF", "video/x-ivf"},
        {".jar", "application/java-archive"},
        {".java", "application/octet-stream"},
        {".jck", "application/liquidmotion"},
        {".jcz", "application/liquidmotion"},
        {".jfif", "image/pjpeg"},
        {".jnlp", "application/x-java-jnlp-file"},
        {".jpb", "application/octet-stream"},
        {".jpe", "image/jpeg"},
        {".jpeg", "image/jpeg"},
        {".jpg", "image/jpeg"},
        {".js", "application/x-javascript"},
        {".json", "application/json"},
        {".jsx", "text/jscript"},
        {".jsxbin", "text/plain"},
        {".latex", "application/x-latex"},
        {".library-ms", "application/windows-library+xml"},
        {".lit", "application/x-ms-reader"},
        {".loadtest", "application/xml"},
        {".lpk", "application/octet-stream"},
        {".lsf", "video/x-la-asf"},
        {".lst", "text/plain"},
        {".lsx", "video/x-la-asf"},
        {".lzh", "application/octet-stream"},
        {".m13", "application/x-msmediaview"},
        {".m14", "application/x-msmediaview"},
        {".m1v", "video/mpeg"},
        {".m2t", "video/vnd.dlna.mpeg-tts"},
        {".m2ts", "video/vnd.dlna.mpeg-tts"},
        {".m2v", "video/mpeg"},
        {".m3u", "audio/x-mpegurl"},
        {".m3u8", "audio/x-mpegurl"},
        {".m4a", "audio/m4a"},
        {".m4b", "audio/m4b"},
        {".m4p", "audio/m4p"},
        {".m4r", "audio/x-m4r"},
        {".m4v", "video/x-m4v"},
        {".mac", "image/x-macpaint"},
        {".mak", "text/plain"},
        {".man", "application/x-troff-man"},
        {".manifest", "application/x-ms-manifest"},
        {".map", "text/plain"},
        {".master", "application/xml"},
        {".mda", "application/msaccess"},
        {".mdb", "application/x-msaccess"},
        {".mde", "application/msaccess"},
        {".mdp", "application/octet-stream"},
        {".me", "application/x-troff-me"},
        {".mfp", "application/x-shockwave-flash"},
        {".mht", "message/rfc822"},
        {".mhtml", "message/rfc822"},
        {".mid", "audio/mid"},
        {".midi", "audio/mid"},
        {".mix", "application/octet-stream"},
        {".mk", "text/plain"},
        {".mmf", "application/x-smaf"},
        {".mno", "text/xml"},
        {".mny", "application/x-msmoney"},
        {".mod", "video/mpeg"},
        {".mov", "video/quicktime"},
        {".movie", "video/x-sgi-movie"},
        {".mp2", "video/mpeg"},
        {".mp2v", "video/mpeg"},
        {".mp3", "audio/mpeg"},
        {".mp4", "video/mp4"},
        {".mp4v", "video/mp4"},
        {".mpa", "video/mpeg"},
        {".mpe", "video/mpeg"},
        {".mpeg", "video/mpeg"},
        {".mpf", "application/vnd.ms-mediapackage"},
        {".mpg", "video/mpeg"},
        {".mpp", "application/vnd.ms-project"},
        {".mpv2", "video/mpeg"},
        {".mqv", "video/quicktime"},
        {".ms", "application/x-troff-ms"},
        {".msi", "application/octet-stream"},
        {".mso", "application/octet-stream"},
        {".mts", "video/vnd.dlna.mpeg-tts"},
        {".mtx", "application/xml"},
        {".mvb", "application/x-msmediaview"},
        {".mvc", "application/x-miva-compiled"},
        {".mxp", "application/x-mmxp"},
        {".nc", "application/x-netcdf"},
        {".nsc", "video/x-ms-asf"},
        {".nws", "message/rfc822"},
        {".ocx", "application/octet-stream"},
        {".oda", "application/oda"},
        {".odc", "text/x-ms-odc"},
        {".odh", "text/plain"},
        {".odl", "text/plain"},
        {".odp", "application/vnd.oasis.opendocument.presentation"},
        {".ods", "application/oleobject"},
        {".odt", "application/vnd.oasis.opendocument.text"},
        {".one", "application/onenote"},
        {".onea", "application/onenote"},
        {".onepkg", "application/onenote"},
        {".onetmp", "application/onenote"},
        {".onetoc", "application/onenote"},
        {".onetoc2", "application/onenote"},
        {".orderedtest", "application/xml"},
        {".osdx", "application/opensearchdescription+xml"},
        {".p10", "application/pkcs10"},
        {".p12", "application/x-pkcs12"},
        {".p7b", "application/x-pkcs7-certificates"},
        {".p7c", "application/pkcs7-mime"},
        {".p7m", "application/pkcs7-mime"},
        {".p7r", "application/x-pkcs7-certreqresp"},
        {".p7s", "application/pkcs7-signature"},
        {".pbm", "image/x-portable-bitmap"},
        {".pcast", "application/x-podcast"},
        {".pct", "image/pict"},
        {".pcx", "application/octet-stream"},
        {".pcz", "application/octet-stream"},
        {".pdf", "application/pdf"},
        {".pfb", "application/octet-stream"},
        {".pfm", "application/octet-stream"},
        {".pfx", "application/x-pkcs12"},
        {".pgm", "image/x-portable-graymap"},
        {".pic", "image/pict"},
        {".pict", "image/pict"},
        {".pkgdef", "text/plain"},
        {".pkgundef", "text/plain"},
        {".pko", "application/vnd.ms-pki.pko"},
        {".pls", "audio/scpls"},
        {".pma", "application/x-perfmon"},
        {".pmc", "application/x-perfmon"},
        {".pml", "application/x-perfmon"},
        {".pmr", "application/x-perfmon"},
        {".pmw", "application/x-perfmon"},
        {".png", "image/png"},
        {".pnm", "image/x-portable-anymap"},
        {".pnt", "image/x-macpaint"},
        {".pntg", "image/x-macpaint"},
        {".pnz", "image/png"},
        {".pot", "application/vnd.ms-powerpoint"},
        {".potm", "application/vnd.ms-powerpoint.template.macroEnabled.12"},
        {".potx", "application/vnd.openxmlformats-officedocument.presentationml.template"},
        {".ppa", "application/vnd.ms-powerpoint"},
        {".ppam", "application/vnd.ms-powerpoint.addin.macroEnabled.12"},
        {".ppm", "image/x-portable-pixmap"},
        {".pps", "application/vnd.ms-powerpoint"},
        {".ppsm", "application/vnd.ms-powerpoint.slideshow.macroEnabled.12"},
        {".ppsx", "application/vnd.openxmlformats-officedocument.presentationml.slideshow"},
        {".ppt", "application/vnd.ms-powerpoint"},
        {".pptm", "application/vnd.ms-powerpoint.presentation.macroEnabled.12"},
        {".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation"},
        {".prf", "application/pics-rules"},
        {".prm", "application/octet-stream"},
        {".prx", "application/octet-stream"},
        {".ps", "application/postscript"},
        {".psc1", "application/PowerShell"},
        {".psd", "application/octet-stream"},
        {".psess", "application/xml"},
        {".psm", "application/octet-stream"},
        {".psp", "application/octet-stream"},
        {".pub", "application/x-mspublisher"},
        {".pwz", "application/vnd.ms-powerpoint"},
        {".qht", "text/x-html-insertion"},
        {".qhtm", "text/x-html-insertion"},
        {".qt", "video/quicktime"},
        {".qti", "image/x-quicktime"},
        {".qtif", "image/x-quicktime"},
        {".qtl", "application/x-quicktimeplayer"},
        {".qxd", "application/octet-stream"},
        {".ra", "audio/x-pn-realaudio"},
        {".ram", "audio/x-pn-realaudio"},
        {".rar", "application/octet-stream"},
        {".ras", "image/x-cmu-raster"},
        {".rat", "application/rat-file"},
        {".rc", "text/plain"},
        {".rc2", "text/plain"},
        {".rct", "text/plain"},
        {".rdlc", "application/xml"},
        {".resx", "application/xml"},
        {".rf", "image/vnd.rn-realflash"},
        {".rgb", "image/x-rgb"},
        {".rgs", "text/plain"},
        {".rm", "application/vnd.rn-realmedia"},
        {".rmi", "audio/mid"},
        {".rmp", "application/vnd.rn-rn_music_package"},
        {".roff", "application/x-troff"},
        {".rpm", "audio/x-pn-realaudio-plugin"},
        {".rqy", "text/x-ms-rqy"},
        {".rtf", "application/rtf"},
        {".rtx", "text/richtext"},
        {".ruleset", "application/xml"},
        {".s", "text/plain"},
        {".safariextz", "application/x-safari-safariextz"},
        {".scd", "application/x-msschedule"},
        {".sct", "text/scriptlet"},
        {".sd2", "audio/x-sd2"},
        {".sdp", "application/sdp"},
        {".sea", "application/octet-stream"},
        {".searchConnector-ms", "application/windows-search-connector+xml"},
        {".setpay", "application/set-payment-initiation"},
        {".setreg", "application/set-registration-initiation"},
        {".settings", "application/xml"},
        {".sgimb", "application/x-sgimb"},
        {".sgml", "text/sgml"},
        {".sh", "application/x-sh"},
        {".shar", "application/x-shar"},
        {".shtml", "text/html"},
        {".sit", "application/x-stuffit"},
        {".sitemap", "application/xml"},
        {".skin", "application/xml"},
        {".sldm", "application/vnd.ms-powerpoint.slide.macroEnabled.12"},
        {".sldx", "application/vnd.openxmlformats-officedocument.presentationml.slide"},
        {".slk", "application/vnd.ms-excel"},
        {".sln", "text/plain"},
        {".slupkg-ms", "application/x-ms-license"},
        {".smd", "audio/x-smd"},
        {".smi", "application/octet-stream"},
        {".smx", "audio/x-smd"},
        {".smz", "audio/x-smd"},
        {".snd", "audio/basic"},
        {".snippet", "application/xml"},
        {".snp", "application/octet-stream"},
        {".sol", "text/plain"},
        {".sor", "text/plain"},
        {".spc", "application/x-pkcs7-certificates"},
        {".spl", "application/futuresplash"},
        {".src", "application/x-wais-source"},
        {".srf", "text/plain"},
        {".SSISDeploymentManifest", "text/xml"},
        {".ssm", "application/streamingmedia"},
        {".sst", "application/vnd.ms-pki.certstore"},
        {".stl", "application/vnd.ms-pki.stl"},
        {".sv4cpio", "application/x-sv4cpio"},
        {".sv4crc", "application/x-sv4crc"},
        {".svc", "application/xml"},
        {".swf", "application/x-shockwave-flash"},
        {".t", "application/x-troff"},
        {".tar", "application/x-tar"},
        {".tcl", "application/x-tcl"},
        {".testrunconfig", "application/xml"},
        {".testsettings", "application/xml"},
        {".tex", "application/x-tex"},
        {".texi", "application/x-texinfo"},
        {".texinfo", "application/x-texinfo"},
        {".tgz", "application/x-compressed"},
        {".thmx", "application/vnd.ms-officetheme"},
        {".thn", "application/octet-stream"},
        {".tif", "image/tiff"},
        {".tiff", "image/tiff"},
        {".tlh", "text/plain"},
        {".tli", "text/plain"},
        {".toc", "application/octet-stream"},
        {".tr", "application/x-troff"},
        {".trm", "application/x-msterminal"},
        {".trx", "application/xml"},
        {".ts", "video/vnd.dlna.mpeg-tts"},
        {".tsv", "text/tab-separated-values"},
        {".ttf", "application/octet-stream"},
        {".tts", "video/vnd.dlna.mpeg-tts"},
        {".txt", "text/plain"},
        {".u32", "application/octet-stream"},
        {".uls", "text/iuls"},
        {".user", "text/plain"},
        {".ustar", "application/x-ustar"},
        {".vb", "text/plain"},
        {".vbdproj", "text/plain"},
        {".vbk", "video/mpeg"},
        {".vbproj", "text/plain"},
        {".vbs", "text/vbscript"},
        {".vcf", "text/x-vcard"},
        {".vcproj", "Application/xml"},
        {".vcs", "text/plain"},
        {".vcxproj", "Application/xml"},
        {".vddproj", "text/plain"},
        {".vdp", "text/plain"},
        {".vdproj", "text/plain"},
        {".vdx", "application/vnd.ms-visio.viewer"},
        {".vml", "text/xml"},
        {".vscontent", "application/xml"},
        {".vsct", "text/xml"},
        {".vsd", "application/vnd.visio"},
        {".vsi", "application/ms-vsi"},
        {".vsix", "application/vsix"},
        {".vsixlangpack", "text/xml"},
        {".vsixmanifest", "text/xml"},
        {".vsmdi", "application/xml"},
        {".vspscc", "text/plain"},
        {".vss", "application/vnd.visio"},
        {".vsscc", "text/plain"},
        {".vssettings", "text/xml"},
        {".vssscc", "text/plain"},
        {".vst", "application/vnd.visio"},
        {".vstemplate", "text/xml"},
        {".vsto", "application/x-ms-vsto"},
        {".vsw", "application/vnd.visio"},
        {".vsx", "application/vnd.visio"},
        {".vtx", "application/vnd.visio"},
        {".wav", "audio/wav"},
        {".wave", "audio/wav"},
        {".wax", "audio/x-ms-wax"},
        {".wbk", "application/msword"},
        {".wbmp", "image/vnd.wap.wbmp"},
        {".wcm", "application/vnd.ms-works"},
        {".wdb", "application/vnd.ms-works"},
        {".wdp", "image/vnd.ms-photo"},
        {".webarchive", "application/x-safari-webarchive"},
        {".webtest", "application/xml"},
        {".wiq", "application/xml"},
        {".wiz", "application/msword"},
        {".wks", "application/vnd.ms-works"},
        {".WLMP", "application/wlmoviemaker"},
        {".wlpginstall", "application/x-wlpg-detect"},
        {".wlpginstall3", "application/x-wlpg3-detect"},
        {".wm", "video/x-ms-wm"},
        {".wma", "audio/x-ms-wma"},
        {".wmd", "application/x-ms-wmd"},
        {".wmf", "application/x-msmetafile"},
        {".wml", "text/vnd.wap.wml"},
        {".wmlc", "application/vnd.wap.wmlc"},
        {".wmls", "text/vnd.wap.wmlscript"},
        {".wmlsc", "application/vnd.wap.wmlscriptc"},
        {".wmp", "video/x-ms-wmp"},
        {".wmv", "video/x-ms-wmv"},
        {".wmx", "video/x-ms-wmx"},
        {".wmz", "application/x-ms-wmz"},
        {".wpl", "application/vnd.ms-wpl"},
        {".wps", "application/vnd.ms-works"},
        {".wri", "application/x-mswrite"},
        {".wrl", "x-world/x-vrml"},
        {".wrz", "x-world/x-vrml"},
        {".wsc", "text/scriptlet"},
        {".wsdl", "text/xml"},
        {".wvx", "video/x-ms-wvx"},
        {".x", "application/directx"},
        {".xaf", "x-world/x-vrml"},
        {".xaml", "application/xaml+xml"},
        {".xap", "application/x-silverlight-app"},
        {".xbap", "application/x-ms-xbap"},
        {".xbm", "image/x-xbitmap"},
        {".xdr", "text/plain"},
        {".xht", "application/xhtml+xml"},
        {".xhtml", "application/xhtml+xml"},
        {".xla", "application/vnd.ms-excel"},
        {".xlam", "application/vnd.ms-excel.addin.macroEnabled.12"},
        {".xlc", "application/vnd.ms-excel"},
        {".xld", "application/vnd.ms-excel"},
        {".xlk", "application/vnd.ms-excel"},
        {".xll", "application/vnd.ms-excel"},
        {".xlm", "application/vnd.ms-excel"},
        {".xls", "application/vnd.ms-excel"},
        {".xlsb", "application/vnd.ms-excel.sheet.binary.macroEnabled.12"},
        {".xlsm", "application/vnd.ms-excel.sheet.macroEnabled.12"},
        {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
        {".xlt", "application/vnd.ms-excel"},
        {".xltm", "application/vnd.ms-excel.template.macroEnabled.12"},
        {".xltx", "application/vnd.openxmlformats-officedocument.spreadsheetml.template"},
        {".xlw", "application/vnd.ms-excel"},
        {".xml", "text/xml"},
        {".xmta", "application/xml"},
        {".xof", "x-world/x-vrml"},
        {".XOML", "text/plain"},
        {".xpm", "image/x-xpixmap"},
        {".xps", "application/vnd.ms-xpsdocument"},
        {".xrm-ms", "text/xml"},
        {".xsc", "application/xml"},
        {".xsd", "text/xml"},
        {".xsf", "text/xml"},
        {".xsl", "text/xml"},
        {".xslt", "text/xml"},
        {".xsn", "application/octet-stream"},
        {".xss", "application/xml"},
        {".xtp", "application/octet-stream"},
        {".xwd", "image/x-xwindowdump"},
        {".z", "application/x-compress"},
        {".zip", "application/x-zip-compressed"},
                #endregion
            };

        public static string GetExtension(string fileName)
        {
            try
            {
                return fileName.Split('.')[fileName.Split('.').Length - 1];
            }
            catch
            {
                return "";
            }
        }

        public static string GetMimeType(string extension)
        {
            if (extension == null)
            {
                throw new ArgumentNullException("extension");
            }

            if (!extension.StartsWith("."))
            {
                extension = "." + extension;
            }

            string mime;

            return _mappings.TryGetValue(extension, out mime) ? mime : "application/octet-stream";
        }

        public static void AddTaskToAsana(string TaskTitle, string TaskDetail)
        {
            string CurrentWebURI = "";
            string CurrentPostData = "";
            try
            {
                string taskid;

                TaskDetail = TaskDetail.Replace("<br>", "\n");

                DateTime TaskDate = DateTime.Now;
                string DueOn = TaskDate.ToString("yyyy-MM-dd");


                JObject data = new JObject();
                data.Add("name", new JValue(TaskTitle));
                data.Add("notes", new JValue(TaskDetail));
                data.Add("assignee", new JValue(16373282728738));  /// jignesh 
                data.Add("workspace", new JValue(16380297501986));
                data.Add("due_on", new JValue(DueOn));

                JArray followers = new JArray();

                JObject darshan = new JObject();
                darshan.Add("id", 16709455013255);
                followers.Add(darshan);

                JObject jignesh = new JObject();
                jignesh.Add("id", 16373282728738);
                followers.Add(jignesh);

                data.Add("followers", followers);

                JObject task = new JObject();
                task.Add("data", data);

                byte[] postData = Encoding.ASCII.GetBytes(task.ToString(Formatting.None));

                CurrentWebURI = "https://app.asana.com/api/1.0/tasks";
                CurrentPostData = task.ToString(Formatting.None);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(CurrentWebURI);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12;
                request.Method = "Post";
                request.Headers.Add("Authorization", "Bearer 0/0de7eecc399ab2e6968fed0eafdbedd1");
                request.Accept = "application/json";
                request.ContentType = "application/json";
                request.ContentLength = postData.Length;

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(postData, 0, postData.Length);
                }

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                var rawJson = new StreamReader(response.GetResponseStream()).ReadToEnd();

                dynamic json = JsonConvert.DeserializeObject(rawJson);

                taskid = json.data.id;


                JObject project = new JObject();
                project.Add("project", new JValue(173177094731786));  // rescue survey actions

                JObject prData = new JObject();
                prData.Add("data", project);

                byte[] postPrData = Encoding.ASCII.GetBytes(prData.ToString(Formatting.None));

                CurrentWebURI = "https://app.asana.com/api/1.0/tasks/" + taskid + "/addProject";
                CurrentPostData = prData.ToString(Formatting.None);

                HttpWebRequest requestProj = (HttpWebRequest)WebRequest.Create("https://app.asana.com/api/1.0/tasks/" + taskid + "/addProject");
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12;
                requestProj.Method = "Post";
                requestProj.Headers.Add("Authorization", "Bearer 0/0de7eecc399ab2e6968fed0eafdbedd1");
                requestProj.Accept = "application/json";
                requestProj.ContentType = "application/json";
                requestProj.ContentLength = postPrData.Length;
                using (var stream = requestProj.GetRequestStream())
                {
                    stream.Write(postPrData, 0, postPrData.Length);
                }

                HttpWebResponse responseProj = (HttpWebResponse)requestProj.GetResponse();



                JObject data1 = new JObject();
                data1.Add("assignee", new JValue(16709455013255));  /// darshan 


                JObject task1 = new JObject();
                task1.Add("data", data1);

                byte[] postAssData = Encoding.ASCII.GetBytes(task1.ToString(Formatting.None));

                CurrentWebURI = "https://app.asana.com/api/1.0/tasks/" + taskid;
                CurrentPostData = task1.ToString(Formatting.None);

                HttpWebRequest requestAss = (HttpWebRequest)WebRequest.Create("https://app.asana.com/api/1.0/tasks/" + taskid);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12;
                requestAss.Method = "Put";
                requestAss.Headers.Add("Authorization", "Bearer 0/0de7eecc399ab2e6968fed0eafdbedd1");
                requestAss.Accept = "application/json";
                requestAss.ContentType = "application/json";
                requestAss.ContentLength = postAssData.Length;

                using (var stream = requestAss.GetRequestStream())
                {
                    stream.Write(postAssData, 0, postAssData.Length);
                }

                HttpWebResponse responseAss = (HttpWebResponse)requestAss.GetResponse();



            }
            catch (WebException e)
            {

                using (WebResponse response = e.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;

                    using (Stream data = response.GetResponseStream())
                    using (var reader = new StreamReader(data))
                    {
                        string text = reader.ReadToEnd();
                        string ErrString = "URI: " + CurrentWebURI + " \n" + "Data: " + CurrentPostData + " \n" + "Response: " + text;

                        SendMail("Error while creating survey task in Asana", ErrString.Replace("\n", "<br>"));
                    }
                }
            }
#pragma warning disable CS0168 // The variable 'e' is declared but never used
            catch (Exception e) { }
#pragma warning restore CS0168 // The variable 'e' is declared but never used

        }



    }
}   