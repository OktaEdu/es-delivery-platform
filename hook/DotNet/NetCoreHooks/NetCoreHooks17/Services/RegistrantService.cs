using NetCoreHooks.Contracts;
using NetCoreHooks.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NetCoreHooks.Data;

namespace NetCoreHooks.Services
{
    public class RegistrantService : IRegistrantRepository
    {
        private readonly RegistrantContext _db;

        public RegistrantService(RegistrantContext context)
        {
            _db = context;
        }
        public async Task<IList<Registrant>> FindAll()
        {
            var registrants = await _db.Registrants.ToListAsync();
            return registrants;
        }

        public async Task<Registrant> FindById(int id)
        {
            var registrant = await _db.Registrants.FindAsync(id);
            return registrant;
        }

        public async Task<Registrant> FindByUserName(string userName)
        {
            var registrant = await _db.Registrants
                .Where(n => n.UserName == userName)
                .FirstOrDefaultAsync();
            return registrant;
        }
    }
}
