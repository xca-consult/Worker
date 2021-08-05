using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Database
{
    public interface IRepositoryWatcher
    {
        Task<List<Guid>> GetNewUsers();  
    }
}