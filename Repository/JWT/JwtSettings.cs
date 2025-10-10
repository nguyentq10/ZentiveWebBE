using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.JWT
{
    public class JwtSettings
    {
        public string SecretKey { get; set; } 
        public int ExpireMinutes { get; set; } = 60;
    }
}
