using Microsoft.AspNetCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NetCoreHooks.model
{
    public class OktaHookResponse
    {
        public List<Command> commands { get; set; }
        //public Error Error { get; set; }

        public OktaHookResponse()
        {
            commands = new List<Command>();
        }

        public override string ToString()
        {
            string json = JsonConvert.SerializeObject(this, Formatting.Indented);
            return json;
        }
    }
}
