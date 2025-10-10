using Repository.Models;
using Repository.Repo;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Services
{
    public class ProjectServices : IProjectServices
    {
        private readonly IUnitOfWork _unitOfWork;

        // Thêm constructor để inject IUnitOfWork
        public ProjectServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task CreateAsync(Project p)
        {
            // Bước 1: Chuẩn bị hành động Create
            _unitOfWork.ProjectRepository.Create(p);
            // Bước 2: Lưu tất cả thay đổi vào DB
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            // Bước 1: Tìm đối tượng cần xóa
            var project = await _unitOfWork.ProjectRepository.GetByIdAsync(id);
            if (project == null)
            {
                return false;
            }

            // Bước 2: Chuẩn bị hành động Remove
            _unitOfWork.ProjectRepository.Remove(project);

            // Bước 3: Lưu thay đổi vào DB
            var result = await _unitOfWork.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> UpdateAsync(Project p)
        {
            // Bước 1: Chuẩn bị hành động Update
            _unitOfWork.ProjectRepository.Update(p);

            // Bước 2: Lưu thay đổi vào DB
            var result = await _unitOfWork.SaveChangesAsync();
            return result > 0;
        }

        public async Task<List<Project>> GetAllAsync()
        {
            return await _unitOfWork.ProjectRepository.GetAllAsync();
        }

        public async Task<Project> GetByIdAsync(Guid id)
        {
            // Giả sử ProjectRepository có GetByIdAsync(Guid id)
            return await _unitOfWork.ProjectRepository.GetByIdAsync(id);
        }
    }
}