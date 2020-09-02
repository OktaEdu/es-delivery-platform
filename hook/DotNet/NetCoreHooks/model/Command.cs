using System;
using System.Collections.Generic;


namespace NetCoreHooks.model
{
    public class Command
    {
        public string type { get; set; }

        public Dictionary<String, String> value { get; set; }

        public Error error { get; set; }
        
    }
}
