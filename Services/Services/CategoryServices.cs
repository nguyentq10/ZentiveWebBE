using Repository.Repo;
using Services.Interface;
using Services.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class CategoryServices : ICategoryServices
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<List<CategoryResponse>> GetActiveCategoriesAsync()
        {
            // Bước 1: Gọi Repository để lấy dữ liệu thô
            var categories = await _unitOfWork.CategoryRepository.GetActiveCategoriesAsync();

            // Bước 2: Chuyển đổi (map) từ Model sang DTO
            var categoryDtos = categories.Select(c => new CategoryResponse
            {
                Id = c.Id,
                Name = c.Name,
                Slug = c.Slug
            }).ToList();

            return categoryDtos;
        }
    }
}
