using System;
using System.Collections.Generic;
using System.Text;

namespace Pardis.Domain.Dto.Users
{
    public class AuthLogDTO
    {
        public string UserId { get; set; }
        public string Client { get; set; }
        public string Ip { get; set; }
    }
}
