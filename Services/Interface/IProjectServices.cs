using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IProjectServices
    {
        Task<int> CreateAsync(Repository.Models.Project p);
        Task<bool> DeleteAsync(int id);
        Task<List<Repository.Models.Project>> GetAllAsync();
        Task<Repository.Models.Project> GetIdAsync(Guid id);
    }
}
