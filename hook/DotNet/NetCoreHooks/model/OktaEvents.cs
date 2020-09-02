using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreHooks.model
{
    public class OktaEvents
    {        
        public String EventType { get; set; }
        public String DisplayMessage { get; set; }
        public String EventTime { get; set; }
        public OktaEvents()
        { }
        public OktaEvents(string eventType, string displayMessage, string eventTime)
        {
            this.EventType = eventType;
            this.DisplayMessage = displayMessage;
            this.EventTime = eventTime;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"EventType: {this.EventType}");
            sb.Append($"EventTime: {this.EventTime}");
            sb.Append($"DisplayMessage: {this.DisplayMessage}");
            return sb.ToString();
        }
    }
}
