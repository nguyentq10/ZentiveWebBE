using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Response
{
    public class CreateDonationResponseDto
    {
        public string ClientSecret { get; set; }
        public string PublishableKey { get; set; }
    }
}
