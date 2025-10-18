using Microsoft.EntityFrameworkCore.Internal;
using Services.Request;
using Services.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IAccountServices
    {
        Task<int> GetTotalUserCountAsync();

    }
}
