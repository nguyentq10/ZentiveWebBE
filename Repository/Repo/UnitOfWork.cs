using DAL.DBcontext;
using Repository.Repo; // Hoặc namespace chứa các repository của bạn
using System;
using System.Threading.Tasks;

namespace Repository.Repo
{
    public interface IUnitOfWork : IDisposable
    {
        AccountRepository AccountRepository { get; }
        CategoryRepository CategoryRepository { get; }
        MediaAssetRepository MediaAssetRepository { get; }
        PaymentRepository PaymentRepository { get; }
        PayoutAccountRepository PayoutAccountRepository { get; }
        PledgeRepository PledgeRepository { get; }
        ProjectRepository ProjectRepository { get; }
        ProjectApprovalRepository ProjectApprovalRepository { get; }
        RewardTierRepository RewardTierRepository { get; }
        SiteDonationRepository SiteDonationRepository { get; }

        Task<int> SaveChangesAsync();
    }
namespace Repository.Repo
    {
        public class UnitOfWork : IUnitOfWork
        {
            private readonly ZenthicDBContext _context;

            // Khai báo các thuộc tính Repository
            public AccountRepository AccountRepository { get; private set; }
            public CategoryRepository CategoryRepository { get; private set; }
            public MediaAssetRepository MediaAssetRepository { get; private set; }
            public PaymentRepository PaymentRepository { get; private set; }
            public PayoutAccountRepository PayoutAccountRepository { get; private set; }
            public PledgeRepository PledgeRepository { get; private set; }
            public ProjectRepository ProjectRepository { get; private set; }
            public ProjectApprovalRepository ProjectApprovalRepository { get; private set; }
            public RewardTierRepository RewardTierRepository { get; private set; }
            public SiteDonationRepository SiteDonationRepository { get; private set; }

            // CHỈ SỬ DỤNG CONSTRUCTOR NÀY
            // DbContext sẽ được inject tự động bởi Dependency Injection
            public UnitOfWork(ZenthicDBContext context)
            {
                _context = context;

                // Khởi tạo tất cả các repository một lần duy nhất
                AccountRepository = new AccountRepository(_context);
                CategoryRepository = new CategoryRepository(_context);
                MediaAssetRepository = new MediaAssetRepository(_context);
                PaymentRepository = new PaymentRepository(_context);
                PayoutAccountRepository = new PayoutAccountRepository(_context);
                PledgeRepository = new PledgeRepository(_context);
                ProjectRepository = new ProjectRepository(_context);
                ProjectApprovalRepository = new ProjectApprovalRepository(_context);
                RewardTierRepository = new RewardTierRepository(_context);
                SiteDonationRepository = new SiteDonationRepository(_context);
            }

            // Phương thức Save duy nhất
            public async Task<int> SaveChangesAsync()
            {
                // DbContext.SaveChangesAsync đã tự quản lý transaction
                // cho tất cả các thay đổi đã được theo dõi (tracked).
                return await _context.SaveChangesAsync();
            }

            // Implement Dispose để giải phóng tài nguyên cho DbContext
            public void Dispose()
            {
                _context.Dispose();
            }
        }
    }
}