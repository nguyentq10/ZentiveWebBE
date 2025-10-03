using DAL.DBcontext;
using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        int SaveChangesWithTransaction();

        Task<int> SaveChangesWithTransactionAsync();

    }
    public class UnitOfWork : IUnitOfWork
    {
            private readonly ZenthicDBContext _context;
            
            private AccountRepository _accountRepository;
            private CategoryRepository _categoryRepository;
            private MediaAssetRepository _mediaAssetRepository;
            private PaymentRepository _paymentRepository;
            private PayoutAccountRepository _payoutAccountRepository;
            private PledgeRepository _pledgeRepository;
            private ProjectRepository _projectRepository;
            private ProjectApprovalRepository _projectApprovalRepository;
            private RewardTierRepository _rewardTierRepository; 
            private SiteDonationRepository _siteDonationRepository; 
            public UnitOfWork() => _context = new ZenthicDBContext();

            public AccountRepository AccountRepository
            {
                get
                {
                    return _accountRepository ??= new AccountRepository(_context);
                }
            }
            public CategoryRepository CategoryRepository
            {
                get
                {
                    return _categoryRepository ??= new CategoryRepository(_context);
                }
            }
            public MediaAssetRepository MediaAssetRepository
            {
                get
                {
                    return _mediaAssetRepository ??= new MediaAssetRepository(_context);
                }
        }
        public PaymentRepository PaymentRepository
        {
            get
            {
                return _paymentRepository ??= new PaymentRepository(_context);
            }
        }
        public PayoutAccountRepository PayoutAccountRepository
        {
            get
            {
                return _payoutAccountRepository ??= new PayoutAccountRepository(_context);
            }
        }
        public PledgeRepository PledgeRepository
        {
            get
            {
                return _pledgeRepository ??= new PledgeRepository(_context);
            }
        }
        public ProjectRepository ProjectRepository
        {
            get
            {
                return _projectRepository ??= new ProjectRepository(_context);
            }
        }
        public ProjectApprovalRepository ProjectApprovalRepository
        {
            get
            {
                return _projectApprovalRepository ??= new ProjectApprovalRepository(_context);
            }
        }
        public RewardTierRepository RewardTierRepository
        {
            get
            {
                return _rewardTierRepository ??= new RewardTierRepository(_context);
            }
        }
        public SiteDonationRepository SiteDonationRepository
        {
            get
            {
                return _siteDonationRepository ??= new SiteDonationRepository(_context);
            }
        }
        public void Dispose()
        {
            throw new NotImplementedException();
        }
        public int SaveChangesWithTransaction()
        {
            int result = -1;

            //System.Data.IsolationLevel.Snapshot
            using (var dbContextTransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    result = _context.SaveChanges();
                    dbContextTransaction.Commit();
                }
                catch (Exception)
                {
                    //Log Exception Handling message                      
                    result = -1;
                    dbContextTransaction.Rollback();
                }
            }

            return result;
        }

        public async Task<int> SaveChangesWithTransactionAsync()
        {
            int result = -1;

            //System.Data.IsolationLevel.Snapshot
            using (var dbContextTransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    result = await _context.SaveChangesAsync();
                    dbContextTransaction.Commit();
                }
                catch (Exception)
                {
                    //Log Exception Handling message                      
                    result = -1;
                    dbContextTransaction.Rollback();
                }
            }

            return result;
        }
    }
     
}
