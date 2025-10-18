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
    public class PaymentRepository : GenericRepository<Payment>
    {
        public PaymentRepository(ZenthicDBContext context) : base(context)
        {

        }
        public async Task<Payment> FindByExternalIdAsync(string externalId)
        {
            return await _context.Payments
                .FirstOrDefaultAsync(p => p.ExternalId == externalId);
        }
    }
}
