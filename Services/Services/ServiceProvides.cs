using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{   public interface IServiceProvides
    {
        AccountServices AccountServices { get; }
        IAccountServices IAccountServices { get; }
        CategoryServices CategoryServices { get; }
        ICategoryServices ICategoryServices { get; }
        MediaAssetServices MediaAssetServices { get; }
        IMediaAssetServices IMediaAssetServices { get; }
        PaymentServices PaymentServices { get; }
        IPaymentServices IPaymentServices { get; }
        PayoutAccountServices PayoutAccountServices { get; }
        IPayoutAccountServices IPayoutAccountServices { get; }  
        PledgeServices PledgeServices { get; }
        IPledgeServices IPledgeServices { get; }    
        ProjectServices ProjectServices { get; }    
        IProjectServices IProjectServices { get; }
        ProjectApprovalService ProjectApprovalService { get; } 
        IProjectApprovalServices IProjectApprovalServices { get; }  
        RewardTierServices RewardTierServices { get; }
        IRewardTierServices IRewardTierServices { get; }
        SiteDonationSerivces SiteDonationSerivces { get; }
        ISiteDonationServices ISiteDonationServices { get; }
    }
    public class ServiceProvides : IServiceProvides
    {
        private AccountServices _accountServices;
        private CategoryServices _categoryServices;
        private MediaAssetServices _mediaAssetServices;
        private PaymentServices _paymentServices;
        private PayoutAccountServices _payoutAccountServices;
        private PledgeServices _pledgeServices;
        private ProjectServices _projectServices;
        private ProjectApprovalService _projectApprovalService;
        private RewardTierServices _rewardTierServices;
        private SiteDonationSerivces _siteDonationSerivces;

        IServiceProvider _IServiceProvider { get; }
        IAccountServices _IAccountServices { get; }

    ICategoryServices _ICategoryServices { get; }

        IMediaAssetServices _IMediaAssetServices { get; }

        IPaymentServices _IPaymentServices { get; }

        IPayoutAccountServices _IPayoutAccountServices { get; }

        IPledgeServices _IPledgeServices { get; }

        IProjectServices _IProjectServices { get; }

        IProjectApprovalServices _IProjectApprovalServices { get; }

        IRewardTierServices _IRewardTierServices { get; }

        ISiteDonationServices _ISiteDonationServices { get; }

        public ServiceProvides() { }

        public AccountServices AccountServices
        {
            get
            {
                return _accountServices ??= new AccountServices();
            }
        }

        public CategoryServices CategoryServices
        {
            get
            {
                return _categoryServices ??= new CategoryServices();
            }
        }

        public MediaAssetServices MediaAssetServices
        {
            get
            {
                return _mediaAssetServices ??= new MediaAssetServices();
            }
        }   


        public PaymentServices PaymentServices
        {
            get
            {
                return _paymentServices ??= new PaymentServices();
            }
        }   

        public PayoutAccountServices PayoutAccountServices
        {
            get
            {
                return _payoutAccountServices ??= new PayoutAccountServices();
            }
        }   

        public PledgeServices PledgeServices
        {
            get
            {
                return _pledgeServices ??= new PledgeServices();
            }
        }

        public ProjectServices ProjectServices
        {
            get
            {
                return _projectServices ??= new ProjectServices();
            }
        }

        public ProjectApprovalService ProjectApprovalService
        {
            get
            {
                return _projectApprovalService ??= new ProjectApprovalService();
            }
        }   

        public RewardTierServices RewardTierServices
        {
            get
            {
                return _rewardTierServices ??= new RewardTierServices();
            }
        }

        public SiteDonationSerivces SiteDonationSerivces
        {
            get
            {
                return _siteDonationSerivces ??= new SiteDonationSerivces();
            }
        }
        public IAccountServices IAccountServices
        {
            get
            {
                return _accountServices ??= new AccountServices();
            }
        }

        public ICategoryServices ICategoryServices
        {
            get
            {
                return _categoryServices ??= new CategoryServices();
            }
        }

        public IMediaAssetServices IMediaAssetServices
        {
            get
            {
                return _mediaAssetServices ??= new MediaAssetServices();
            }
        }

        public IPaymentServices IPaymentServices
        {
            get
            {
                return _paymentServices ??= new PaymentServices();
            }
        }

        public IPayoutAccountServices IPayoutAccountServices
        {
            get
            {
                return _payoutAccountServices ??= new PayoutAccountServices();
            }
        }

        public IPledgeServices IPledgeServices
        {
            get
            {
                return _pledgeServices ??= new PledgeServices();
            }
        }   

        public IProjectServices IProjectServices
        {
            get
            {
                return _projectServices ??= new ProjectServices();
            }
        }   

        public IProjectApprovalServices IProjectApprovalServices
        {
            get
            {
                return _projectApprovalService ??= new ProjectApprovalService();
            }
        }   

        public IRewardTierServices IRewardTierServices
        {
            get
            {
                return _rewardTierServices ??= new RewardTierServices();
            }
        }   

        public ISiteDonationServices ISiteDonationServices
        {
            get
            {
                return _siteDonationSerivces ??= new SiteDonationSerivces();
            }
        }   
    }
}
