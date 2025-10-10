using DAL.DBcontext;
using Repository.Basic;
using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repo
{
    public class ProjectApprovalRepository : GenericRepository<ProjectApproval>
    {
        public ProjectApprovalRepository(ZenthicDBContext context) : base(context)
        {
        }
    }
}
