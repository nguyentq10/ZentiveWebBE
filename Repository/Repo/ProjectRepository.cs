using DAL.DBcontext;
using Microsoft.EntityFrameworkCore;
using Repository.Basic;
using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repo
{
    public class ProjectRepository : GenericRepository<Project>
    {
        public ProjectRepository(ZenthicDBContext context) : base(context)
        {
        }

        public async Task<List<Project>> GetAllAsync()
        {
            var items = await _context.Projects
                .Include(p => p.Category)
                .Include(p => p.Creator)
                .Include(p => p.MediaAssets)
                .Include(p => p.Pledges)
                .Include(p => p.ProjectApprovals)
                .Include(p => p.RewardTiers)
                .ToListAsync();
            return items ?? new List<Project>();
        }
        public async Task<Project> GetByIdAsync(Guid id)
        {
            var items = await _context.Projects
                .Include(p => p.Category)
                .Include(p => p.Creator)
                .Include(p => p.MediaAssets)
                .Include(p => p.Pledges)
                .Include(p => p.ProjectApprovals)
                .Include(p => p.RewardTiers)
                .FirstOrDefaultAsync(m => m.Id == id);
            return items ?? new Project();
        }

        public async Task<(List<Project> projects, int totalCount)> QueryProjectsAsync(
    string? searchQuery,
    Guid? categoryId,
    string status,
    string sortBy,
    int page,
    int pageSize)
        {
            // Bắt đầu với một IQueryable để xây dựng truy vấn động
            var query = _context.Projects
                .Include(p => p.Creator) // Include Creator để lấy tên
                .AsQueryable();

            // 1. Lọc theo Status
            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(p => p.Status == status);
            }

            // 2. Lọc theo CategoryId
            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

           //// 3. Lọc theo từ khóa tìm kiếm (q)
            if (!string.IsNullOrWhiteSpace(searchQuery))
           {
               query = query.Where(p => p.Title.Contains(searchQuery) || p.Title.Contains(searchQuery));
            }

            // Lấy tổng số lượng kết quả TRƯỚC khi phân trang
            var totalCount = await query.CountAsync();

            // 4. Sắp xếp (Sort)
            switch (sortBy?.ToLower())
            {
                case "popular":
                    
                    query = query.OrderByDescending(p => p.CurrentAmount);
                    break;
                case "ending":
                    query = query.OrderBy(p => p.EndAt);
                    break;
                case "newest":
                default:
                    query = query.OrderByDescending(p => p.CreatedAt);
                    break;
            }
            var projects = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (projects, totalCount);
        }



    }
}
