using SSCMobileServiceBus.OfflineSync.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSCMobile.Models.Common
{
    public class MediaAsset
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public MediaAssetType Type { get; set; }
        public string PreviewPath { get; set; }
        public string Path { get; set; }
        public bool IsSelect { get; set; } = false;
    }
}
