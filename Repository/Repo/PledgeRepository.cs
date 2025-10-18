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
    public class PledgeRepository : GenericRepository<Pledge>
    {
        public PledgeRepository(ZenthicDBContext context) : base(context) { }

        // Phương thức mới để lấy Pledges với phân trang và filter
        public async Task<(IEnumerable<Pledge> Pledges, int TotalCount)> GetPledgesForProjectAsync(Guid projectId, PledgeQueryParameters queryParams)
        {
            // Xây dựng câu truy vấn động
            var query = _context.Pledges
                .Where(p => p.ProjectId == projectId)
                .Include(p => p.Backer) // Quan trọng: Lấy kèm thông tin người ủng hộ
                .OrderByDescending(p => p.CreatedAt)
                .AsQueryable();

            // Áp dụng filter nếu có
            if (!string.IsNullOrEmpty(queryParams.Status))
            {
                query = query.Where(p => p.Status == queryParams.Status);
            }

            // Lấy tổng số lượng bản ghi TRƯỚC khi phân trang
            var totalCount = await query.CountAsync();

            // Áp dụng phân trang
            var pledges = await query
                .Skip((queryParams.Page - 1) * queryParams.PageSize)
                .Take(queryParams.PageSize)
                .ToListAsync();

            return (pledges, totalCount);
        }
    }
    public class PledgeQueryParameters
    {
        private const int MaxPageSize = 50;
        private int _pageSize = 10;

        public int Page { get; set; } = 1;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }

        public string? Status { get; set; }
    }
}

