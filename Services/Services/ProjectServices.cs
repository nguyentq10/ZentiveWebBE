using Repository.Models;
using Repository.Repo;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class ProjectServices : IProjectServices
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProjectServices() => _unitOfWork = new UnitOfWork();

        public async Task<int> CreateAsync(Project p)
        {
            return await _unitOfWork.ProjectRepository.CreateAsync(p);
        }
        public async Task<bool> DeleteAsync(int id)
        {
            var items = await _unitOfWork.ProjectRepository.GetByIdAsync(id);
            return await _unitOfWork.ProjectRepository.RemoveAsync(items);
        }

        public async Task<List<Project>> GetAllAsync()
        {
            return await _unitOfWork.ProjectRepository.GetAllAsync();
        }

        public async Task<Project> GetIdAsync(Guid id)
        {
            return await _unitOfWork.ProjectRepository.GetByIdAsync(id);
        }

    }
}
