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
    public class CategoryRepository : GenericRepository<Category>
    {
        public CategoryRepository(ZenthicDBContext context) : base(context)
        {
        }
        public async Task<List<Category>> GetActiveCategoriesAsync()
        {
            return await _context.Categories
                .Where(c => c.IsActive) // Lọc các category đang hoạt động
                .ToListAsync();
        }
    }
}
