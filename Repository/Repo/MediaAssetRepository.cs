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
    public class MediaAssetRepository : GenericRepository<MediaAsset>
    {
        public MediaAssetRepository(ZenthicDBContext context) : base(context)
        {
        }
        public async Task<List<MediaAsset>> GetMediaForProjectAsync(Guid projectId)
        {
        return await _context.MediaAssets
            .Where(m => m.ProjectId == projectId) // Nhớ kiểm tra IsDeleted
            .OrderBy(m => m.SortOrder) // Sắp xếp theo thứ tự
            .ToListAsync();
         }

    }

}
