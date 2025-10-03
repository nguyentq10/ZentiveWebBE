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
    public class PaymentRepository : GenericRepository<Payment>
    {
        public PaymentRepository() { }
        public PaymentRepository(ZenthicDBContext context) => _context = context;
    }
}
