using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OktaAPILab.Models
{
    public class OktaError
    {
        public string errorCode { get; set; }
        public string errorSummary { get; set; }
        public string errorId { get; set; }
    }
}
