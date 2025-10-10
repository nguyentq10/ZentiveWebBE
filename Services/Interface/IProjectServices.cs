using Repository.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IProjectServices
    {
        Task<List<Project>> GetAllAsync();
        Task<Project> GetByIdAsync(Guid id);
        Task CreateAsync(Project p);
        Task<bool> UpdateAsync(Project p);
        Task<bool> DeleteAsync(Guid id);
    }
}