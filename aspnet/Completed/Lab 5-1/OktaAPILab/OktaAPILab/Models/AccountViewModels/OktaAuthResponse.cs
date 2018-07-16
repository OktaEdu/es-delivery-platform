using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OktaAPILab.Models.AccountViewModels
{
    public class OktaAuthResponse
    {
        public string status { get; set; }
        public string sessionToken { get; set; }
    }
}
