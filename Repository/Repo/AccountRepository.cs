using DAL.DBcontext;
using Microsoft.EntityFrameworkCore;
using Repository.Basic;
using Repository.Models;
using System;
using System.Threading.Tasks;

namespace Repository.Repo
{
    public class AccountRepository : GenericRepository<Account>
    {
        // Constructor này nhận DbContext và truyền nó lên cho lớp cha (GenericRepository)
        public AccountRepository(ZenthicDBContext context) : base(context)
        {
        }

        /// <summary>
        /// Lấy tài khoản bằng email, chỉ lấy tài khoản còn hoạt động.
        /// Hàm này sẽ được dùng cho chức năng Login.
        /// </summary>
        public async Task<Account?> GetByEmailAsync(string email)
        {
            return await _context.Accounts.FirstOrDefaultAsync(a => a.Email == email && a.IsActive);
        }

        /// <summary>
        /// Lấy tài khoản bằng Id, không phân biệt trạng thái.
        /// </summary>
        public async Task<Account?> GetByIdAsync(Guid id)
        {
            return await _context.Accounts.FirstOrDefaultAsync(u => u.Id == id);
        }

        /// <summary>
        /// Kiểm tra xem email đã tồn tại trong hệ thống hay chưa.
        /// Dùng cho chức năng Register.
        /// </summary>
        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _context.Accounts.AnyAsync(u => u.Email == email);
        }
        public async Task<int> CountActiveUsersAsync()
        {
            // Giả sử model của bạn có IsDeleted
            return await _context.Accounts // Hoặc Users
                .CountAsync(u => u.IsActive == false);
        }
    }
}