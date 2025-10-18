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
    public class SiteDonationRepository : GenericRepository<SiteDonation>
    {
        public SiteDonationRepository(ZenthicDBContext context) : base(context)
        {
        }

        public async Task<(IEnumerable<SiteDonation> Donations, int TotalCount)> GetAllDonationsAsync(PaginationQueryParameters queryParams)
        {
            var query = _context.SiteDonations
                .Include(d => d.Donor) // Lấy thông tin người ủng hộ (Account)
                .OrderByDescending(d => d.CreatedAt) // Sắp xếp mới nhất trước
                .AsQueryable();

            // Thêm filter theo ngày tháng nếu cần
            // if (queryParams.FromDate.HasValue) query = query.Where(d => d.DonationDate >= queryParams.FromDate);
            // if (queryParams.ToDate.HasValue) query = query.Where(d => d.DonationDate <= queryParams.ToDate);

            var totalCount = await query.CountAsync();

            var donations = await query
                .Skip((queryParams.Page - 1) * queryParams.PageSize)
                .Take(queryParams.PageSize)
                .ToListAsync();

            return (donations, totalCount);
        }
    }
    public class PaginationQueryParameters
    {
        private const int MaxPageSize = 50; // Tăng giới hạn cho Admin
        private int _pageSize = 20;

        public int Page { get; set; } = 1;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }

        // Có thể thêm filter theo ngày tháng ở đây nếu cần
        // public DateTime? FromDate { get; set; }
        // public DateTime? ToDate { get; set; }
    }
}
