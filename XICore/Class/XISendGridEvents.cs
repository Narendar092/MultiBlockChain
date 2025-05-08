using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XICore
{
    public class XISendGridEvents
    {
        public string @event { get; set; }
        public string email { get; set; }
        public string category { get; set; }
        public string response { get; set; }
        public string attempt { get; set; }
        public string timestamp { get; set; }
        public string url { get; set; }
        public string status { get; set; }
        public string reason { get; set; }
        public string type { get; set; }
        public string useragent { get; set; }
        public string ip { get; set; }
        public string sg_event_id { get; set; }
        public string sg_message_id { get; set; }
    }
}