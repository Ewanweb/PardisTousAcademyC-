using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Pardis.Domain.Users
{
    public class AuthLog : BaseEntity
    {
        public string UserId { get; private set; } = string.Empty;

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }

        public string Client { get; private set; }

        public string Ip { get; private set; }

        private AuthLog()
        {
            
        }

        public AuthLog(string client, string ip, string userId)
        {
            Client = client;
            Ip = ip;
            UserId = userId;
        }

    }
}
