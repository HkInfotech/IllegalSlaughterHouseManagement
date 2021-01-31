using Acr.UserDialogs;
//using DataServiceBus.OfflineHelper.DataTypes;
//using DataServiceBus.OfflineHelper.DataTypes.Common;
//using DataServiceBus.OnlineHelper.DataTypes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Forms;
//using ConstantsSync = DataServiceBus.OfflineHelper.DataTypes.Common.ConstantsSync;
using System.Globalization;
using Plugin.Permissions;
using Xamarin.Essentials;
using SSCMobileDataBus.OfflineStore.Models.Commons;
using Plugin.DeviceInfo;
using SSCMobile.Helpers;

namespace SSCMobile.Commonfiles
{
    public class HkFunctions
    {

        public static int AppStartCount = 1;
        public static int tempitemid;
        public static Boolean FromHomePage = true;
        public static string UserName = "";
        public static string UserFullName = "";
        public static string InstanceName = "";
        public static Boolean HasTeam = false;
        public static string access_token = "";
        public static int Selected_Instance = 0;
        public static bool IsPWDRemember = false;
        public static bool IsLogin = false;
        public static bool IsLogoutSuccess = false;
        public static string UserLoginName = "";
        public static string UserPassword = "";
      
        public static bool IsShortcutAdded = false;
        public static bool IsShortcutNavigationAvailable = false;

        //public static ObservableCollection<SyncStatus> lstSyncAPIStatus = new ObservableCollection<SyncStatus>()
        //{
        //    new SyncStatus() { APIName = ApiSyncCallTypes.HomeCount },
        //    new SyncStatus() { APIName = ApiSyncCallTypes.CaseOrigination },
        //    new SyncStatus() { APIName = ApiSyncCallTypes.CasesSync },
        //    new SyncStatus() { APIName = ApiSyncCallTypes.CaseListAssignToMe },
        //    new SyncStatus() { APIName = ApiSyncCallTypes.CaseListCreatedByMe },
        //    new SyncStatus() { APIName = ApiSyncCallTypes.CaseListOwnedByMe },
        //    new SyncStatus() { APIName = ApiSyncCallTypes.CaseListAssignedToMyTeam },
        //    new SyncStatus() { APIName = ApiSyncCallTypes.EntitySync },
        //    new SyncStatus() { APIName = ApiSyncCallTypes.EntityAssociatedWithMe },
        //    new SyncStatus() { APIName = ApiSyncCallTypes.QuestSync },
        //    new SyncStatus() { APIName = ApiSyncCallTypes.StandardSync },
        //    new SyncStatus() { APIName = ApiSyncCallTypes.EmployeeSearchSync },
        //    new SyncStatus() { APIName = ApiSyncCallTypes.ExternalDataCases },
        //    new SyncStatus() { APIName = ApiSyncCallTypes.ExternalDataEntity },
        //    new SyncStatus() { APIName = ApiSyncCallTypes.ExternalDataQuest },
        //    new SyncStatus() { APIName = ApiSyncCallTypes.HopperCaseList },
        //};
        public enum BookCategoryTypes
        {
            All, MostPopular, Whatsnew, ForMe
        }

        public static bool IsEditEntity = false;
        public static bool CheckInternetWithAlert()
        {
            bool Isonline = true;
            Page p = new Page();
            Isonline = CrossConnectivity.Current.IsConnected;
            if (!Isonline)
                p.DisplayAlert("Internet Connection!", "Please, Check Your Internet Connectivity!", "Ok");
            return Isonline;
        }
        public static string ToTitle(string data)
        {
            if (!string.IsNullOrEmpty(data))
            {
                TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
                return string.Format(textInfo.ToTitleCase(data.ToLower()));
            }
            return data;
        }
        public static string HTMLToText(string HTMLCode)
        {
            if (HTMLCode == null)
                return "";

            // Remove new lines since they are not visible in HTML
            HTMLCode = HTMLCode.Replace("\r\n", Environment.NewLine);
            HTMLCode = HTMLCode.Replace("\r", Environment.NewLine);
            HTMLCode = HTMLCode.Replace("\n", Environment.NewLine);

            // Remove tab spaces
            HTMLCode = HTMLCode.Replace("\t", " ");

            // Remove multiple white spaces from HTML
            //HTMLCode = Regex.Replace(HTMLCode, "\\s+", " ");

            // Remove HEAD tag
            HTMLCode = Regex.Replace(HTMLCode, "<head.*?</head>", ""
                                , RegexOptions.IgnoreCase | RegexOptions.Singleline);

            // Remove any JavaScript
            HTMLCode = Regex.Replace(HTMLCode, "<script.*?</script>", ""
              , RegexOptions.IgnoreCase | RegexOptions.Singleline);

            // Replace special characters like &, <, >, " etc.
            StringBuilder sbHTML = new StringBuilder(HTMLCode);
            // Note: There are many more special characters, these are just
            // most common. You can add new characters in this arrays if needed
            string[] OldWords = {"&nbsp;", "&amp;", "&quot;", "&lt;",
   "&gt;", "&reg;", "&copy;", "&bull;", "&trade;", "&#39;"};
            string[] NewWords = { " ", "&", "\"", "<", ">", "Â®", "Â©", "â€¢", "â„¢", "'" };
            for (int i = 0; i < OldWords.Length; i++)
            {
                sbHTML.Replace(OldWords[i], NewWords[i]);
            }

            // Check if there are line breaks (<br>) or paragraph (<p>)
            sbHTML.Replace("<br>", Environment.NewLine);
            sbHTML.Replace("<br/>", Environment.NewLine);
            sbHTML.Replace("<br />", Environment.NewLine);
            //sbHTML.Replace("<br ", Environment.NewLine);

            sbHTML.Replace("<p>", Environment.NewLine);
            sbHTML.Replace("<p/>", Environment.NewLine);
            // sbHTML.Replace("<p ", Environment.NewLine);



            // Finally, remove all HTML tags and return plain text
            return Regex.Replace(sbHTML.ToString(), "<[^>]*>", "");
        }

        public static string StripHTML(string source)
        {
            try
            {
                string result;

                // Remove HTML Development formatting
                // Replace line breaks with space
                // because browsers inserts space
                result = source.Replace("\r", " ");
                // Replace line breaks with space
                // because browsers inserts space
                result = result.Replace("\n", " ");
                // Remove step-formatting
                result = result.Replace("\t", string.Empty);
                // Remove repeating spaces because browsers ignore them
                result = System.Text.RegularExpressions.Regex.Replace(result,
                                                                      @"( )+", " ");

                // Remove the header (prepare first by clearing attributes)
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*head([^>])*>", "<head>",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"(<( )*(/)( )*head( )*>)", "</head>",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(<head>).*(</head>)", string.Empty,
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // remove all scripts (prepare first by clearing attributes)
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*script([^>])*>", "<script>",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"(<( )*(/)( )*script( )*>)", "</script>",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                //result = System.Text.RegularExpressions.Regex.Replace(result,
                //         @"(<script>)([^(<script>\.</script>)])*(</script>)",
                //         string.Empty,
                //         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"(<script>).*(</script>)", string.Empty,
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // remove all styles (prepare first by clearing attributes)
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*style([^>])*>", "<style>",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"(<( )*(/)( )*style( )*>)", "</style>",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(<style>).*(</style>)", string.Empty,
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // insert tabs in spaces of <td> tags
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*td([^>])*>", "\t",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // insert line breaks in places of <BR> and <LI> tags
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*br( )*>", "\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*li( )*>", "\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // insert line paragraphs (double line breaks) in place
                // if <P>, <DIV> and <TR> tags
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*div([^>])*>", "\r\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*tr([^>])*>", "\r\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*p([^>])*>", "\r\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // Remove remaining tags like <a>, links, images,
                // comments etc - anything that's enclosed inside < >
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<[^>]*>", string.Empty,
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // replace special characters:
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @" ", " ",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&bull;", " * ",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&lsaquo;", "<",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&rsaquo;", ">",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&trade;", "(tm)",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&frasl;", "/",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&lt;", "<",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&gt;", ">",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&copy;", "(c)",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&reg;", "(r)",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                // Remove all others. More can be added, see
                // http://hotwired.lycos.com/webmonkey/reference/special_characters/
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&(.{2,6});", string.Empty,
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // for testing
                //System.Text.RegularExpressions.Regex.Replace(result,
                //       this.txtRegex.Text,string.Empty,
                //       System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // make line breaking consistent
                result = result.Replace("\n", "\r");

                // Remove extra line breaks and tabs:
                // replace over 2 breaks with 2 and over 4 tabs with 4.
                // Prepare first to remove any whitespaces in between
                // the escaped characters and remove redundant tabs in between line breaks
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\r)( )+(\r)", "\r\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\t)( )+(\t)", "\t\t",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\t)( )+(\r)", "\t\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\r)( )+(\t)", "\r\t",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                // Remove redundant tabs
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\r)(\t)+(\r)", "\r\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                // Remove multiple tabs following a line break with just one tab
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\r)(\t)+", "\r\t",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                // Initial replacement target string for line breaks
                string breaks = "\r\r\r";
                // Initial replacement target string for tabs
                string tabs = "\t\t\t\t\t";
                for (int index = 0; index < result.Length; index++)
                {
                    result = result.Replace(breaks, "\r\r");
                    result = result.Replace(tabs, "\t\t\t\t");
                    breaks = breaks + "\r";
                    tabs = tabs + "\t";
                }

                // That's it.
                return result;
            }
            catch
            {
                //MessageBox.Show("Error");
                return source;
            }
        }


        public static void ShowtoastAlert(string Message)
        {
            //ToastConfig t = new ToastConfig(Message);
            //t.SetDuration(2000);
            //UserDialogs.Instance.Toast(t);
        }



        public static async Task DownloadImageFile(string Url)
        {
            try
            {
                Uri url = new Uri(Url);
                var client = new HttpClient();

                //IFile file = await FileSystem.Current.LocalStorage.CreateFileAsync(UserName + ".png", CreationCollisionOption.ReplaceExisting);
                //using (var fileHandler = await file.OpenAsync(PCLStorage.FileAccess.ReadAndWrite))
                {
                    var httpResponse = await client.GetAsync(url);

                    byte[] dataBuffer = await httpResponse.Content.ReadAsByteArrayAsync();
                    FileExtensions.SaveToLocalFolder(dataBuffer, UserName, "png");
                    //await fileHandler.WriteAsync(dataBuffer, 0, dataBuffer.Length);
                }
            }
            catch (Exception ex)
            {
                //throw ex;
            }
        }

        public static void ShowOverlayView_Grid(ContentView ActInd, bool IsHide, Grid grd)
        {
            ActInd.IsVisible = IsHide;
            grd.Opacity = IsHide ? 0.5 : 1;
        }

        public static void ShowOverlayView_StackLayout(ContentView ActInd, bool IsHide, StackLayout Stack)
        {
            ActInd.IsVisible = IsHide;
            Stack.Opacity = IsHide ? 0.5 : 1;
        }


        // public static void ShowIndicator(ActivityIndicator ActInd, StackLayout Stack_indicator, bool IsHide, StackLayout Mainstack, double Opacity)
        // {
        //     ActInd.IsEnabled = IsHide;
        //     ActInd.IsRunning = IsHide;
        //     Stack_indicator.IsVisible = IsHide;
        //     Mainstack.IsEnabled = !IsHide;
        //     Mainstack.Opacity = Opacity;
        //}

        //  public static void ShowIndicatorUpdate(ActivityIndicator ActInd, StackLayout Stack_indicator, bool IsHide, AbsoluteLayout Mainstack, double Opacity, Page Pg)
        //  {
        //     ActInd.IsEnabled = IsHide;
        //     ActInd.IsRunning = IsHide;
        //     Stack_indicator.IsVisible = IsHide;
        //     Mainstack.IsEnabled = !IsHide;
        //      Mainstack.Opacity = Opacity;
        //      Pg.IsBusy = IsHide;
        //  }

        public static string GetDecodeConnectionString(string Connectionstr)
        {
            string Con = "";
            try
            {
                if (!Connectionstr.Contains("Data Source =") && !Connectionstr.Contains("Data Source="))
                {
                    throw new NotSupportedException();
                    //ar res = ""; // DefaultAPIMethod.Decrypt(Connectionstr);
                    //var temp = res.GetValue("ResponseContent");
                    //if (!string.IsNullOrEmpty(temp?.ToString()) && temp.ToString() != "[]")
                    //{
                    //    Con = temp.ToString();
                    //}
                }
                else
                    Con = Connectionstr;
            }
            catch (Exception)
            {
            }
            return Con;

        }

        //public static void GetSystemCodesfromSqlServerAsync()
        //{
        //    try
        //    {
        //        List<MobileBranding> MBrand = new List<MobileBranding>();
        //        if (CrossConnectivity.Current.IsConnected)
        //        {
        //            try
        //            {
        //                //var Check = DBHelper.UserScreenRetrive("SYSTEMCODES", _settings.DBPath, "SYSTEMCODES");
        //                //if (Check != null)
        //                //{
        //                //    if (!string.IsNullOrEmpty(Check?.ASSOC_FIELD_INFO))
        //                //    {
        //                //        MBrand = JsonConvert.DeserializeObject<List<MobileBranding>>(Check.ASSOC_FIELD_INFO.ToString());
        //                //    }
        //                //}
        //                //else
        //                // {
        //                var Res = DefaultAPIMethod.GetImageList();
        //                var Result = Res.GetValue("ResponseContent");
        //                if (!string.IsNullOrEmpty(Convert.ToString(Result)))
        //                {
        //                    MBrand = JsonConvert.DeserializeObject<List<MobileBranding>>(Result.ToString());
        //                }
        //                //}
        //                var settings = DependencyService.Get<IAppSettings>();


        //                string Code = settings.IsCustomLandingPage ? "B2VER" : "MBVER";
        //                settings.EntityImgURL = MBrand.Where(v => v.SYSTEM_CODE.ToUpper() == "ENTHM").FirstOrDefault()?.VALUE;
        //                settings.CasesImgURL = MBrand.Where(v => v.SYSTEM_CODE.ToUpper() == "CSHOM").FirstOrDefault()?.VALUE;
        //                settings.StandardImgURL = MBrand.Where(v => v.SYSTEM_CODE.ToUpper() == "STHOM").FirstOrDefault()?.VALUE;
        //                settings.CurrentVersion = MBrand.Where(v => v.SYSTEM_CODE.ToUpper() == Code).FirstOrDefault()?.VALUE;
        //                settings.ClientId = MBrand.Where(v => v.SYSTEM_CODE.ToUpper() == "SMLCL").FirstOrDefault()?.VALUE;
        //                settings.IsSAMLAuth = MBrand.Where(v => v.SYSTEM_CODE.ToUpper() == "MSAML").FirstOrDefault()?.VALUE == "Y" ? true : false;
        //                settings.ClientSecret = MBrand.Where(v => v.SYSTEM_CODE.ToUpper() == "SMLSR")?.FirstOrDefault()?.VALUE;
        //                settings.Scope = MBrand.Where(v => v.SYSTEM_CODE.ToUpper() == "SMLSC")?.FirstOrDefault()?.VALUE;
        //                settings.AuthorizeUrl = MBrand.Where(v => v.SYSTEM_CODE.ToUpper() == "SMLAU").FirstOrDefault()?.VALUE;
        //                settings.AccessTokenUrl = MBrand.Where(v => v.SYSTEM_CODE.ToUpper() == "SMLTU").FirstOrDefault()?.VALUE;
        //                settings.UserinfoUrl = MBrand.Where(v => v.SYSTEM_CODE.ToUpper() == "SMLUL").FirstOrDefault()?.VALUE;
        //                settings.RedirectUrl = MBrand.Where(v => v.SYSTEM_CODE.ToUpper() == "SMLRU").FirstOrDefault()?.VALUE;
        //                settings.Cert = MBrand.Where(v => v.SYSTEM_CODE.ToUpper() == "SMLCR").FirstOrDefault()?.VALUE;

        //                var CheckRec = DBHelper.GetAppTypeInfoListByTypeID_SystemName(settings.CurrentInstance.Id, "SYSTEMCODES", "SYSTEMCODES", settings.DBPath);

        //                AppTypeInformationModel _AppTypeInfoList = new AppTypeInformationModel();

        //                _AppTypeInfoList = new AppTypeInformationModel
        //                {
        //                    AssocFieldInfo = JsonConvert.SerializeObject(MBrand),
        //                    ModifiedAt = DateTime.Now,
        //                    System = "SYSTEMCODES",
        //                    TypeId = settings.CurrentInstance.Id,
        //                    ExternalId = 0,
        //                    CategoryId = 0,
        //                    CategoryName = "",
        //                    TransactionType = "M",
        //                    TypeScreenInfo = "SYSTEMCODES",
        //                    InstanceUserAssocId = ConstantsSync.INSTANCE_USER_ASSOC_ID,
        //                    IsOnline = true
        //                };

        //                if (CheckRec.Result == null)
        //                    _AppTypeInfoList.Id = 0;
        //                else
        //                    _AppTypeInfoList.Id = CheckRec.Result.Id;
        //                var y = DBHelper.SaveAppTypeInfo(_AppTypeInfoList, settings.DBPath);

        //            }
        //            catch (Exception ex)
        //            {
        //            }
        //        }
        //        else
        //        {
        //            try
        //            {
        //                GetBaseURLfromSQLite();
        //            }
        //            catch (Exception)
        //            {
        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {
        //    }
        //}

        //public static void GetBaseURLfromSQLite()
        //{
        //    try
        //    {
        //        IAppSettings settings = DependencyService.Get<IAppSettings>();

        //        var Result = DBHelper.UserScreenRetrive("SYSTEMCODES", settings.DBPath, "SYSTEMCODES");

        //        if (!string.IsNullOrEmpty(Result.AssocFieldInfo))
        //        {
        //            var MBrand = JsonConvert.DeserializeObject<List<MobileBranding>>(Result.AssocFieldInfo.ToString());
        //            settings.EntityImgURL = MBrand.Where(v => v.SYSTEM_CODE.ToUpper() == "ENTHM").FirstOrDefault().VALUE;
        //            settings.CasesImgURL = MBrand.Where(v => v.SYSTEM_CODE.ToUpper() == "CSHOM").FirstOrDefault().VALUE;
        //            settings.StandardImgURL = MBrand.Where(v => v.SYSTEM_CODE.ToUpper() == "STHOM").FirstOrDefault().VALUE;

        //            string Code = settings.IsCustomLandingPage ? "B2VER" : "MBVER";
        //            settings.CurrentVersion = MBrand.Where(v => v.SYSTEM_CODE.ToUpper() == Code).FirstOrDefault().VALUE;

        //            settings.ClientId = MBrand.Where(v => v.SYSTEM_CODE.ToUpper() == "SMLCL").FirstOrDefault().VALUE;
        //            settings.IsSAMLAuth = MBrand.Where(v => v.SYSTEM_CODE.ToUpper() == "MSAML").FirstOrDefault().VALUE == "Y" ? true : false;
        //            //App.ClientSecret = "c&;^#*%&.a{;+./V|v{$^?Y#[@.t&)8&])$!-";
        //            settings.ClientSecret = MBrand.Where(v => v.SYSTEM_CODE.ToUpper() == "SMLSR")?.FirstOrDefault()?.VALUE;
        //            //App.Scope = "https://graph.microsoft.com/.default";
        //           settings.Scope = MBrand.Where(v => v.SYSTEM_CODE.ToUpper() == "SMLSC")?.FirstOrDefault()?.VALUE;
        //            settings.AuthorizeUrl = MBrand.Where(v => v.SYSTEM_CODE.ToUpper() == "SMLAU").FirstOrDefault().VALUE;
        //            settings.AccessTokenUrl = MBrand.Where(v => v.SYSTEM_CODE.ToUpper() == "SMLTU").FirstOrDefault().VALUE;
        //            settings.UserinfoUrl = MBrand.Where(v => v.SYSTEM_CODE.ToUpper() == "SMLUL").FirstOrDefault().VALUE;
        //            settings.RedirectUrl = MBrand.Where(v => v.SYSTEM_CODE.ToUpper() == "SMLRU").FirstOrDefault().VALUE;
        //        }
        //    }
        //    catch (Exception)
        //    {
        //    }
        //}

        //public static void ClearApplocalData()
        //{
        //    try
        //    {
        //        Functions.UserName = string.Empty;
        //        Application.Current.Properties["UserName"] = string.Empty;

        //        Functions.UserFullName = string.Empty;
        //        Application.Current.Properties["UserFullName"] = string.Empty;

        //        Application.Current.Properties["IsLogin"] = false;
        //        Functions.IsLogin = false;

        //        Application.Current.Properties["IsPWDRemember"] = false;
        //        Functions.IsPWDRemember = false;

        //        Functions.UserLoginName = string.Empty;
        //        Application.Current.Properties["UserLoginName"] = string.Empty;

        //        Functions.UserPassword = string.Empty;
        //        Application.Current.Properties["UserPassword"] = string.Empty;

        //        Application.Current.Properties["Baseurl"] = DataServiceBus.OnlineHelper.DataTypes.Constants.Baseurl;

        //        Application.Current.Properties["Selected_Instance"] = Functions.Selected_Instance;

        //        Application.Current.Properties["InstanceName"] = Functions.InstanceName;

        //    }
        //    catch (Exception)
        //    {
        //    }

        //}


        #region Messages For Whole APp

        public static string Appinfomsg = "HkLibrary Central to Go v" + CrossDeviceInfo.Current.AppVersion + "(" + CrossDeviceInfo.Current.AppBuild + ")\nCopyright © 2018 by HkLibrary Enterprise LLC.";
        public static string Goonline_forFunc = "Please Go online to use this functionality!";

        public static string nRcrdOffline = "No Record Found.\nPlease go online to view full list.";
        public static string nRcrdOnline = "No Record Found.";

        #endregion

        public static string GenerateEntityFullURL(string Note)
        {
            IAppSettings settings = DependencyService.Get<IAppSettings>();

            string URl = "";
            try
            {
               
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error GettingEntity URL. ex: " + ex);
            }
            return URl;
        }

        public static string GenerateCasesFullURL(string Note)
        {
            string URl = string.Empty;
            IAppSettings settings = DependencyService.Get<IAppSettings>();
            try
            {
                //if (Note.Contains("/DownloadFile.aspx"))
                //    URl = Note.Replace("/DownloadFile.aspx", settings.CasesImgURL + "/DownloadFile.aspx");
                //else if (Note.Contains("/downloadfile.aspx"))
                //    URl = Note.Replace("/downloadfile.aspx", settings.CasesImgURL + "/DownloadFile.aspx");
                //else if (Note.ToString().Contains("'DownloadFile.aspx"))
                //    Note = Note.Replace("'DownloadFile.aspx", settings.CasesImgURL + "/DownloadFile.aspx");
                //else if (Note.ToString().Contains("'downloadFile.aspx"))
                //    Note = Note.Replace("'downloadFile.aspx", settings.CasesImgURL + "/DownloadFile.aspx");
                //else
                //    URl = Note;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error Getting full URL. ex: " + ex);
            }
            return URl.ToString();
        }

        //public static ImageSource GetImageFromEntityAssoc(List<AssociationField> AssociationFieldCollection)
        //{
        //    try
        //    {
        //        var FileID = AssociationFieldCollection.Where(x => x.AssocSystemCode == "PHGAL").FirstOrDefault().AssocMetaDataText.FirstOrDefault().EntityFileID;

        //        var EntityID = AssociationFieldCollection.Where(x => x.AssocSystemCode == "PHGAL").FirstOrDefault().AssocMetaDataText.FirstOrDefault().EntityID;

        //        string fileStr = string.Empty;

        //        Task.Run(() =>
        //        {
        //            var d = EntityAPIMethods.GetFileFromEntity(EntityID.ToString(), FileID.ToString(), Functions.UserName);
        //            fileStr = d.GetValue("ResponseContent").ToString();
        //        }).Wait();

        //        FileItem fileResp = JsonConvert.DeserializeObject<FileItem>(fileStr);

        //        return ImageSource.FromStream(() => new MemoryStream(fileResp.BLOB));
        //    }
        //    catch (Exception ex)
        //    {
        //        return ImageSource.FromFile("Assets/na.png");
        //    }
        //}

        public static async Task<bool> IsMediaPermissionGrantedAsync()
        {
            Plugin.Permissions.Abstractions.PermissionStatus persmissionstatus = Plugin.Permissions.Abstractions.PermissionStatus.Unknown;

            if (Device.RuntimePlatform == Device.Android || Device.RuntimePlatform == Device.UWP)
            {

                persmissionstatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Plugin.Permissions.Abstractions.Permission.Storage);

                if (persmissionstatus != Plugin.Permissions.Abstractions.PermissionStatus.Granted)
                {
                    await CrossPermissions.Current.RequestPermissionsAsync(Plugin.Permissions.Abstractions.Permission.Storage);
                    persmissionstatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Plugin.Permissions.Abstractions.Permission.Storage);
                }
            }

            if (Device.RuntimePlatform == Device.iOS)
            {
                persmissionstatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Plugin.Permissions.Abstractions.Permission.Photos);

                if (persmissionstatus != Plugin.Permissions.Abstractions.PermissionStatus.Granted)
                {
                    await CrossPermissions.Current.RequestPermissionsAsync(Plugin.Permissions.Abstractions.Permission.Photos);
                    persmissionstatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Plugin.Permissions.Abstractions.Permission.Photos);
                }
            }

            if (persmissionstatus == Plugin.Permissions.Abstractions.PermissionStatus.Granted)
            {
                return true;
            }

            return false;
        }


        //    public static void GetSAMLAuthenticationData()
        //{

        //    var result = DefaultAPIMethod.GetLogo("SMLCL,SMLSR,SMLSC,SMLAU,SMLTU,SMLUL,SMLRU,MSAML");
        //    var urlString = result.GetValue("ResponseContent")?.ToString();
        //    try
        //    {
        //        if (!string.IsNullOrEmpty(urlString))
        //        {
        //            var MBrand = JsonConvert.DeserializeObject<List<MobileBranding>>(urlString);
        //            //AppTypeInfoList _AppTypeInfoList = new AppTypeInfoList();
        //            try
        //            {
        //                if (MBrand != null)
        //                {
        //                    IAppSettings settings = DependencyService.Get<IAppSettings>();

        //                    foreach (MobileBranding temp in MBrand)
        //                    {
        //                        var Check = DBHelper.UserScreenRetrive("SYSTEMCODES", settings.DBPath, temp.SYSTEM_CODE);
        //                        var _AppTypeInfoList = new AppTypeInformationModel
        //                        {
        //                            AssocFieldInfo = JsonConvert.SerializeObject(temp.VALUE),
        //                            TypeScreenInfo = temp.SYSTEM_CODE
        //                        };

        //                        if (Check == null)
        //                            _AppTypeInfoList.Id = 0;
        //                        else
        //                            _AppTypeInfoList.Id = Check.Id;

        //                        var y = DBHelper.SaveAppTypeInfo(_AppTypeInfoList, settings.DBPath);

        //                    }

        //                    settings.ClientId = MBrand.Where(v => v.SYSTEM_CODE.ToUpper() == "SMLCL").FirstOrDefault().VALUE;
        //                    settings.IsSAMLAuth = MBrand.Where(v => v.SYSTEM_CODE.ToUpper() == "MSAML").FirstOrDefault().VALUE == "Y" ? true : false;
        //                    //App.ClientSecret = "c&;^#*%&.a{;+./V|v{$^?Y#[@.t&)8&])$!-";
        //                    settings.ClientSecret = MBrand.Where(v => v.SYSTEM_CODE.ToUpper() == "SMLSR")?.FirstOrDefault()?.VALUE;
        //                    //App.Scope = "https://graph.microsoft.com/.default";
        //                    settings.Scope = MBrand.Where(v => v.SYSTEM_CODE.ToUpper() == "SMLSC")?.FirstOrDefault()?.VALUE;
        //                    settings.AuthorizeUrl = MBrand.Where(v => v.SYSTEM_CODE.ToUpper() == "SMLAU").FirstOrDefault().VALUE;
        //                    settings.AccessTokenUrl = MBrand.Where(v => v.SYSTEM_CODE.ToUpper() == "SMLTU").FirstOrDefault().VALUE;
        //                    settings.UserinfoUrl = MBrand.Where(v => v.SYSTEM_CODE.ToUpper() == "SMLUL").FirstOrDefault().VALUE;
        //                    settings.RedirectUrl = MBrand.Where(v => v.SYSTEM_CODE.ToUpper() == "SMLRU").FirstOrDefault().VALUE;
        //                }

        //            }

        //            catch (Exception) { }
        //        }
        //    }
        //    catch (Exception) { }


        //}

        public static string DateFormatStringToString(string inputDate, int dateflag)
        {

            /*  0 - Full Date n Time,1 - Only Date , 2 - Only Time*/
            string sInput = inputDate;
            string sOutPut = string.Empty;
            try
            {

                DateTime Dout = new DateTime();
                DateTime.TryParse(inputDate, CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out Dout);

                switch (dateflag)
                {
                    case 0:// Full Date n Time
                        sOutPut = Dout.ToString();
                        break;
                    case 1: // Only Date
                        sOutPut = Dout.ToString("MM/dd/yyyy"); break;
                    case 2:// Only Time
                        sOutPut = Dout.ToString("hh:mm tt");
                        break;
                    default:
                        break;
                }



            }
            catch (Exception ex)
            {
                return sInput;
            }
            return sOutPut;
        }
        public static string GetInstallationID()
        {
            var uniqueInstallationId = Preferences.Get("installation_id", string.Empty);
            if (string.IsNullOrWhiteSpace(uniqueInstallationId))
            {
                uniqueInstallationId = System.Guid.NewGuid().ToString();
                Preferences.Set("installation_id", uniqueInstallationId);
            }

            return uniqueInstallationId;
        }

        public static string DoFormat(double myNumber)
        {
            var s = string.Format("{0:0.000000}", myNumber);

            if (s.EndsWith("00"))
            {
                return ((int)myNumber).ToString();
            }
            else
            {
                return s;
            }
        }
    }
}
