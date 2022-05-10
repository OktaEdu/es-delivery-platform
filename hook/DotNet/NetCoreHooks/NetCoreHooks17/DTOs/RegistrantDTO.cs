using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreHooks.DTOs
{
    public class RegistrantDTO
    {
        private string _ssn;
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string SSN
        {
            get
            {
                return _ssn;
            }
            set
            {
                if (_ssn != null)
                {
                    _ssn = _ssn.Replace("-", "");
                }
            }
        }
        public string UserName { get; set; }
    }
}
