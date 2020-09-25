using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NetCoreHooks.Data;
using NetCoreHooks.model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreHooks.Services
{
    public class DataGenerationService
    {
        public static void Initialize(IServiceProvider sp)
        {
            /*use the incoming ServiceProvider object to create a RegistrantContext object. Use the
             context to add new registrants to the InMemory database. Those "rows" will be available to 
            subsequent calls from controllers that accept the injected context.*/
            using (var ctx = new RegistrantContext(sp.GetRequiredService<DbContextOptions<RegistrantContext>>()))
            {
                Registrant r1 = new Registrant { Id = 1, FirstName = "Laura", LastName = "Mipsum", UserName = "laura.mipsum@oktaice.com", SSN = "333-33-3333" };
                Registrant r2 = new Registrant { Id = 2, FirstName = "Hannibal", LastName = "Smith", UserName = "hannibal.smith@oktaice.com", SSN = "123-45-6789" };
                Registrant r3 = new Registrant { Id = 3, FirstName = "Jane", LastName = "Zielinski", UserName = "jane.zielinski@oktaice.com", SSN = "987-65-4321" };
                Registrant r4 = new Registrant { Id = 4, FirstName = "Hank", LastName = "Aaron", UserName = "hank.aaron@oktaice.com", SSN = "444-44-4444" };
                Registrant r5 = new Registrant { Id = 5, FirstName = "Slick", LastName = "Salesman", UserName = "slick.salesman@oktaice.com", SSN = "555-55-5555" };
                Registrant r6 = new Registrant { Id = 6, FirstName = "Quinn", LastName = "Morelli", UserName = "quinn.morelli@oktaice.com", SSN = "222-22-2222" };
                Registrant r7 = new Registrant { Id = 7, FirstName = "Javier", LastName = "Lopez", UserName = "javier.lopez@oktaice.com", SSN = "777-77-7777" };

                ctx.Registrants.Add(r1);
                ctx.Registrants.Add(r2);
                ctx.Registrants.Add(r3);
                ctx.Registrants.Add(r4);
                ctx.Registrants.Add(r5);
                ctx.Registrants.Add(r6);
                ctx.Registrants.Add(r7);
                ctx.SaveChanges();
                Debug.WriteLine("registrant count: " + ctx.Registrants.Count());
            }

        }
    }
}
