using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pardis.Domain.Dto.Users
{

    public class UserResource
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Mobile { get; set; }
        public string IsActive { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
    }
}
