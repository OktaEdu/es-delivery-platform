using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreHooks.Contracts
{
    public interface IRepositoryBase<T> where T: class
    {
        Task<IList<T>> FindAll();
        Task<T> FindById(int id);
        Task<T> FindByUserName(string userName);
        
    }
}
