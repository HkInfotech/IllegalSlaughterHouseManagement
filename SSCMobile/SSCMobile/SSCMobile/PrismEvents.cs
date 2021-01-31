using Prism.Events;
using System.Collections.Generic;

namespace SSCMobile
{
    public class SyncUpdatePayload
    {
        public string Message { get; set; }
        public bool Success { get; set; }
    }

    public class SyncUpdateNotificationEvent : PubSubEvent<SyncUpdatePayload>
    {
    }

    public class HtmlConverterPayload
    {
        public string PageName { get; set; }
        public string Url { get; set; }
        public string AppType { get; set; }
        public bool IsNavigate { get; set; }
        public string FileName { get; set; }
    }

    public class HtlmConverterNotificationEvent : PubSubEvent<HtmlConverterPayload>
    {
    }

    public class ListViewItemPayload
    {
        public IEnumerable<object> Items { get; set; }
    }

    public class ListViewItemNotificationEvent : PubSubEvent<ListViewItemPayload>
    {
    }
}