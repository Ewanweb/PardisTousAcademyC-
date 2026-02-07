using System;

namespace Pardis.Domain.Dto.Users
{
    public class AuthLogDTO
    {
        public string UserId { get; set; } = string.Empty;
        public string Client { get; set; } = string.Empty;
        public string Ip { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
