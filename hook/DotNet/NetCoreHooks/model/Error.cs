using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreHooks.model
{
    public class Error
    {
        public string ErrorSummary { get; set; }
        public List<ErrorCause> ErrorCauses { get; set; }
    }
}
