using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authorization.Domain
{
    public class Invitation
    {
        public Guid Id { get; set; }
        public required string Email { get; set; }
        public required string Token { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
