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



    }
}
